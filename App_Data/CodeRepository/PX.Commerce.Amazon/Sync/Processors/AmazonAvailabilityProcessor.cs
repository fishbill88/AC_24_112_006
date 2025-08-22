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

using PX.Api;
using PX.Commerce.Amazon.API;
using PX.Commerce.Amazon.API.Rest;
using PX.Commerce.Amazon.API.Rest.Client.Common;
using PX.Commerce.Amazon.API.Rest.Constants;
using PX.Commerce.Core;
using PX.Commerce.Core.API;
using PX.Commerce.Objects;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon
{
	public class AmazonAvailabilityEntityBucket : EntityBucketBase, IEntityBucket
	{
		public IMappedEntity Primary => Product;
		public IMappedEntity[] Entities => new IMappedEntity[] { Primary };
		public MappedAvailability Product;
	}

	[BCProcessor(typeof(BCAmazonConnector), BCEntitiesAttribute.ProductAvailability, BCCaptions.ProductAvailability, sortOrder: 60,
		IsInternal = false,
		Direction = SyncDirection.Export,
		PrimaryDirection = SyncDirection.Export,
		PrimarySystem = PrimarySystem.Local,
		PrimaryGraph = typeof(PX.Objects.IN.InventorySummaryEnq),
		ExternTypes = new Type[] { },
		LocalTypes = new Type[] { },
		AcumaticaPrimaryType = typeof(PX.Objects.IN.InventoryItem),
		URL = "/skucentral?mSku={0}",
		Requires = new string[] { BCEntitiesAttribute.ProductLinkingOnly }
	)]
	public class AmazonAvailabilityProcessor : AvailabilityProcessorBase<AmazonAvailabilityProcessor, AmazonAvailabilityEntityBucket, MappedAvailability>, IProcessor
	{
		
		[InjectDependency]
		public Func<IAmazonRestClient, JsonFeedData, IJsonFeedDataProvider> JsonFeedDataProviderFactory { get; set; }

		public IJsonFeedDataProvider JsonFeedDataProvider;
		[InjectDependency]
		public IAmazonReportReader ReportReader { get; set; }

		public async override Task Initialise(IConnector iconnector, ConnectorOperation operation, CancellationToken cancellationToken = default)
		{
			await base.Initialise(iconnector, operation);
			var client = ((BCAmazonConnector)iconnector).GetRestClient(GetBindingExt<BCBindingAmazon>(), GetBinding());
			JsonFeedData feedData = new()
			{
				Marketplace = GetBindingExt<BCBindingAmazon>().Marketplace,
				SellerId = GetBindingExt<BCBindingAmazon>().SellerPartnerId,
				FeedType = FeedTypes.JSON_LISTINGS_FEED
			};
			JsonFeedDataProvider = JsonFeedDataProviderFactory(client, feedData);
		}

		#region Export

		public override async Task FetchBucketsExport(CancellationToken cancellationToken = default)
		{
			DateTime? startDate = Operation.PrepareMode == PrepareMode.Incremental ? GetEntityStats()?.LastIncrementalExportDateTime : Operation.StartDate;
			IEnumerable<StorageDetailsResult> results = Enumerable.Empty<StorageDetailsResult>();
			if (GetEntity(BCEntitiesAttribute.ProductLinkingOnly)?.IsActive == true)
			{
				results = results.Concat(FetchStorageDetails(GetBindingExt<BCBindingExt>(), startDate, Operation.EndDate, FetchAvailabilityBaseCommandForStockItem));
			}
			// TODO: this part is not implemented for the processor
			if (GetEntity(BCEntitiesAttribute.ProductWithVariant)?.IsActive == true)
			{
				results = results.Concat(FetchStorageDetails(GetBindingExt<BCBindingExt>(), startDate, Operation.EndDate, FetchAvailabilityBaseCommandForTemplateItem));
			}

			foreach (StorageDetailsResult lineItem in results)
			{
				if (lineItem.IsTemplate.Value == true)
					continue;
				DateTime? lastModified = new DateTime?[] { lineItem.SiteLastModifiedDate?.Value, lineItem.InventoryLastModifiedDate?.Value }.Where(d => d != null).Select(d => d.Value).Max();
				MappedAvailability obj = new MappedAvailability(lineItem, lineItem.InventoryNoteID.Value, lastModified, lineItem.ParentSyncId.Value);
				EntityStatus status = EnsureStatus(obj, SyncDirection.Export);
				if (status == EntityStatus.Deleted) status = EnsureStatus(obj, SyncDirection.Export, resync: true);
			}
		}

		/// TODO: Probably we need to get rid of this overriding 
		/// Had to override this method to pass AmazonBCEntitiesAttribute.productLinkingOnly
		public override PXView FetchAvailabilityBaseCommandForStockItem(BCBindingExt bindingExt, DateTime? startTime, DateTime? endTime, ref List<object> parameters)
		{
			var commandBQL = FetchAvailabilityBaseCommand(bindingExt,
				typeof(InnerJoin<PX.Objects.IN.InventoryItem,
						On<PX.Objects.IN.InventoryItem.noteID, Equal<BCSyncStatus.localID>,
						And<BCSyncStatus.entityType, Equal<BCEntitiesAttribute.productLinkingOnly>>>,
					LeftJoin<INSiteStatusByCostCenter,
						On<PX.Objects.IN.InventoryItem.inventoryID, Equal<INSiteStatusByCostCenter.inventoryID>,
						And<INSiteStatusByCostCenter.costCenterID, Equal<CostCenter.freeStock>>>>>),
				typeof(Where<BCSyncStatus.entityType, Equal<BCEntitiesAttribute.productLinkingOnly>>),
				ref parameters);

			if (startTime != null)
			{
				commandBQL.WhereAnd(BqlCommand.Compose(typeof(Where<,,>), typeof(INSiteStatusByCostCenter.lastModifiedDateTime), typeof(GreaterEqual<>), typeof(P.AsDateTime),
					typeof(Or<,>), typeof(PX.Objects.IN.InventoryItem.lastModifiedDateTime), typeof(GreaterEqual<>), typeof(P.AsDateTime)));
				parameters.Add(startTime);
				parameters.Add(startTime);
			}

			if (endTime != null)
			{
				commandBQL.WhereAnd(BqlCommand.Compose(typeof(Where<,,>), typeof(INSiteStatusByCostCenter.lastModifiedDateTime), typeof(LessEqual<>), typeof(P.AsDateTime),
					typeof(Or<,>), typeof(PX.Objects.IN.InventoryItem.lastModifiedDateTime), typeof(LessEqual<>), typeof(P.AsDateTime)));
				parameters.Add(endTime);
				parameters.Add(endTime);
			}
			return commandBQL;
		}

		// This logic is fully taken from the BC availability processor
		public override async Task<List<AmazonAvailabilityEntityBucket>> GetBucketsExport(List<BCSyncStatus> ids, CancellationToken cancellationToken = default)
		{
			BCEntityStats entityStats = GetEntityStats();
			BCBinding binding = GetBinding();
			BCBindingExt bindingExt = GetBindingExt<BCBindingExt>();
			List<AmazonAvailabilityEntityBucket> buckets = new List<AmazonAvailabilityEntityBucket>();

			var warehouses = new Dictionary<int, INSite>();
			List<BCLocations> locationMappings = BCLocationSlot.GetBCLocations(bindingExt.BindingID);
			Dictionary<int, Dictionary<int, PX.Objects.IN.INLocation>> siteLocationIDs = BCLocationSlot.GetWarehouseLocations(bindingExt.BindingID);

			if (bindingExt.WarehouseMode == BCWarehouseModeAttribute.SpecificWarehouse)
			{
				warehouses = BCLocationSlot.GetWarehouses(bindingExt.BindingID);
			}
			Boolean anyLocation = locationMappings.Any(x => x.LocationID != null);

			IEnumerable<StorageDetailsResult> response = GetStorageDetailsResults(bindingExt, ids) as IEnumerable<StorageDetailsResult>;

			if (response == null || response.Any() == false) return buckets;

			List<StorageDetailsResult> results = new List<StorageDetailsResult>();
			foreach (var detailsGroup in response.GroupBy(r => new { InventoryID = r.InventoryCD?.Value, /*SiteID = r.SiteID?.Value*/ }))
			{
				if (detailsGroup.First().Availability?.Value == BCItemAvailabilities.DoNotUpdate)
					continue;
				StorageDetailsResult result = detailsGroup.First();
				result.SiteLastModifiedDate = detailsGroup.Where(d => d.SiteLastModifiedDate?.Value != null).Select(d => d.SiteLastModifiedDate.Value).Max().ValueField();
				result.LocationLastModifiedDate = detailsGroup.Where(d => d.LocationLastModifiedDate?.Value != null).Select(d => d.LocationLastModifiedDate.Value).Max().ValueField();
				result.SiteOnHand = detailsGroup.Sum(k => k.SiteOnHand?.Value ?? 0m).ValueField();
				result.SiteAvailable = detailsGroup.Sum(k => k.SiteAvailable?.Value ?? 0m).ValueField();
				result.SiteAvailableforIssue = detailsGroup.Sum(k => k.SiteAvailableforIssue?.Value ?? 0m).ValueField();
				result.SiteAvailableforShipping = detailsGroup.Sum(k => k.SiteAvailableforShipping?.Value ?? 0m).ValueField();
				if (bindingExt.WarehouseMode == BCWarehouseModeAttribute.SpecificWarehouse && locationMappings.Any() == false)//if warehouse is specific but nothing is configured in table
				{
					result.LocationOnHand = result.LocationAvailable = result.LocationAvailableforIssue = result.LocationAvailableforShipping = 0m.ValueField();
				}
				else
				{
					if (detailsGroup.Any(i => i.SiteID?.Value != null))
					{
						result.LocationOnHand = anyLocation ? detailsGroup.Where
							(k => warehouses.Count <= 0
							|| (siteLocationIDs.ContainsKey(k.SiteID?.Value ?? 0)
								&& (siteLocationIDs[k.SiteID?.Value ?? 0].Count == 0
								|| (k.LocationID?.Value != null
									&& siteLocationIDs[k.SiteID?.Value ?? 0].ContainsKey(k.LocationID.Value.Value)))))
							.Sum(k => k.LocationOnHand?.Value ?? 0m).ValueField() : null;
						result.LocationAvailable = anyLocation ? detailsGroup.Where(
							k => warehouses.Count <= 0
							|| (siteLocationIDs.ContainsKey(k.SiteID?.Value ?? 0)
								&& (siteLocationIDs[k.SiteID?.Value ?? 0].Count == 0
								|| (k.LocationID?.Value != null
									&& siteLocationIDs[k.SiteID?.Value ?? 0].ContainsKey(k.LocationID.Value.Value)))))
							.Sum(k => k.LocationAvailable?.Value ?? 0m).ValueField() : null;
						result.LocationAvailableforIssue = anyLocation ? detailsGroup.Where(
							k => warehouses.Count <= 0
							|| (siteLocationIDs.ContainsKey(k.SiteID?.Value ?? 0)
								&& (siteLocationIDs[k.SiteID?.Value ?? 0].Count == 0
								|| (k.LocationID?.Value != null
								&& siteLocationIDs[k.SiteID?.Value ?? 0].ContainsKey(k.LocationID.Value.Value)))))
							.Sum(k => k.LocationAvailableforIssue?.Value ?? 0m).ValueField() : null;
						result.LocationAvailableforShipping = anyLocation ? detailsGroup.Where(
							k => warehouses.Count <= 0
							|| (siteLocationIDs.ContainsKey(k.SiteID?.Value ?? 0)
								&& (siteLocationIDs[k.SiteID?.Value ?? 0].Count == 0
								|| (k.LocationID?.Value != null
								&& siteLocationIDs[k.SiteID?.Value ?? 0].ContainsKey(k.LocationID.Value.Value)))))
							.Sum(k => k.LocationAvailableforShipping?.Value ?? 0m).ValueField() : null;
					}
					else
						result.LocationOnHand = result.LocationAvailable = result.LocationAvailableforIssue = result.LocationAvailableforShipping = null;
				}
				results.Add(result);
			}

			if (results != null)
			{
				var stockItems = results.Where(x => x.TemplateItemID?.Value == null);
				if (stockItems != null)
				{
					foreach (StorageDetailsResult line in results)
					{
						if (line.IsTemplate?.Value == true)
							continue;

						AmazonAvailabilityEntityBucket bucket = new AmazonAvailabilityEntityBucket();
						DateTime lastModified = new DateTime?[] { line.LocationLastModifiedDate?.Value, line.SiteLastModifiedDate?.Value, line.InventoryLastModifiedDate?.Value }.Where(d => d != null).Select(d => d.Value).Max();
						MappedAvailability obj = bucket.Product = new MappedAvailability(line, line.InventoryNoteID?.Value, lastModified, line.ParentSyncId.Value);
						EntityStatus status = EnsureStatus(obj, SyncDirection.Export);

						obj.ParentID = line.ParentSyncId.Value;
						if (Operation.PrepareMode != PrepareMode.Reconciliation && Operation.PrepareMode != PrepareMode.Full && status != EntityStatus.Pending && Operation.SyncMethod != SyncMode.Force)
						{
							SynchronizeStatus(bucket.Product, BCSyncOperationAttribute.Reconfiguration);
							Statuses.Cache.Persist(PXDBOperation.Update);
							Statuses.Cache.Persisted(false);
							continue;
						}

						buckets.Add(bucket);
					}
				}
			}

			return buckets;
		}

		/// Had to override this method to pass AmazonBCEntitiesAttribute.productLinkingOnly
		public override PXView GetAvailabilityBaseCommand(BCBindingExt bindingExt, List<BCSyncStatus> statuses, out List<object> parameters)
		{
			parameters = new List<object>();
			var siteLocations = BCLocationSlot.GetWarehouseLocations(bindingExt.BindingID);
			List<Type> typesList = new List<Type> {
				typeof(Select5<,,,,>),
				typeof(BCSyncStatus),
				typeof(LeftJoin<,,>), typeof(BCSyncDetail),
				typeof(On<BCSyncStatus.syncID, Equal<BCSyncDetail.syncID>,
						And<BCSyncDetail.entityType, Equal<BCEntitiesAttribute.variant>>>),
				typeof(InnerJoin<,,>), typeof(Objects.Availability.ChildInventoryItem),
				typeof(On<Objects.Availability.ChildInventoryItem.noteID, Equal<BCSyncDetail.localID>,
						Or<Objects.Availability.ChildInventoryItem.noteID, Equal<BCSyncStatus.localID>>>),
				typeof(InnerJoin<,,>), typeof(PX.Objects.IN.InventoryItem),
				typeof(On<PX.Objects.IN.InventoryItem.noteID, Equal<BCSyncStatus.localID>>),
				typeof(LeftJoin<,,>), typeof(PX.Objects.IN.INSite),
				typeof(On<PX.Objects.IN.INSite.active, Equal<True>>),
				typeof(LeftJoin<,,>), typeof(Objects.Availability.InventoryItemINUnit),
				typeof(On<Objects.Availability.InventoryItemINUnit.inventoryID, Equal<PX.Objects.IN.InventoryItem.inventoryID>,
						And<Objects.Availability.InventoryItemINUnit.fromUnit, Equal<PX.Objects.IN.InventoryItem.salesUnit>,
						And<Objects.Availability.InventoryItemINUnit.toUnit, Equal<PX.Objects.IN.InventoryItem.baseUnit>>>>),
				typeof(LeftJoin<,,>), typeof(Objects.Availability.ItemClassINUnit),
				typeof(On<Objects.Availability.ItemClassINUnit.itemClassID, Equal<PX.Objects.IN.InventoryItem.itemClassID>,
						And<Objects.Availability.ItemClassINUnit.fromUnit, Equal<PX.Objects.IN.InventoryItem.salesUnit>,
						And<Objects.Availability.ItemClassINUnit.toUnit, Equal<PX.Objects.IN.InventoryItem.baseUnit>>>>),
				typeof(LeftJoin<,,>), typeof(Objects.Availability.GlobalINUnit),
				typeof(On<Objects.Availability.GlobalINUnit.unitType, Equal<Objects.Availability.GlobalINUnit.globalUnitType>,
						And<Objects.Availability.GlobalINUnit.fromUnit, Equal<PX.Objects.IN.InventoryItem.salesUnit>,
						And<Objects.Availability.GlobalINUnit.toUnit, Equal<PX.Objects.IN.InventoryItem.baseUnit>>>>),
			};

			Type groupBy = typeof(Aggregate<GroupBy<Objects.Availability.ChildInventoryItem.inventoryID, GroupBy<INSiteStatusByCostCenter.siteID>>>);
			if (BCLocationSlot.GetExportBCLocations(bindingExt.BindingID).Any(x => x.LocationID != null))
			{
				groupBy = typeof(Aggregate<GroupBy<Objects.Availability.ChildInventoryItem.inventoryID, GroupBy<INSiteStatusByCostCenter.siteID, GroupBy<INLocationStatusByCostCenter.locationID>>>>);
				typesList.Add(typeof(LeftJoin<,,>));
				typesList.Add(typeof(INSiteStatusByCostCenter));
				typesList.Add(typeof(On<INSiteStatusByCostCenter.inventoryID, Equal<Objects.Availability.ChildInventoryItem.inventoryID>,
					And<PX.Objects.IN.INSite.siteID, Equal<INSiteStatusByCostCenter.siteID>,
					And<INSiteStatusByCostCenter.costCenterID, Equal<CostCenter.freeStock>>>>));
				typesList.Add(typeof(LeftJoin<INLocationStatusByCostCenter,
					On<INSiteStatusByCostCenter.inventoryID.IsEqual<INLocationStatusByCostCenter.inventoryID>
						.And<INSiteStatusByCostCenter.siteID.IsEqual<INLocationStatusByCostCenter.siteID>
						.And<INLocationStatusByCostCenter.costCenterID.IsEqual<CostCenter.freeStock>>>>>));
			}
			else
			{
				typesList.Add(typeof(LeftJoin<,>));
				typesList.Add(typeof(INSiteStatusByCostCenter));
				typesList.Add(typeof(On<INSiteStatusByCostCenter.inventoryID, Equal<Objects.Availability.ChildInventoryItem.inventoryID>,
					And<PX.Objects.IN.INSite.siteID, Equal<INSiteStatusByCostCenter.siteID>,
					And<INSiteStatusByCostCenter.costCenterID, Equal<CostCenter.freeStock>>>>));
			}

			Type where = typeof(Where<Objects.Availability.ChildInventoryItem.stkItem.IsEqual<True>
					.And<Objects.Availability.ChildInventoryItem.exportToExternal.IsEqual<True>>
					.And<BCSyncStatus.connectorType.IsEqual<P.AsString>>
					.And<BCSyncStatus.bindingID.IsEqual<P.AsInt>>
					.And<Brackets<BCSyncStatus.entityType.IsEqual<BCEntitiesAttribute.productLinkingOnly>
						.Or<BCSyncStatus.entityType.IsEqual<BCEntitiesAttribute.productWithVariant>>>>
					.And<BCSyncStatus.localID.IsNotNull>
					.And<BCSyncStatus.externID.IsNotNull>
					.And<BCSyncStatus.deleted.IsNotEqual<True>>>);

			typesList.Add(where);
			typesList.Add(groupBy);
			typesList.Add(typeof(OrderBy<Asc<BCSyncStatus.syncID, Asc<Objects.Availability.ChildInventoryItem.inventoryID>>>));

			Type select = BqlCommand.Compose(typesList.ToArray());
			BqlCommand cmd = BqlCommand.CreateInstance(select);
			PXView view = new PXView(this, true, cmd);

			parameters.Add(Operation.ConnectorType);
			parameters.Add(bindingExt?.BindingID);

			Type siteConditions = null;
			foreach (var key in siteLocations.Keys)
			{
				siteConditions = siteConditions == null ? BqlCommand.Compose(typeof(Where<,>), typeof(PX.Objects.IN.INSite.siteID), typeof(Equal<>), typeof(P.AsInt)) :
					BqlCommand.Compose(typeof(Where2<,>), siteConditions, typeof(Or<,>), typeof(PX.Objects.IN.INSite.siteID), typeof(Equal<>), typeof(P.AsInt));
				parameters.Add(key);
			}
			if (siteConditions != null)
				view.WhereAnd(siteConditions);

			view = ComposeStatusConditions(view, statuses, ref parameters);

			return view;
		}

		public override async Task MapBucketExport(AmazonAvailabilityEntityBucket entity, IMappedEntity existing, CancellationToken cancellationToken = default)
		{
			StorageDetailsResult storageDetailsResult = entity.Product.Local;
			InventoryMessageData inventoryFulfillmentData = entity.Product.Extern = new InventoryMessageData();

			int productQty = GetStorageQty(GetBindingExt<BCBindingExt>(), storageDetailsResult);
			InventoryMessage inventoryFulfillment = new(storageDetailsResult.ProductExternID.Value, productQty);

			inventoryFulfillmentData.InventoryMessages.Add(inventoryFulfillment);

		}

		public override async Task SaveBucketsExport(List<AmazonAvailabilityEntityBucket> buckets, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(GetBindingExt<BCBindingAmazon>().SellerPartnerId))
				throw new PXException(PXMessages.Localize(AmazonMessages.ProductAvailabilityCannotBeProcessedBecauseOfSellingPartnerId));

			List<InventoryMessage> feedMessages = CreateListOfMessages(buckets);
			await ProcessFeed(buckets, feedMessages);
		}

		//TODO: Move ProcessFeed to a separate class

		/// <summary>
		/// only proceed further to attempt exporting availabilities if there's any feed message
		/// </summary>
		/// <param name="buckets"></param>
		/// <param name="feedMessages"></param>
		private async Task ProcessFeed(List<AmazonAvailabilityEntityBucket> buckets, List<InventoryMessage> feedMessages)
		{
			if (!feedMessages.Any())
				return;

			// create and submit feed to export Availabilties
			var feedProcessingResults = await JsonFeedDataProvider.SendFeedAsync<InventoryMessage>(feedMessages);

			Dictionary<Guid, MappedAvailability> bucketDict = buckets.ToDictionary(keySelector: x => x.Product.LocalID.Value, elementSelector: x => x.Product);

			//// process export processing results
			//// create a dictionary that has inventory CD as key and errors (if there's any) as value
			//// a record that has an empty list as value means availabilty  feed was  was processed sucessfully
			var availabilityProcessingResults = new Dictionary<string, List<string>>();
			foreach (JsonFeedProcessingResult<InventoryMessage> result in feedProcessingResults)
			{
				List<InventoryMessage> processedMessages = result.FeedMessages;
				JsonProcessingReport processingReport = result.ProcessingReport;

				//select errors from the processing results
				List<Issues> errorResults = processingReport.Issues.Where(x => x.Severity == Severity.Error).ToList();
				foreach (var message in processedMessages)
				{
					MappedAvailability availability = bucketDict[message.OriginalInventotyNoteId];
					if (!availabilityProcessingResults.ContainsKey(availability.Local.InventoryCD.Value))
						availabilityProcessingResults[availability.Local.InventoryCD.Value] = new List<string>();

					List<string> availabilityErrors = errorResults.Where(x => x.MessageId == message.MessageID).Select(x => x.Message).ToList();
					if (availabilityErrors.Count == 0)
					{
						availabilityErrors = errorResults.Where(x => x.MessageId == null).Select(x => x.Message).ToList();
					}
					availabilityProcessingResults[availability.Local.InventoryCD.Value].AddRange(availabilityErrors);
				}
			}

			bool hasErrors = false;
			foreach (var availability in buckets)
			{
				MappedAvailability obj = availability.Product;

				if (obj.Extern.InventoryMessages == null || obj.Extern.InventoryMessages.Count() == 0) continue;

				string inventoryCD = obj.Local.InventoryCD.Value;
				List<string> availabilityErrors = availabilityProcessingResults[inventoryCD];

				// if the dictionary has no record of the inventory availability then it means, for some reason, the availability has never been sent to Amazon
				// thus, continue without modifying sync status of the availability
				if (availabilityErrors == null) continue;

				obj.AddExtern(obj.Extern, obj.Extern.InventoryMessages.FirstOrDefault()?.SKU, obj.Extern.InventoryMessages.FirstOrDefault()?.SKU, DateTime.Now);

				if (availabilityErrors.Count == 0)
				{
					UpdateStatus(obj, BCSyncOperationAttribute.ExternUpdate);
				}
				else
				{
					Log(obj.SyncID, SyncDirection.Export, new Exception(string.Join(" ", availabilityErrors)));
					UpdateStatus(obj, BCSyncOperationAttribute.ExternFailed,
						PXMessages.LocalizeFormatNoPrefixNLA(AmazonMessages.ProductAvailabilityExternError, inventoryCD, string.Join(" ", availabilityErrors)));
					Operation.Callback?.Invoke(new SyncInfo(obj.SyncID ?? 0, SyncDirection.Export, SyncResult.Error, new Exception(string.Join(" ", availabilityErrors))));
				}
			}
		}

		// TODO: Move it to a separate class
		private List<InventoryMessage> CreateListOfMessages(List<AmazonAvailabilityEntityBucket> buckets)
		{
			List<InventoryMessage> inventoryAvailabilities = new List<InventoryMessage>();
			int messageIdInc = 1;
			foreach (var availabilityBucket in buckets)
			{
				MappedAvailability obj = availabilityBucket.Product;
				var fulfillments = obj.Extern.InventoryMessages;
				// mark availability as invalid when there's nothing in Fulfillments
				if (fulfillments == null || fulfillments?.Count() == 0)
				{
					SynchronizeStatus(obj, BCSyncOperationAttribute.Skipped, BCSyncStatusAttribute.Invalid,
						PXMessages.LocalizeFormatNoPrefixNLA(AmazonMessages.InventoryIsEmpty, obj.Local.InventoryID.Value));
					continue;
				}

				foreach (var fulfillment in fulfillments)
				{
					fulfillment.MessageID = messageIdInc;
					fulfillment.OriginalInventotyNoteId = obj.LocalID.Value;
					inventoryAvailabilities.Add(fulfillment);
					messageIdInc++;
				}
			}

			return inventoryAvailabilities;
		}
		#endregion

		#region Import
		public override Task FetchBucketsImport(CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}
		public override Task<List<AmazonAvailabilityEntityBucket>> GetBucketsImport(List<BCSyncStatus> ids, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}
		public override Task SaveBucketsImport(List<AmazonAvailabilityEntityBucket> buckets, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}
		#endregion

		protected virtual int GetStorageQty(BCBindingExt bindingExt, StorageDetailsResult storageDetailsResult)
		{
			int qty = GetInventoryLevel(bindingExt, storageDetailsResult);
			qty = ReplaceNegativeValueByZero(qty);
			return qty;
		}

		protected virtual int ReplaceNegativeValueByZero(int qty)
		{
			if (qty < 0)
			{
				qty = 0;
			}

			return qty;
		}
	}
}
