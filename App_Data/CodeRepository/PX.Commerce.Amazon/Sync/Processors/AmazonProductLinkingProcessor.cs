/* ---------------------------------------------------------------------*
*                             Acumatica Inc.                            *

*              Copyright (c) 2005-2024 All rights reserved.             *

*                                                                       *

*                                                                       *

* This file and its contents are protected by United States and         *

* International copyright laws.  Unauthorized reproduction and/or       *

* distribution of all or any portion of the code contained herein       *

* is strictly prohibited and will result in severe civil and criminal   *

* penalties.  Any violations of this copyright will be prosecuted       *

* to the fullest extent possible under law.                             *

*                                                                       *

* UNDER NO CIRCUMSTANCES MAY THE SOURCE CODE BE USED IN WHOLE OR IN     *

* PART, AS THE BASIS FOR CREATING A PRODUCT THAT PROVIDES THE SAME, OR  *

* SUBSTANTIALLY THE SAME, FUNCTIONALITY AS ANY ACUMATICA PRODUCT.       *

*                                                                       *

* THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE.              *

* --------------------------------------------------------------------- */

using Microsoft.SqlServer.Server;
using PX.Api.ContractBased.Models;
using PX.Commerce.Amazon.API.Rest;
using PX.Commerce.Amazon.API.Rest.Constants;
using PX.Commerce.Core;
using PX.Commerce.Core.API;
using PX.Commerce.Objects;
using PX.Commerce.Objects.Substitutes;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CN.Common.Services.DataProviders;
using PX.Objects.IN;
using PX.Objects.IN.RelatedItems;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon
{
	public class AmazonProductLinkingEntityBucket : EntityBucketBase, IEntityBucket
	{
		public IMappedEntity Primary => Product;
		public IMappedEntity[] Entities => new IMappedEntity[] { Primary };

		public MappedProductLinkingItem Product;
	}

	[DebuggerDisplay("IsValid = {IsValid} - NoMatch = {NoMatch}")]
	public class MatchStockItemVerificationResult
	{
		public bool IsValid { get; set; }
		public bool HasMatch
		{
			get { return AcceptableInventoryCDs.Count > 0; }
		}
		public List<string> AcceptableInventoryCDs { get; set; }
		public List<string> UnacceptableInventoryCDs { get; set; }
		public bool HasDuplicate
		{
			get { return AcceptableInventoryCDs.Count > 1; }
		}

		public string MatchingItem
		{
			get
			{
				if (AcceptableInventoryCDs.Count == 1)
				{
					return AcceptableInventoryCDs[0];
				}
				else
				{
					return string.Empty;
				}
			}
		}
		public bool InventoryInactive { get; set; }
		public bool CrossRefInactive { get; set; }

		public static MatchStockItemVerificationResult Default
		{
			get
			{
				return new MatchStockItemVerificationResult()
				{
					AcceptableInventoryCDs = new List<string>(),
					UnacceptableInventoryCDs = new List<string>(),
					InventoryInactive = false,
					CrossRefInactive = false
				};
			}

		}
	}

	[DebuggerDisplay("MatchedInventoryItem = {MatchedInventoryItem?.InventoryCD} - MatchedCrossRefs = {MatchedCrossRefs.Count}")]
	public class MatchStockItemResult
	{
		public string MatchingId { get; set; }
		public InventoryItem MatchedInventoryItem { get; set; }
		public List<InventoryItem> MatchedCrossRefs { get; set; }
	}

	[BCProcessor(typeof(BCAmazonConnector), BCEntitiesAttribute.ProductLinkingOnly, BCCaptions.ProductLinkingOnly, sortOrder: 50,
		IsInternal = false,
		Direction = SyncDirection.Import,
		PrimaryDirection = SyncDirection.Import,
		PrimarySystem = PrimarySystem.Extern,
		PrimaryGraph = typeof(PX.Objects.IN.InventoryItemMaint),
		ExternTypes = new Type[] { },
		LocalTypes = new Type[] { },
		AcumaticaPrimaryType = typeof(PX.Objects.IN.InventoryItem),
		AcumaticaPrimarySelect = typeof(Search<InventoryItem.inventoryCD, Where<InventoryItem.stkItem, Equal<True>>>),
		URL = "/skucentral?mSku={0}"
	)]
	public class AmazonProductLinkingProcessor : BCProcessorSingleBase<AmazonProductLinkingProcessor, AmazonProductLinkingEntityBucket, MappedProductLinkingItem>, IProcessor
	{

		private ReportRunner<MerchantListingData> _reportRunner;
		private ListingsDataProvider _listingDataProvider;

		public override async Task Initialise(IConnector connector, ConnectorOperation operation, CancellationToken cancellationToken = default)
		{
			await base.Initialise(connector, operation);
			var client = ((BCAmazonConnector)connector).GetRestClient(GetBindingExt<BCBindingAmazon>(), GetBinding());

			var reportDataProvider = new ReportDataProvider(client, ReportTypes.GET_MERCHANT_LISTINGS_DATA, new[] { GetBindingExt<BCBindingAmazon>().Marketplace });
			var reportConverter = new ReportConverterCSV();
			var reportReader = new ReportReader();
			_reportRunner = new ReportRunner<MerchantListingData>(reportDataProvider, reportConverter, reportReader);
			_listingDataProvider = new ListingsDataProvider(client);
		}

		#region Common

		public override Task<MappedProductLinkingItem> PullEntity(string externID, string externalInfo, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public override Task<MappedProductLinkingItem> PullEntity(Guid? localID, Dictionary<string, object> externalInfo, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Import

		public override async Task FetchBucketsForImport(DateTime? minDateTime, DateTime? maxDateTime, PXFilterRow[] filters, CancellationToken cancellationToken = default)
		{
			IEnumerable<MerchantListingData> allListings = await _reportRunner
				.GetRecordsFromReport();

			var productsFBM = allListings
				.Where(listing => listing.FulfillmentChannel == ListingFulfillmentChannel.Default);

			foreach (MerchantListingData product in productsFBM)
			{
				AmazonProductLinkingEntityBucket bucket = CreateBucket();

				BCSyncStatus previousStatus = GetBCSyncStatusResult(BCEntitiesAttribute.ProductLinkingOnly, externID: product.SellerSku);
				MappedProductLinkingItem obj = bucket.Product = bucket.Product.Set(product, product.SellerSku, product.SellerSku.ToString(), previousStatus?.ExternTS ?? DateTime.Now);
				EntityStatus status = EnsureStatus(obj, SyncDirection.Import);
			}
		}

		public override async Task<EntityStatus> GetBucketForImport(AmazonProductLinkingEntityBucket bucket, BCSyncStatus status, CancellationToken cancellationToken = default)
		{
			MerchantListingData external = new MerchantListingData();
			MappedProductLinkingItem newObj = bucket.Product = bucket.Product.Set(external, status.ExternID, status.ExternID.ToString(), status.ExternTS ?? DateTime.Now);
			return EnsureStatus(newObj, SyncDirection.Import);
		}

		public override async Task MapBucketImport(AmazonProductLinkingEntityBucket bucket, IMappedEntity existing, CancellationToken cancellationToken = default)
		{
			MappedProductLinkingItem obj = bucket.Product;
			var marketPlaceId = GetBindingExt<BCBindingAmazon>().Marketplace;
			var sellerPartnerId = GetBindingExt<BCBindingAmazon>().SellerPartnerId;
			var product = await _listingDataProvider.GetListing(sellerPartnerId, obj.ExternID, marketPlaceId);
			string ASIN = string.Empty;
			if (product?.Summaries != null && product.Summaries.Count > 0)
				ASIN = product.Summaries.FirstOrDefault().ASIN;
			StockItem impl = obj.Local = MatchStockItem(obj.ExternID, ASIN);
		}


		protected virtual MatchStockItemResult FindLocalMatchingStockItemsById(string externalId)
		{
			PX.Objects.IN.InventoryItem potentialInventoryItem = SelectFrom<PX.Objects.IN.InventoryItem>
							.Where<PX.Objects.IN.InventoryItem.inventoryCD.IsEqual<@P.AsString>>
							.View.Select(this, externalId);

			var potentialCrossRefs = PXSelectJoin<PX.Objects.IN.INItemXRef,
						 InnerJoin<PX.Objects.IN.InventoryItem,
							On<PX.Objects.IN.INItemXRef.inventoryID, Equal<PX.Objects.IN.InventoryItem.inventoryID>>>,
						 Where<PX.Objects.IN.INItemXRef.alternateType, Equal<INAlternateType.global>,
							And<PX.Objects.IN.INItemXRef.alternateID, Equal<Required<PX.Objects.IN.INItemXRef.alternateID>>>>>
						.Select(this, externalId)
						.RowCast<PX.Objects.IN.InventoryItem>();

			return new MatchStockItemResult
			{
				MatchingId = externalId,
				MatchedInventoryItem = potentialInventoryItem,
				MatchedCrossRefs = potentialCrossRefs.ToList()
			};
		}

		/// <summary>
		/// Match local stock item to Amazon SellerSku. If it does not find it, try to match by ASIN.
		/// </summary>
		/// <param name="sellerSku"></param>
		/// <param name="ASIN"></param>
		/// <returns></returns>

		protected virtual StockItem MatchStockItem(string sellerSku, string ASIN)
		{
			var matchedBySellerSku = string.IsNullOrEmpty(sellerSku) ? null : FindLocalMatchingStockItemsById(sellerSku);
			var matchedBySellerSkuVerification = string.IsNullOrEmpty(sellerSku) ? MatchStockItemVerificationResult.Default : VerifyUsingMatchingRules(sellerSku, matchedBySellerSku.MatchedInventoryItem, matchedBySellerSku.MatchedCrossRefs);

			//Matching by SKU didn't work. Try to match by ASIN.
			var matchedByASIN = string.IsNullOrEmpty(ASIN) ? null : FindLocalMatchingStockItemsById(ASIN);
			var matchedByASINVerification = string.IsNullOrEmpty(ASIN) ? MatchStockItemVerificationResult.Default : VerifyUsingMatchingRules(ASIN, matchedByASIN.MatchedInventoryItem, matchedByASIN.MatchedCrossRefs);

			CheckForDuplicatesAndRaiseError(sellerSku, ASIN, matchedBySellerSkuVerification, matchedByASINVerification);

			if (matchedBySellerSkuVerification.HasMatch && matchedBySellerSkuVerification.IsValid)
			{
				PX.Objects.IN.InventoryItem matched = matchedBySellerSku.MatchedInventoryItem?.ItemStatus.IsIn(AcceptableStatuses) == true ?
											matchedBySellerSku.MatchedInventoryItem : matchedBySellerSku.MatchedCrossRefs.Where(x => x.ItemStatus.IsIn(AcceptableStatuses)).FirstOrDefault();
				return new StockItem()
				{
					InventoryID = matched.InventoryCD.ValueField(),
					Description = matched.Descr.ValueField(),
					NoteID = matched.NoteID.ValueField()
				};
			}

			if (matchedByASINVerification.HasMatch && matchedByASINVerification.IsValid)
			{
				PX.Objects.IN.InventoryItem matched = matchedByASIN.MatchedInventoryItem?.ItemStatus.IsIn(AcceptableStatuses) == true ?
											matchedByASIN.MatchedInventoryItem : matchedByASIN.MatchedCrossRefs.Where(x => x.ItemStatus.IsIn(AcceptableStatuses)).FirstOrDefault();
				return new StockItem()
				{
					InventoryID = matched.InventoryCD.ValueField(),
					Description = matched.Descr.ValueField(),
					NoteID = matched.NoteID.ValueField()
				};
			}

			if (matchedBySellerSkuVerification.InventoryInactive || matchedByASINVerification.InventoryInactive ||
				matchedBySellerSkuVerification.CrossRefInactive || matchedByASINVerification.CrossRefInactive)
			{
				throw new PXException(AmazonMessages.SellerSKUOrASINMatchIsInactive, sellerSku, ASIN, string.Join(", ", matchedBySellerSkuVerification.UnacceptableInventoryCDs.Union(matchedByASINVerification.UnacceptableInventoryCDs)));
			}

			//Both matching by SKU and ASIN didn't work. Raise an exception
			//Get the right message to display.
			if (!matchedBySellerSkuVerification.HasMatch && !matchedByASINVerification.HasMatch)
			{
				throw new PXException(AmazonMessages.SellerSKUAndASINHaveNoMatch, sellerSku, ASIN);
			}

			return null;
		}

		private void CheckForDuplicatesAndRaiseError(string sellerSku, string ASIN,
													 MatchStockItemVerificationResult matchedBySellerSkuVerification,
													 MatchStockItemVerificationResult matchedByASINVerification)
		{
			var validIventoryIds = matchedBySellerSkuVerification.AcceptableInventoryCDs.Union(matchedByASINVerification.AcceptableInventoryCDs);
			if (validIventoryIds.Count() > 1)
			{
				throw new PXException(AmazonMessages.SellerSKUOrASINDuplicateMatches, sellerSku, ASIN, string.Join(", ", matchedBySellerSkuVerification.AcceptableInventoryCDs.Union(matchedByASINVerification.AcceptableInventoryCDs)));
			}
		}

		protected static readonly string[] AcceptableStatuses = new[] { INItemStatus.Active, INItemStatus.NoPurchases, InventoryItemStatus.NoRequest };
		protected static readonly string[] UnacceptableStatuses = new[] { INItemStatus.ToDelete, INItemStatus.NoSales, INItemStatus.Inactive };
		protected virtual MatchStockItemVerificationResult VerifyUsingMatchingRules(string ExternID, InventoryItem potentialInventoryItem, IEnumerable<InventoryItem> potentialCrossRefs)
		{
			List<string> acceptableInventoryCDs = GetListOfInventoryCDByStatuses(AcceptableStatuses, potentialInventoryItem, potentialCrossRefs);
			List<string> UnacceptableInventoryCDs = GetListOfInventoryCDByStatuses(UnacceptableStatuses, potentialInventoryItem, potentialCrossRefs);

			bool inventoryInactive = potentialInventoryItem != null && potentialInventoryItem.ItemStatus.IsNotIn(AcceptableStatuses) == true;
			bool crossRefInactive = potentialCrossRefs != null && potentialCrossRefs.Count() != 0 && potentialCrossRefs.All(i => i.ItemStatus.IsNotIn(AcceptableStatuses));

			return new MatchStockItemVerificationResult()
			{
				IsValid = acceptableInventoryCDs.Count == 1,
				CrossRefInactive = crossRefInactive,
				InventoryInactive = inventoryInactive,
				AcceptableInventoryCDs = acceptableInventoryCDs,
				UnacceptableInventoryCDs = UnacceptableInventoryCDs
			};
		}

		protected virtual List<string> GetListOfInventoryCDByStatuses(string[] statuses, InventoryItem matchedInventoryItem, IEnumerable<InventoryItem> matchedCrossRefs)
		{
			List<string> resultListOfCDs = new List<string>();
			if (matchedInventoryItem?.ItemStatus.IsIn(statuses) == true)
			{
				resultListOfCDs.Add(matchedInventoryItem.InventoryCD.Trim());
			}
			resultListOfCDs.AddRange(matchedCrossRefs.Where(x => x.ItemStatus.IsIn(statuses)).Select(x => x.InventoryCD.Trim()));

			return resultListOfCDs;
		}

		public override async Task SaveBucketImport(AmazonProductLinkingEntityBucket bucket, IMappedEntity existing, string operation, CancellationToken cancellationToken = default)
		{
			MappedProductLinkingItem obj = bucket.Product;
			bucket.Product.AddLocal(obj.Local, obj.Local.NoteID.Value, DateTime.Now);
			UpdateStatus(obj, operation);
		}

		#endregion

		#region Export
		public override Task FetchBucketsForExport(DateTime? minDateTime, DateTime? maxDateTime, PXFilterRow[] filters, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public override async Task<EntityStatus> GetBucketForExport(AmazonProductLinkingEntityBucket bucket, BCSyncStatus status, CancellationToken cancellationToken = default)
		{
			return EntityStatus.None;
		}

		public override Task SaveBucketExport(AmazonProductLinkingEntityBucket bucket, IMappedEntity existing, string operation, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}
		#endregion

	}
}
