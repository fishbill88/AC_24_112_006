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

using PX.Api.ContractBased.Models;
using PX.Commerce.Amazon.API.Rest;
using PX.Commerce.Amazon.API.Rest.Constants;
using PX.Commerce.Core;
using PX.Commerce.Core.API;
using PX.Commerce.Objects;
using PX.Common;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Messages = PX.Objects.AR.Messages;

namespace PX.Commerce.Amazon
{
	public class AmazonInvoiceBucket : EntityBucketBase, IEntityBucket
	{
		public IMappedEntity Primary { get => Invoice; }
		public IMappedEntity[] Entities => new IMappedEntity[] { Invoice };

		public MappedInvoice Invoice;
	}

	public class AmazonInvoiceRestrictor : BCBaseRestrictor, IRestrictor
	{
		public FilterResult RestrictExport(IProcessor processor, IMappedEntity mapped, FilterMode mode)
		{
			return null;
		}

		public FilterResult RestrictImport(IProcessor processor, IMappedEntity mapped, FilterMode mode)
		{
			return base.Restrict<MappedInvoice>(mapped, delegate (MappedInvoice obj)
			{
				BCBindingExt bindingExt = processor.GetBindingExt<BCBindingExt>();

				if (obj.Local is SalesInvoice invoice
									&& invoice.Status.Value.IsIn(PX.Objects.AR.Messages.Open, PX.Objects.AR.Messages.Closed))
				{
					return new FilterResult(FilterStatus.Filtered,
						PXMessages.LocalizeFormatNoPrefixNLA(AmazonMessages.InvoiceStatusDoesNotAllowModification, invoice.ReferenceNbr.Value));
				}

				// skip AFN orders if they are not yet  Shipped
				if (obj.Extern != null && obj.Extern.OrderStatus != OrderStatus.Shipped)
				{
					return new FilterResult(FilterStatus.Invalid,
						PXMessages.LocalizeFormatNoPrefixNLA(AmazonMessages.OrderIsSkippedBecauseItIsNotShipped, obj.Extern.AmazonOrderId, obj.Extern.OrderStatus));
				}

				// skip order that was created before SyncOrdersFrom
				if (obj.Extern != null && bindingExt.SyncOrdersFrom != null && obj.Extern.PurchaseDate.ToDate(timeZone: PXTimeZoneInfo.FindSystemTimeZoneById(bindingExt.OrderTimeZone)) < bindingExt.SyncOrdersFrom)
				{
					return new FilterResult(FilterStatus.Ignore,
						PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogOrderSkippedCreatedBeforeSyncOrdersFrom, obj.Extern.AmazonOrderId, bindingExt.SyncOrdersFrom.Value.Date.ToString("d")));
				}

				return null;
			});
		}
	}

	[Obsolete(PX.Objects.Common.Messages.ClassIsObsolete)]
	[BCProcessor(typeof(BCAmazonConnector), BCEntitiesAttribute.SOInvoice, BCCaptions.Invoice, 220,
		IsInternal = false,
		Direction = SyncDirection.Import,
		PrimaryDirection = SyncDirection.Import,
		PrimarySystem = PrimarySystem.Extern,
		PrimaryGraph = typeof(PX.Objects.SO.SOInvoiceEntry),
		ExternTypes = new Type[] { typeof(Order) },
		LocalTypes = new Type[] { typeof(SalesInvoice) },
		AcumaticaPrimarySelect = typeof(Search<
			PX.Objects.SO.SOInvoice.refNbr,
			Where<PX.Objects.SO.SOInvoice.docType, Equal<ARDocType.invoice>>>),
		URL = "orders-v3/order/{0}"
		)]
	[BCProcessorDetail(EntityType = AmazonCaptions.SalesInvoiceLineEntityType,
		EntityName = AmazonCaptions.SalesInvoiceLineEntity,
		AcumaticaType = typeof(PX.Objects.AR.ARTran))]
	public class AmazonInvoiceProcessor : BCProcessorSingleBase<AmazonInvoiceProcessor, AmazonInvoiceBucket, MappedInvoice>, IProcessor
	{
		private readonly List<string> unmodifiableInvoiceStatuses = new List<string> { PX.Objects.AR.Messages.Open, PX.Objects.AR.Messages.Closed };
		public IOrderDataProvider OrderDataProvider { get; set; }
		private AmazonHelper _helper = PXGraph.CreateInstance<AmazonHelper>();

		public PXSelect<State, Where<State.name, Equal<Required<State.name>>, Or<State.stateID, Equal<Required<State.stateID>>>>> states;

		public override async Task Initialise(IConnector iconnector, ConnectorOperation operation, CancellationToken cancellationToken = default)
		{
			await base.Initialise(iconnector, operation, cancellationToken);
			_helper.Initialize(this);
			var client = ((BCAmazonConnector)iconnector).GetRestClient(GetBindingExt<BCBindingAmazon>(), GetBinding());
			OrderDataProvider = new OrderDataProvider(client);
		}

		#region common

		public override async Task<MappedInvoice> PullEntity(string externID, string jsonObject, CancellationToken cancellationToken = default)
		{
			return null;
		}

		public override async Task<MappedInvoice> PullEntity(Guid? localID, Dictionary<String, Object> fields, CancellationToken cancellationToken = default)
		{
			return null;
		}

		public override async Task<PullSimilarResult<MappedInvoice>> PullSimilar(IExternEntity entity, CancellationToken cancellationToken = default)
		{
			Order order = (Order)entity;
			string uniqueField = order?.AmazonOrderId?.ToString();
			if (string.IsNullOrEmpty(uniqueField))
				return null;
			uniqueField = APIHelper.ReferenceMake(uniqueField, GetBinding().BindingName);
			List<MappedInvoice> result = new List<MappedInvoice>();
			foreach (ARRegister arRegister in PXSelect<ARRegister,
				  Where<ARRegister.docType, Equal<ARDocType.invoice>, And<ARRegister.externalRef, Equal<Required<ARRegister.externalRef>>>>>.Select(this, uniqueField))
			{
				SalesInvoice data = new SalesInvoice() { SyncID = arRegister.NoteID, SyncTime = arRegister.LastModifiedDateTime, ExternalRef = arRegister.ExternalRef?.ValueField() };
				result.Add(new MappedInvoice(data, data.SyncID, data.SyncTime));
			}

			return new PullSimilarResult<MappedInvoice>() { UniqueField = uniqueField, Entities = result };
		}

		public override bool ControlModification(IMappedEntity mapped, BCSyncStatus status, string operation, CancellationToken cancellationToken = default)
		{
			if (mapped is MappedInvoice mappedInvoice)
				return IsInvoiceToBeModified(mappedInvoice);

			return base.ControlModification(mapped, status, operation, cancellationToken);
		}

		private bool IsInvoiceToBeModified(MappedInvoice mappedInvoice)
		{
			if (mappedInvoice.Extern?.OrderStatus == OrderStatus.Shipped
				&& (mappedInvoice?.Local?.Status?.Value == Messages.Open
					|| mappedInvoice?.Local?.Status?.Value == Messages.Closed
					|| mappedInvoice?.Local?.Status?.Value == Messages.Canceled))
				return false;

			return true;
		}

		public override void ControlDirection(AmazonInvoiceBucket bucket, BCSyncStatus status, ref bool shouldImport, ref bool shouldExport, ref bool skipSync, ref bool skipForce)
		{
			MappedInvoice invoice = bucket.Invoice;

			if (invoice != null
				&& (shouldImport || Operation.SyncMethod == SyncMode.Force)
				&& invoice?.IsNew == false && invoice?.ExternID != null && invoice?.LocalID != null
				&& (invoice?.Local?.Status?.Value?.IsIn(unmodifiableInvoiceStatuses) == true))
			{
				var newHash = invoice.Extern.CalculateHash();
				//If externHash is null and Acumatica order existing, that means BCSyncStatus record was deleted and re-created
				if (string.IsNullOrEmpty(status.ExternHash) || newHash == status.ExternHash || !IsInvoiceToBeModified(invoice))
				{
					skipForce = true;
					skipSync = true;
					status.LastOperation = BCSyncOperationAttribute.ExternChangedWithoutUpdateLocal;
					status.LastErrorMessage = null;
					UpdateStatus(invoice, status.LastOperation, status.LastErrorMessage);
					shouldImport = false;
				}
			}
		}

		public override async Task<PXTransactionScope> WithTransaction(Func<Task> action)
		{
			await action();
			return null;
		}
		#endregion

		#region Import
		public override async Task FetchBucketsForImport(DateTime? minDateTime, DateTime? maxDateTime, PXFilterRow[] filters, CancellationToken cancellationToken = default)
		{
			throw new PXException(AmazonMessages.FetchIsNotSupportedForEntity, BCCaptions.Invoice, AmazonCaptions.FBAEntity);
		}

		public override async Task<EntityStatus> GetBucketForImport(AmazonInvoiceBucket bucket, BCSyncStatus syncStatus, CancellationToken cancellationToken = default)
		{
			bool invoiceCannotBeModified = bucket.Invoice?.Local?.Status?.Value.IsIn(unmodifiableInvoiceStatuses) == true;
			// If an invoice cannot be modified,
			// we should not execute OrderDataProvider.GetById, because it is expensive useless in this case.
			// Instead, it creates a new mapped object from the sync status and execute EnsureStatus. 
			// The EnsureStatus triggers the restrictor.
			// The record will not be processed, but filtered.
			if (invoiceCannotBeModified)
			{
				bucket.Invoice = bucket.Invoice.Set(new Order(), syncStatus.ExternID, syncStatus.ExternDescription, syncStatus.ExternTS);
			}
			// if an invoice can be modified or it doesn't exist yet, we should execute OrderDataProvider.GetById to get the latest data from Amazon and sync the record.
			else
			{
				Order data = await OrderDataProvider.GetById(syncStatus.ExternID);
				if (data?.AmazonOrderId == null) return EntityStatus.None;

				bucket.Invoice = bucket.Invoice.Set(data, data.AmazonOrderId?.ToString(), data.AmazonOrderId?.ToString(), data.LastUpdateDate.ToDate());
			}

			EntityStatus status = EnsureStatus(bucket.Invoice, SyncDirection.Import);

			return status;
		}

		public override async Task MapBucketImport(AmazonInvoiceBucket bucket, IMappedEntity existing, CancellationToken cancellationToken = default)
		{
			MappedInvoice obj = bucket.Invoice;
			BCBindingExt bindingExt = GetBindingExt<BCBindingExt>();
			BCBinding currentBinding = GetBinding();
			BCBindingAmazon bCBindingAmazon = GetBindingExt<BCBindingAmazon>();
			Order data = obj.Extern;
			SalesInvoice impl = obj.Local = new SalesInvoice();
			SalesInvoice presented = existing?.Local as SalesInvoice;
			if (presented != null && presented.Status?.Value != PX.Objects.AR.Messages.Balanced && presented.Status?.Value != PX.Objects.AR.Messages.Hold)
			{
				throw new PXException(AmazonMessages.InvoiceStatusDoesNotAllowModification, presented.ReferenceNbr?.Value);
			}
			impl.Type = ARDocType.Invoice.ValueField();
			var description = PXMessages.LocalizeFormat(AmazonMessages.InvoiceDecription, data.AmazonOrderId, currentBinding.BindingName);
			impl.Description = description.ValueField();
			var branch = Branch.PK.Find(this, currentBinding.BranchID);
			impl.ExternalRef = APIHelper.ReferenceMake(data.AmazonOrderId, currentBinding.BindingName).ValueField();
			impl.CustomerOrder = data.AmazonOrderId.ValueField();
			impl.NeedRelease = bCBindingAmazon?.ReleaseInvoices ?? false;

			DateTime? date;
			var purchaseDate = data.PurchaseDate.ToDate(timeZone: PXTimeZoneInfo.FindSystemTimeZoneById(GetBindingExt<Objects.BCBindingExt>().OrderTimeZone));
			var latestShipDate = data.LatestShipDate.ToDate(timeZone: PXTimeZoneInfo.FindSystemTimeZoneById(GetBindingExt<Objects.BCBindingExt>().OrderTimeZone));
			// For FBA , assign LatestShipDate to Date in ERP if it is greater than purchase date else assign purchase date
			if (latestShipDate > purchaseDate)
				date = latestShipDate;
			else
				date = purchaseDate;

			if (date.HasValue)
				impl.Date = (new DateTime(date.Value.Date.Ticks)).ValueField();

			impl.FinancialDetails = new SalesInvoiceFinancialDetails();
			impl.FinancialDetails.Branch = branch?.BranchCD?.Trim().ValueField();

			// CustomerId
			var guestCustomerResult = PXSelectJoin<
				PX.Objects.AR.Customer,
				LeftJoin<PX.Objects.CR.Address,
					On<PX.Objects.AR.Customer.defBillAddressID, Equal<PX.Objects.CR.Address.addressID>>>,
				Where<PX.Objects.AR.Customer.bAccountID, Equal<Required<PX.Objects.AR.Customer.bAccountID>>>>
				.Select(this, bindingExt.GuestCustomerID)
				.Cast<PXResult<PX.Objects.AR.Customer, PX.Objects.CR.Address>>()
				.FirstOrDefault();
			var customer = guestCustomerResult?.GetItem<PX.Objects.AR.Customer>();
			if (customer == null) throw new PXException(AmazonMessages.NoGenericCustomer);
			if (customer.Status != PX.Objects.AR.CustomerStatus.Active) throw new PXException(AmazonMessages.CustomerNotActive, customer.AcctCD);
			impl.CustomerID = customer.AcctCD?.Trim().ValueField();

			// LocationId
			var address = guestCustomerResult?.GetItem<PX.Objects.CR.Address>();

			#region Bill-To Address && Contact
			State state;
			if (data.ShippingAddress != null)
			{
				impl.ShipToAddress = new SalesInvoiceAddress();
				impl.ShipToAddressOverride = true.ValueField();
				impl.ShipToAddress.AddressLine1 = data.ShippingAddress?.AddressLine1?.ValueField();
				impl.ShipToAddress.AddressLine2 = data.ShippingAddress?.AddressLine2?.ValueField();
				impl.ShipToAddress.City = data.ShippingAddress?.City?.ValueField();
				impl.ShipToAddress.Country = data.ShippingAddress?.CountryCode?.ValueField();
				if (!string.IsNullOrEmpty(data.ShippingAddress?.StateOrRegion))
				{
					state = states.Select(data.ShippingAddress?.StateOrRegion, data.ShippingAddress?.StateOrRegion);
					if (state == null)
						impl.ShipToAddress.State = _helper.GetSubstituteLocalByExtern(BCSubstitute.GetValue(Operation.ConnectorType, BCSubstitute.State), data.ShippingAddress?.StateOrRegion, data.ShippingAddress?.StateOrRegion).ValueField();
					else
						impl.ShipToAddress.State = state.StateID?.ValueField();
				}
				else
					impl.ShipToAddress.State = string.Empty.ValueField();
				impl.ShipToAddress.PostalCode = data.ShippingAddress.PostalCode?.ToUpperInvariant()?.ValueField();

				impl.ShipToContact = new SalesInvoiceDocContact();
				impl.ShipToContactOverride = true.ValueField();
				impl.ShipToContact.Phone1 = data.ShippingAddress?.Phone?.ValueField();
				impl.ShipToContact.Email = data.BuyerInfo?.BuyerEmail?.ValueField();
				impl.ShipToContact.Attention = data.ShippingAddress?.Name?.ValueField();

			}
			else
			{
				impl.ShipToAddress = new SalesInvoiceAddress();
				impl.ShipToAddress.AddressLine1 = string.Empty.ValueField();
				impl.ShipToAddress.AddressLine2 = string.Empty.ValueField();
				impl.ShipToAddress.City = string.Empty.ValueField();
				impl.ShipToAddress.State = string.Empty.ValueField();
				impl.ShipToAddress.PostalCode = string.Empty.ValueField();
				impl.ShipToContact = new SalesInvoiceDocContact();
				impl.ShipToContact.Phone1 = string.Empty.ValueField();
				impl.ShipToContact.Email = data.BuyerInfo?.BuyerEmail?.ValueField();
				impl.ShipToContact.Attention = string.Empty.ValueField();
				impl.ShipToContact.BusinessName = string.Empty.ValueField();
				impl.ShipToAddressOverride = true.ValueField();
				impl.ShipToContactOverride = true.ValueField();
				impl.ShipToAddress.Country = (address?.CountryID)?.ValueField();
			}

			impl.BillToContact = new SalesInvoiceDocContact();
			impl.BillToAddress = new SalesInvoiceAddress();
			impl.BillToAddress.AddressLine1 = impl.ShipToAddress.AddressLine1;
			impl.BillToAddress.AddressLine2 = impl.ShipToAddress.AddressLine2;
			impl.BillToAddress.City = impl.ShipToAddress.City;
			impl.BillToAddress.Country = impl.ShipToAddress.Country;
			impl.BillToAddress.State = impl.ShipToAddress.State;
			impl.BillToAddress.PostalCode = impl.ShipToAddress.PostalCode;
			impl.BillToAddressOverride = true.ValueField();
			impl.BillToContact.Phone1 = impl.ShipToContact.Phone1;
			impl.BillToContact.Email = impl.ShipToContact.Email;
			impl.BillToContact.BusinessName = impl.ShipToContact.BusinessName;
			impl.BillToContact.Attention = impl.ShipToContact.Attention;
			impl.BillToContactOverride = true.ValueField();
			#endregion

			//#region Products
			impl.Details = new List<SalesInvoiceDetail>();
			List<BCLocations> locationMappings = new List<BCLocations>();
			decimal? freight = 0, taxableAmount = 0, taxAmount = 0, giftwrappingPrice = 0;
			impl.Currency = data.OrderTotal?.CurrencyCode?.ValueField();
			decimal? shippingDiscount = 0, promotionalDiscount = 0;
			foreach (var orderItem in data.OrderItems)
			{
				decimal? quantity = orderItem.QuantityOrdered ?? 0;
				decimal? subTotal = orderItem.ItemPrice?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0;
				SalesInvoiceDetail detail = new SalesInvoiceDetail();
				detail.DiscountAmount = 0m.ValueField();
				string inventoryCD = _helper.GetInventoryCDByExternID(orderItem.SellerSKU, orderItem.ASIN, AmazonMessages.ItemNotFoundInvoice, out string uom);
				detail.InventoryID = inventoryCD.ValueField();
				detail.Qty = quantity.ValueField();
				detail.UOM = uom.ValueField();
				detail.UnitPrice = (quantity > 0 ? (subTotal / quantity) : 0).ValueField();
				detail.TransactionDescr = orderItem.Title.ValueField();
				detail.ExternalRef = orderItem.OrderItemId.ToString().ValueField();
				detail.BranchID = impl.FinancialDetails.Branch;
				detail.ManualPrice = true.ValueField();
				// warehouse mapping
				if (bCBindingAmazon.Warehouse != null)
				{
					INSite defaultFbaWarehouse = PXSelect<INSite, Where<INSite.siteID, Equal<Required<INSite.siteID>>>>.Select(this, bCBindingAmazon.Warehouse);

					if (defaultFbaWarehouse != null)
						detail.WarehouseID = defaultFbaWarehouse.SiteCD.ValueField();
				}

				if (bCBindingAmazon.LocationID != null)
				{
					INLocation fbaLocation = PXSelect<INLocation, Where<INLocation.locationID, Equal<Required<INLocation.locationID>>>>.Select(this, bCBindingAmazon.LocationID);

					if (fbaLocation != null)
						detail.Location = fbaLocation.LocationCD.ValueField();
				}

				var itemfreight = orderItem.ShippingPrice?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0;
				var itemGiftWrappingPrice = orderItem.BuyerInfo?.GiftWrapPrice?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0;
				var itemShippingDiscount = orderItem.ShippingDiscount?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0;
				var itemPromotionalDiscount = orderItem.PromotionDiscount?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0;
				freight += itemfreight;
				giftwrappingPrice += itemGiftWrappingPrice;
				shippingDiscount += itemShippingDiscount;
				promotionalDiscount += itemPromotionalDiscount;

				taxableAmount += ((orderItem.ItemPrice?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0 + itemfreight + itemGiftWrappingPrice)
					 - (itemPromotionalDiscount + itemShippingDiscount));
				taxAmount += (orderItem.ItemTax?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0) + (orderItem.ShippingTax?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0)
					+ (orderItem.ShippingDiscountTax?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0) + (orderItem.PromotionDiscountTax?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0)
					+ (orderItem.BuyerInfo?.GiftWrapTax?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0);

				//Check for existing				
				DetailInfo matchedDetail = existing?.Details?.FirstOrDefault(d => d.EntityType == AmazonCaptions.SalesInvoiceLineEntityType && orderItem.OrderItemId.ToString() == d.ExternID);
				if (matchedDetail != null) detail.Id = matchedDetail.LocalID; //Search by Details
				else if (presented?.Details != null && presented.Details.Count > 0) //Serach by Existing line
				{
					SalesInvoiceDetail matchedLine = presented.Details.FirstOrDefault(x =>
						(x.ExternalRef?.Value != null && x.ExternalRef?.Value == orderItem.OrderItemId.ToString())
						||
						(x.InventoryID?.Value?.Trim() == detail.InventoryID?.Value?.Trim() && (detail.UOM == null || detail.UOM.Value == x.UOM?.Value)));
					if (matchedLine != null && !impl.Details.Any(i => i.Id == matchedLine.Id)) detail.Id = matchedLine.Id;
				}

				impl.Details.Add(detail);
			}

			//Gift wrapping Line
			if (giftwrappingPrice > 0)
				AddGiftWrapLine(impl, presented, giftwrappingPrice);
			//Gift Freight Line
			if (freight > 0)
				AddFreightLine(impl, presented, customer, freight);

			#region Discounts
			impl.DiscountDetails = new List<SalesInvoiceDiscountDetails>();

			SalesInvoiceDiscountDetails discountDetail = new SalesInvoiceDiscountDetails();
			if (shippingDiscount > 0)
			{
				discountDetail.Type = PX.Objects.Common.Discount.DiscountType.ExternalDocument.ValueField();
				discountDetail.Description = AmazonMessages.ShippingDiscount.ValueField();
				discountDetail.ExternalDiscountCode = AmazonMessages.PromotionalDiscount.ValueField();
				discountDetail.DiscountAmount = shippingDiscount.ValueField();
				impl.DiscountDetails.Add(discountDetail);
			}

			if (promotionalDiscount > 0)
			{
				discountDetail = new SalesInvoiceDiscountDetails();
				discountDetail.Type = PX.Objects.Common.Discount.DiscountType.ExternalDocument.ValueField();
				discountDetail.Description = AmazonMessages.ShippingDiscount.ValueField();
				discountDetail.ExternalDiscountCode = AmazonMessages.PromotionalDiscount.ValueField();
				discountDetail.DiscountAmount = promotionalDiscount.ValueField();
				impl.DiscountDetails.Add(discountDetail);
			}

			#endregion

			#region Taxes

			impl.TaxDetails = new List<SalesInvoiceTaxDetail>();
			if (bindingExt.TaxSynchronization == true)
			{
				impl.IsTaxvalid = true.ValueField();
				string taxName = bCBindingAmazon.DefaultTaxID;
				if (string.IsNullOrEmpty(taxName)) throw new PXException(AmazonMessages.NoDefaultTaxID);
				impl.TaxDetails.Add(new SalesInvoiceTaxDetail()
				{
					TaxID = taxName.ValueField(),
					TaxAmount = taxAmount.ValueField(),
					TaxableAmount = taxableAmount.ValueField()
				});
				impl.FinancialDetails.CustomerTaxZone = bindingExt.DefaultTaxZoneID.ValueField();
			}
			//Set tax calculation to default mode
			impl.TaxCalcMode = PX.Objects.TX.TaxCalculationMode.TaxSetting.ValueField();
			#endregion

			#region Adjust for Existing
			if (presented != null)
			{
				obj.Local.Type = presented.Type; //Keep the same order Type

				//remap entities if existing
				presented.DiscountDetails?.ForEach(e => obj.Local.DiscountDetails?.FirstOrDefault(n => n.ExternalDiscountCode.Value == e.ExternalDiscountCode.Value).With(n => n.Id = e.Id));
				//delete unnecessary entities
				obj.Local.Details?.AddRange(presented.Details == null ? Enumerable.Empty<SalesInvoiceDetail>()
					: presented.Details.Where(e => obj.Local.Details == null || !obj.Local.Details.Any(n => e.Id == n.Id)).Select(n => new SalesInvoiceDetail() { Id = n.Id, Delete = true }));
				obj.Local.DiscountDetails?.AddRange(presented.DiscountDetails == null ? Enumerable.Empty<SalesInvoiceDiscountDetails>()
					: presented.DiscountDetails.Where(e => obj.Local.DiscountDetails == null || !obj.Local.DiscountDetails.Any(n => e.Id == n.Id)).Select(n => new SalesInvoiceDiscountDetails() { Id = n.Id, Delete = true }));
			}
			#endregion
		}

		public virtual void AddGiftWrapLine(SalesInvoice impl, SalesInvoice presented, decimal? amount)
		{
			var currentBindingExt = GetBindingExt<BCBindingExt>();
			InventoryItem inventory = currentBindingExt?.GiftWrappingItemID != null ? PX.Objects.IN.InventoryItem.PK.Find(this, currentBindingExt?.GiftWrappingItemID) : null;
			if (inventory?.InventoryCD == null)
				throw new PXException(AmazonMessages.NoGiftWrapItem);
			var uom = inventory?.SalesUnit?.Trim();

			SalesInvoiceDetail detail = new SalesInvoiceDetail();
			detail.BranchID = impl.FinancialDetails.Branch;
			string inventoryCD = inventory?.InventoryCD?.Trim();
			detail.InventoryID = inventoryCD?.TrimEnd().ValueField();
			detail.Qty = 1m.ValueField();
			detail.UOM = uom.ValueField();
			detail.UnitPrice = amount.ValueField();
			detail.ManualPrice = true.ValueField();
			if (presented != null && presented.Details?.Count > 0)
			{
				presented.Details.FirstOrDefault(x => x.InventoryID.Value == detail.InventoryID.Value).With(e => detail.Id = e.Id);

			}
			impl.Details.Add(detail);
		}
		public virtual void AddFreightLine(SalesInvoice impl, SalesInvoice presented, PX.Objects.CR.BAccount customer, decimal? amount)
		{
			Location location = null;
			var bindingAmazon = GetBindingExt<BCBindingAmazon>();
			if (bindingAmazon.ShippingAccount == null || bindingAmazon.ShippingSubAccount == null)
			{
				location = PXSelect<Location, Where<Location.bAccountID, Equal<Required<Location.bAccountID>>,
				And<Location.locationID, Equal<Required<Location.locationID>>>>>.Select(this, customer.BAccountID, customer.DefLocationID);
			}

			int? acctID = bindingAmazon.ShippingAccount ?? location?.CFreightAcctID;
			int? subAcct = bindingAmazon.ShippingSubAccount ?? location?.CFreightSubID;

			if (acctID is null || subAcct is null) throw new PXException(AmazonMessages.ShippingAccountOrSubMissing);

			Account account = acctID != null ? Account.PK.Find(this, acctID) : null;
			Sub subAccount = subAcct != null ? Sub.PK.Find(this, subAcct) : null;

			SalesInvoiceDetail detail = new SalesInvoiceDetail();
			detail.BranchID = impl.FinancialDetails.Branch;
			detail.Qty = 1m.ValueField();
			detail.UnitPrice = amount.ValueField();
			detail.TransactionDescr = AmazonCaptions.ShippingPrice.ValueField();
			detail.Account = account.AccountCD.ValueField();
			detail.SubAccount = subAccount.SubCD.ValueField();
			detail.ManualPrice = true.ValueField();
			if (presented != null && presented.Details?.Count > 0)
			{
				presented.Details.FirstOrDefault(x => string.IsNullOrEmpty(x.InventoryID?.Value) && !string.IsNullOrEmpty(x.Account?.Value) && !string.IsNullOrEmpty(x.SubAccount?.Value)).With(e => detail.Id = e.Id);

			}
			impl.Details.Add(detail);
		}

		public override async Task SaveBucketImport(AmazonInvoiceBucket bucket, IMappedEntity existing, string operation, CancellationToken cancellationToken = default)
		{
			MappedInvoice obj = bucket.Invoice;
			SalesInvoice local = obj.Local;
			SalesInvoice presented = existing?.Local as SalesInvoice;
			BCBindingExt bindingExt = GetBindingExt<BCBindingExt>();
			Boolean needRelease = obj.Local.NeedRelease;

			SalesInvoice impl = null;
			//sort solines by deleted =true first because of api bug  in case if lines are deleted
			obj.Local.Details = obj.Local.Details.OrderByDescending(o => o.Delete).ToList();
			obj.Local.DiscountDetails = obj.Local.DiscountDetails.OrderByDescending(o => o.Delete).ToList();

			#region Taxes
			_helper.LogTaxDetails(obj.SyncID, obj.Local);
			#endregion

			using (var transaction = await WithTransaction(async () =>
			{
				impl = cbapi.Put<SalesInvoice>(obj.Local, obj.LocalID);
			}))
			{
				transaction?.Complete();
			}

			if (needRelease && impl.Status?.Value == PX.Objects.AR.Messages.Balanced)
			{
				using (var transaction = await WithTransaction(async () =>
				{
					impl = cbapi.Invoke<SalesInvoice, ReleaseSalesInvoice>(null, impl.Id);
				}))
				{
					transaction?.Complete();
				}
			}



			#region Taxes
			await _helper.ValidateTaxes(obj.SyncID, impl, obj.Local);
			#endregion

			obj.ExternHash = obj.Extern.CalculateHash();
			obj.AddLocal(impl, impl.SyncID, impl.SyncTime);

			// Save Details
			DetailInfo[] oldDetails = obj.Details.ToArray();
			obj.ClearDetails();

			foreach (OrderItem orderItem in obj.Extern.OrderItems) //Line ID detail
			{
				SalesInvoiceDetail detail = null;
				detail = impl.Details.FirstOrDefault(x => x.NoteID.Value == oldDetails.FirstOrDefault(o => o.ExternID == orderItem.OrderItemId.ToString())?.LocalID);
				if (detail == null) detail = impl.Details.FirstOrDefault(x => x.ExternalRef?.Value != null && x.ExternalRef?.Value == orderItem.OrderItemId.ToString());
				if (detail == null)
				{
					String inventoryCD = _helper.GetInventoryCDByExternID(orderItem.SellerSKU, orderItem.ASIN, AmazonMessages.ItemNotFoundInvoice, out string uom);
					detail = impl.Details.FirstOrDefault(x => !obj.Details.Any(o => x.NoteID.Value == o.LocalID) && x.InventoryID.Value == inventoryCD);
				}
				if (detail != null)
				{
					obj.AddDetail(AmazonCaptions.SalesInvoiceLineEntityType, detail.NoteID.Value, orderItem.OrderItemId.ToString());
					continue;
				}
				throw new PXException(BCMessages.CannotMapLines);
			}

			UpdateStatus(obj, operation);
		}

		#endregion

		#region Export
		public override async Task FetchBucketsForExport(DateTime? minDateTime, DateTime? maxDateTime, PXFilterRow[] filters, CancellationToken cancellationToken = default)
		{
			var bindingExt = GetBindingExt<BCBindingExt>();
			var amazonbindingExt = GetBindingExt<BCBindingAmazon>();
			var minDate = minDateTime == null || (minDateTime != null && bindingExt.SyncOrdersFrom != null && minDateTime < bindingExt.SyncOrdersFrom) ? bindingExt.SyncOrdersFrom : minDateTime;
			var impls = cbapi.GetAll<SalesInvoice>(
					new SalesInvoice()
					{
						Type = ARDocType.Invoice.SearchField(),
						ReferenceNbr = new StringReturn(),
						Status = new StringReturn(),
						CustomerID = new StringReturn(),
						Details = new List<SalesInvoiceDetail>() { new SalesInvoiceDetail() {
							ReturnBehavior = ReturnBehavior.OnlySpecified,
							InventoryID = new StringReturn() } }
					},
					minDate, maxDateTime, filters).ToArray();

			if (impls != null && impls.Count() > 0)
			{
				int countNum = 0;
				List<IMappedEntity> mappedList = new List<IMappedEntity>();
				foreach (SalesInvoice impl in impls)
				{
					MappedInvoice obj = new MappedInvoice(impl, impl.SyncID, impl.SyncTime);

					mappedList.Add(obj);
					countNum++;
					if (countNum % BatchFetchCount == 0 || countNum == impls.Count())
					{
						ProcessMappedListForExport(mappedList);
					}
				}
			}
		}

		public override async Task<EntityStatus> GetBucketForExport(AmazonInvoiceBucket bucket, BCSyncStatus syncstatus, CancellationToken cancellationToken = default)
		{
			SalesInvoice impl = cbapi.GetByID<SalesInvoice>(syncstatus.LocalID);
			if (impl == null) return EntityStatus.None;

			MappedInvoice obj = bucket.Invoice = bucket.Invoice.Set(impl, impl.SyncID, impl.SyncTime);
			EntityStatus status = EnsureStatus(bucket.Invoice, SyncDirection.Export);

			return status;
		}

		public override async Task SaveBucketExport(AmazonInvoiceBucket bucket, IMappedEntity existing, string operation, CancellationToken cancellationToken = default)
		{
		}

		#endregion
	}
}
