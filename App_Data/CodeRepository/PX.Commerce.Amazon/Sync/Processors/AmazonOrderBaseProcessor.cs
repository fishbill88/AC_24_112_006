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

using PX.Commerce.Amazon.API.Rest;
using PX.Commerce.Core;
using PX.Commerce.Objects;
using PX.Data;
using PX.Objects.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PX.Objects.SO;
using PX.Commerce.Core.API;
using PX.Api.ContractBased.Models;
using System.Globalization;
using PX.Common;
using PX.Commerce.Amazon.API;
using PX.Commerce.Amazon.API.Rest.Constants;
using PX.Commerce.Amazon.Domain.Orders;

namespace PX.Commerce.Amazon
{
	public class AmazonOrderBucketBase<TMapped> : EntityBucketBase, IEntityBucket
		where TMapped : BCMappedOrderEntity
	{
		public IMappedEntity Primary { get => Order; }
		public IMappedEntity[] Entities => new IMappedEntity[] { Order };

		public TMapped Order;
	}

	// Restrictions relevant for both FBA and FBM
	public class AmazonAmazonOrderBaseProcessorRestrictor : BCBaseRestrictor, IRestrictor
	{
		public virtual FilterResult RestrictExport(IProcessor processor, IMappedEntity mapped, FilterMode mode)
		{
			return null;
		}

		public virtual FilterResult RestrictImport(IProcessor processor, IMappedEntity mapped, FilterMode mode)
		{
			return base.Restrict<BCMappedOrderEntity>(mapped, delegate (BCMappedOrderEntity obj)
			{
				BCBindingExt bindingExt = processor.GetBindingExt<BCBindingExt>();

				// skip order that was created before SyncOrdersFrom
				if (obj.Extern != null && bindingExt.SyncOrdersFrom != null && obj.Extern.PurchaseDate.ToDate(timeZone: PXTimeZoneInfo.FindSystemTimeZoneById(bindingExt.OrderTimeZone)) < bindingExt.SyncOrdersFrom)
				{
					return new FilterResult(FilterStatus.Ignore,
						PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogOrderSkippedCreatedBeforeSyncOrdersFrom, obj.Extern.AmazonOrderId, bindingExt.SyncOrdersFrom.Value.Date.ToString("d")));
				}

				if (obj.Local is SalesOrder salesOrder &&
					salesOrder.Status.Value.IsIn(PX.Objects.SO.Messages.Completed,
						PX.Objects.SO.Messages.Cancelled,
						PX.Objects.SO.Messages.BackOrder,
						PX.Objects.SO.Messages.Shipping,
						PX.Objects.SO.Messages.Invoiced))
				{
					return new FilterResult(FilterStatus.Filtered,
						PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.OrderStatusDoesNotAllowModification, salesOrder.OrderNbr.Value));
				}

				if (obj.Extern.OrderStatus == OrderStatus.Canceled)
				{
					return new FilterResult(FilterStatus.Filtered,
					PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogOrderSkippedExternStatusNotSupported, obj.Extern.AmazonOrderId, obj.Extern.OrderStatus));
				}

				return null;
			});
		}
	}

	public abstract class AmazonOrderBaseProcessor<TPrimaryMapped> : BCProcessorSingleBase<PXGraph, AmazonOrderBucketBase<TPrimaryMapped>, TPrimaryMapped>, IProcessor
		where TPrimaryMapped : BCMappedOrderEntity, new()
	{
		private readonly List<string> unmodifiableOrderStatuses = new List<string>
		{
			PX.Objects.SO.Messages.Completed,
			PX.Objects.SO.Messages.Cancelled,
			PX.Objects.SO.Messages.BackOrder,
			PX.Objects.SO.Messages.Shipping,
			PX.Objects.SO.Messages.Invoiced
		};
		public IOrderDataProvider OrderDataProvider { get; set; }
		public AmazonHelper helper = PXGraph.CreateInstance<AmazonHelper>();

		public PXSelect<State, Where<State.name, Equal<Required<State.name>>, Or<State.stateID, Equal<Required<State.stateID>>>>> states;

		public override async Task Initialise(IConnector iconnector, ConnectorOperation operation, CancellationToken cancellationToken = default)
		{
			await base.Initialise(iconnector, operation, cancellationToken);
			helper.Initialize(this);
			var client = ((BCAmazonConnector)iconnector).GetRestClient(GetBindingExt<BCBindingAmazon>(), GetBinding());
			OrderDataProvider = new OrderDataProvider(client);
		}

		#region Common
		public override async Task<TPrimaryMapped> PullEntity(string externID, string jsonObject, CancellationToken cancellationToken = default)
		{
			return null;
		}

		public override async Task<TPrimaryMapped> PullEntity(Guid? localID, Dictionary<String, Object> fields, CancellationToken cancellationToken = default)
		{
			return null;
		}

		public override async Task<PullSimilarResult<TPrimaryMapped>> PullSimilar(IExternEntity entity, CancellationToken cancellationToken = default)
		{
			Order order = (Order)entity;
			string uniqueField = order?.AmazonOrderId?.ToString();
			if (string.IsNullOrEmpty(uniqueField))
				return null;
			uniqueField = APIHelper.ReferenceMake(uniqueField, GetBinding().BindingName);

			List<TPrimaryMapped> result = new List<TPrimaryMapped>();
			foreach (SOOrder item in helper.OrderByTypesAndCustomerRefNbr.Select(GetSOTypeList(GetBindingExt<BCBindingAmazon>()), uniqueField))
			{
				SalesOrder data = new SalesOrder() { SyncID = item.NoteID, SyncTime = item.LastModifiedDateTime, ExternalRef = item.CustomerRefNbr?.ValueField() };
				result.Add((TPrimaryMapped)CreateMappedOrder(data));
			}

			return new PullSimilarResult<TPrimaryMapped>() { UniqueField = uniqueField, Entities = result };
		}

		public override void ControlDirection(AmazonOrderBucketBase<TPrimaryMapped> bucket, BCSyncStatus status, ref bool shouldImport, ref bool shouldExport, ref bool skipSync, ref bool skipForce)
		{
			TPrimaryMapped order = bucket.Order;

			if (order != null
				&& (shouldImport || Operation.SyncMethod == SyncMode.Force)
				&& order?.IsNew == false && order?.ExternID != null && order?.LocalID != null
				&& (order?.Local?.Status?.Value?.IsIn(unmodifiableOrderStatuses) == true))
			{
				var newHash = order.Extern.CalculateHash();
				//If externHash is null and Acumatica order existing, that means BCSyncStatus record was deleted and re-created
				if (string.IsNullOrEmpty(status.ExternHash) || newHash == status.ExternHash)
				{
					skipForce = true;
					skipSync = true;
					status.LastOperation = BCSyncOperationAttribute.ExternChangedWithoutUpdateLocal;
					status.LastErrorMessage = null;
					UpdateStatus(order, status.LastOperation, status.LastErrorMessage);
					shouldImport = false;
				}
			}
		}
		#endregion

		#region Import

		protected virtual FilterOrders CreateFilter(DateTime? minDateTime, DateTime? maxDateTime)
		{
			FilterOrders filter = new FilterOrders() { MarketplaceIds = new List<string>() { GetBindingExt<BCBindingAmazon>().Marketplace } };
			helper.SetFilterDate(filter, minDateTime, maxDateTime, GetBindingExt<BCBindingExt>().SyncOrdersFrom);

			return filter;
		}

		public override async Task FetchBucketsForImport(DateTime? minDateTime, DateTime? maxDateTime, PXFilterRow[] filters, CancellationToken cancellationToken = default)
			=> await FetchBucketsForImport(CreateFilter(minDateTime, maxDateTime), cancellationToken);

		public virtual async Task FetchBucketsForImport(FilterOrders filter, CancellationToken cancellationToken)
		{
			int countNum = 0;
			List<IMappedEntity> mappedList = new List<IMappedEntity>();
			try
			{
				// It needs to use Task.Yield to avoid situations when async code runs synchroniously. This instruction forces the code to be asynchronous
				await Task.Yield();
				await foreach (Order order in OrderDataProvider.GetAll(filter, cancellationToken))
				{
					mappedList.Add(CreateMappedOrder(order));
					countNum++;
					if (countNum % BatchFetchCount == 0)
					{
						ProcessMappedListForImport(mappedList, true);
					}
				}
			}
			finally
			{
				if (mappedList.Any())
				{
					ProcessMappedListForImport(mappedList, true);
				}
			}
		}

		public override async Task<EntityStatus> GetBucketForImport(AmazonOrderBucketBase<TPrimaryMapped> bucket, BCSyncStatus syncStatus, CancellationToken cancellationToken = default)
		{
			bool localOrderCannotBeModified = bucket.Order?.Local?.Status?.Value.IsIn(unmodifiableOrderStatuses) == true;

			// If an order cannot be modified,
			// we should not execute OrderDataProvider.GetById, because it is expensive useless in this case.
			// Instead, it creates a new mapped object from the sync status and execute EnsureStatus. 
			// The EnsureStatus triggers the restrictor.
			// The record will not be processed, but filtered.
			if (localOrderCannotBeModified)
			{
				bucket.Order = bucket.Order.Set(new Order(), syncStatus.ExternID, syncStatus.ExternDescription, syncStatus.ExternTS);
			}
			// if an order can be modified or it doesn't exist yet, we should execute OrderDataProvider.GetById to get the latest data from Amazon and sync the record.
			else
			{
				Order data = await OrderDataProvider.GetById(syncStatus.ExternID);
				if (data?.AmazonOrderId == null) return EntityStatus.None;

				bucket.Order = bucket.Order.Set(data, data.AmazonOrderId?.ToString(), data.AmazonOrderId?.ToString(), data.LastUpdateDate.ToDate());
			}
			EntityStatus status = EnsureStatus(bucket.Order, SyncDirection.Import);

			return status;
		}

		public override async Task MapBucketImport(AmazonOrderBucketBase<TPrimaryMapped> bucket, IMappedEntity existing, CancellationToken cancellationToken = default)
		{
			BCBindingExt bindingExt = GetBindingExt<BCBindingExt>();
			BCBinding currentBinding = GetBinding();
			BCBindingAmazon bCBindingAmazon = GetBindingExt<BCBindingAmazon>();
			Order data = bucket.Order.Extern;
			SalesOrder existingSalesOrder = existing?.Local as SalesOrder;
			SalesOrderTotals salesOrderTotals = new(data.OrderItems);

			if (existingSalesOrder != null && existingSalesOrder.Status?.Value != PX.Objects.SO.Messages.Open && existingSalesOrder.Status?.Value != PX.Objects.SO.Messages.Hold
				&& existingSalesOrder.Status?.Value != BCObjectsMessages.RiskHold && existingSalesOrder.Status?.Value != PX.Objects.SO.Messages.CreditHold)
			{
				throw new PXException(BCMessages.OrderStatusDoesNotAllowModification, existingSalesOrder.OrderNbr?.Value);
			}

			BCShippingMappings shippingMapping = TryGetShippingMapping(currentBinding.BindingID, data.ShipmentServiceLevelCategory);

			string stateSubstitution = helper.GetSubstituteLocalByExtern(BCSubstitute.GetValue(Operation.ConnectorType, BCSubstitute.State), data.ShippingAddress?.StateOrRegion, data.ShippingAddress?.StateOrRegion);
			(string CustomerCD, string CountryID) guestCustomerInfo = TryGetGuestCustomerInfo(bindingExt.GuestCustomerID);

			if (salesOrderTotals.GiftPrice > 0 && bindingExt.GiftWrappingItemID is null)
			{
				throw new PXException(AmazonMessages.GiftWrappingItemIsNotSpecified);
			}

			(string InventoryCD, string UOM) giftWrapInventoryInfo = helper.GetInventoryCDForGiftWrap(bindingExt.GiftWrappingItemID);
			Dictionary<string, (string inventoryCD, string uom)> inventoryInfoForDetails = TryGetInventoryInfo(helper, data.OrderItems);

			State state = states.Select(data.ShippingAddress?.StateOrRegion, data.ShippingAddress?.StateOrRegion);
			PX.Objects.GL.Branch branch = PX.Objects.GL.Branch.PK.Find(this, currentBinding.BranchID);
			var orderDescription = PXMessages.LocalizeFormat(AmazonMessages.OrderDescription, data.AmazonOrderId, currentBinding.BindingName);
			var orderLatestShipDate = data.LatestShipDate.ToDate(timeZone: PXTimeZoneInfo.FindSystemTimeZoneById(GetBindingExt<Objects.BCBindingExt>().OrderTimeZone));
			string orderExternalRef = APIHelper.ReferenceMake(data.AmazonOrderId, currentBinding.BindingName);

			SalesOrder impl =
				new SalesOrderBuilder()
					.WithSummary()
						.WithType(GetOrderType(bCBindingAmazon).Value)
						.WithCustomer(guestCustomerInfo.CustomerCD)
						.WithCurrency(data.OrderTotal?.CurrencyCode)
						.WithDate(GetOrderDate(data))
						.WithDescription(orderDescription)
						.WithExternalReferences(orderExternalRef, currentBinding.BindingName)
						.WithBranch(branch.BranchCD)
					.WithDetails()
						.WithDetails(data.OrderItems, existing, inventoryInfoForDetails, this.MarketplaceWarehouseLocation)
						.WithGiftWrapDetail(giftWrapInventoryInfo.InventoryCD, giftWrapInventoryInfo.UOM, salesOrderTotals.GiftPrice)
					.WithTaxes()
						.WithTaxes(salesOrderTotals.TaxAmount, salesOrderTotals.TaxableAmount, bindingExt.DefaultTaxZoneID, bCBindingAmazon.DefaultTaxID, bindingExt.TaxSynchronization)
					.WithDiscounts()
						.WithDisabledAutomaticDiscountUpdate()
						.WithShippingDiscount(salesOrderTotals.ShippingDiscount, bindingExt?.PostDiscounts)
						.WithPromotionalDiscount(salesOrderTotals.PromotionalDiscount, bindingExt?.PostDiscounts)
					.WithShippingSettings()
						.WithShippingSettings(shippingMapping)
						.WithCancelByDate(orderLatestShipDate)
					.WithShipToInfo()
						.WithShipToAddressAndContact(data.ShippingAddress, data.BuyerInfo, guestCustomerInfo.CountryID)
						.WithStateSubstitution(state, stateSubstitution)
					.WithBillToInfo()
						.WithSameBillToAdressAndContactFromShipTo()
					.WithOptionalInfo()
						.WithAdjustmentsForExistingOrder(existingSalesOrder)
					.Build();

			MapFreight(impl, salesOrderTotals.Freight, bCBindingAmazon);

			bucket.Order.Local = impl;
		}

		protected virtual Dictionary<string, (string inventoryCD, string uom)> TryGetInventoryInfo(AmazonHelper helper, List<OrderItem> orderItems)
		{
			Dictionary<string, (string inventoryCD, string uom)> result = new();
			foreach (var orderItem in orderItems)
			{
				string inventoryCD = helper.GetInventoryCDByExternID(orderItem.SellerSKU, orderItem.ASIN, AmazonMessages.ItemNotFound, out string uom);
				result.Add(orderItem.OrderItemId, (inventoryCD, uom));
			}
			return result;
		}

		protected virtual BCShippingMappings TryGetShippingMapping(int? bindingID, string shipmentServiceLevelCategory)
		{
			PXCache cache = base.Caches[typeof(BCShippingMappings)];

			//WHY DO WE NEED THIS CHECK, WHAT WILL HAPPEN IF IS IT NOT NULL ??????
			if (!string.IsNullOrEmpty(shipmentServiceLevelCategory))
			{
				String shippingMethod = shipmentServiceLevelCategory;
				BCShippingMappings mappingValue = helper.ShippingMethods().FirstOrDefault(x => x.BindingID == bindingID
					&& string.Equals(x.ShippingMethod, shippingMethod, StringComparison.OrdinalIgnoreCase));

				bool hasMappingError = false;
				if (mappingValue != null)
				{
					if (mappingValue.Active == true && mappingValue.CarrierID == null)
					{
						hasMappingError = true;
					}
					else if (mappingValue.Active == true && mappingValue.CarrierID != null)
					{
						return mappingValue;
					}
				}
				else
				{
					hasMappingError = true;
					BCShippingMappings inserted = new BCShippingMappings() { BindingID = Operation.Binding, ShippingZone = string.Empty, ShippingMethod = shippingMethod, Active = true };
					cache.Insert(inserted);
				}

				if (cache.Inserted.Count() > 0)
					cache.Persist(PXDBOperation.Insert);
				if (hasMappingError)
				{
					throw new PXException(AmazonMessages.OrderShippingMappingIsMissing, shippingMethod);
				}
			}
			return new BCShippingMappings();
		}

		protected virtual (string CustomerCD, string CountryID) TryGetGuestCustomerInfo(int? guestCustomerID)
		{
			var guestCustomerResult = PXSelectJoin<
				PX.Objects.AR.Customer,
				LeftJoin<PX.Objects.CR.Address,
					On<PX.Objects.AR.Customer.defBillAddressID, Equal<PX.Objects.CR.Address.addressID>>>,
				Where<PX.Objects.AR.Customer.bAccountID, Equal<Required<PX.Objects.AR.Customer.bAccountID>>>>
				.Select(this, guestCustomerID)
				.Cast<PXResult<PX.Objects.AR.Customer, PX.Objects.CR.Address>>()
				.FirstOrDefault();
			var customer = guestCustomerResult?.GetItem<PX.Objects.AR.Customer>();
			if (customer == null) throw new PXException(AmazonMessages.NoGenericCustomer);
			if (customer.Status != PX.Objects.AR.CustomerStatus.Active) throw new PXException(AmazonMessages.CustomerNotActive, customer.AcctCD);

			string guestCustomerCD = customer.AcctCD?.Trim();
			string guestCustomerCountryID = guestCustomerResult?.GetItem<PX.Objects.CR.Address>().CountryID;
			return (guestCustomerCD, guestCustomerCountryID);
		}

		public override async Task SaveBucketImport(AmazonOrderBucketBase<TPrimaryMapped> bucket, IMappedEntity existing, string operation, CancellationToken cancellationToken = default)
		{
			TPrimaryMapped mappedObject = bucket.Order;

			// If custom mapped orderType, this will prevent attempt to modify existing SO type and following error
			if (existing != null)
				mappedObject.Local.OrderType = ((TPrimaryMapped)existing).Local.OrderType;

			SalesOrder impl;
			//sort solines by deleted =true first because of api bug  in case if lines are deleted
			mappedObject.Local.Details = mappedObject.Local.Details.OrderByDescending(o => o.Delete).ToList();
			mappedObject.Local.DiscountDetails = mappedObject.Local.DiscountDetails.OrderByDescending(o => o.Delete).ToList();

			#region Taxes
			helper.LogTaxDetails(mappedObject.SyncID, mappedObject.Local);
			#endregion

			impl = cbapi.Put<SalesOrder>(mappedObject.Local, mappedObject.LocalID);

			#region Taxes
			helper.ValidateTaxes(mappedObject.SyncID, impl, mappedObject.Local);
			#endregion

			//If we need to cancel the order in Acumatica
			if (mappedObject.Extern?.OrderStatus == OrderStatus.Canceled)
			{
				impl = cbapi.Invoke<SalesOrder, CancelSalesOrder>(null, impl.SyncID);
			}

			mappedObject.ExternHash = mappedObject.Extern.CalculateHash();
			mappedObject.AddLocal(impl, impl.SyncID, impl.SyncTime);

			// Save Details
			DetailInfo[] oldDetails = mappedObject.Details.ToArray();
			mappedObject.ClearDetails();

			foreach (OrderItem orderItem in mappedObject.Extern.OrderItems) //Line ID detail
			{
				SalesOrderDetail detail = null;
				detail = impl.Details.FirstOrDefault(x => x.NoteID.Value == oldDetails.FirstOrDefault(o => o.ExternID == orderItem.OrderItemId.ToString())?.LocalID);
				if (detail == null) detail = impl.Details.FirstOrDefault(x => x.ExternalRef?.Value != null && x.ExternalRef?.Value == orderItem.OrderItemId.ToString());
				if (detail == null)
				{
					String inventoryCD = helper.GetInventoryCDByExternID(orderItem.SellerSKU, orderItem.ASIN, AmazonMessages.ItemNotFound, out string uom);
					detail = impl.Details.FirstOrDefault(x => !mappedObject.Details.Any(o => x.NoteID.Value == o.LocalID) && x.InventoryID.Value == inventoryCD);
				}
				if (detail != null)
				{
					mappedObject.AddDetail(BCEntitiesAttribute.OrderLine, detail.NoteID.Value, orderItem.OrderItemId.ToString());
					continue;
				}
				throw new PXException(BCMessages.CannotMapLines);
			}

			UpdateStatus(mappedObject, operation);
		}

		#endregion

		#region Export
		public override async Task FetchBucketsForExport(DateTime? minDateTime, DateTime? maxDateTime, PXFilterRow[] filters, CancellationToken cancellationToken = default)
		{
			var bindingExt = GetBindingExt<BCBindingExt>();
			var amazonbindingExt = GetBindingExt<BCBindingAmazon>();
			var minDate = minDateTime == null || (minDateTime != null && bindingExt.SyncOrdersFrom != null && minDateTime < bindingExt.SyncOrdersFrom) ? bindingExt.SyncOrdersFrom : minDateTime;
			var impls = cbapi.GetAll<SalesOrder>(
					new SalesOrder()
					{
						OrderType = amazonbindingExt.SellerFulfilledOrderType.SearchField(),
						OrderNbr = new StringReturn(),
						Status = new StringReturn(),
						CustomerID = new StringReturn(),
						ExternalRef = new StringReturn(),
						Details = new List<SalesOrderDetail>() { new SalesOrderDetail() {
							ReturnBehavior = ReturnBehavior.OnlySpecified,
							InventoryID = new StringReturn() } }
					},
					minDate, maxDateTime, filters).ToArray();

			if (impls != null && impls.Count() > 0)
			{
				int countNum = 0;
				List<IMappedEntity> mappedList = new List<IMappedEntity>();
				foreach (SalesOrder impl in impls)
				{
					IMappedEntity obj = CreateMappedOrder(impl);

					mappedList.Add(obj);
					countNum++;
					if (countNum % BatchFetchCount == 0 || countNum == impls.Count())
					{
						ProcessMappedListForExport(mappedList);
					}
				}
			}
		}

		public override async Task<EntityStatus> GetBucketForExport(AmazonOrderBucketBase<TPrimaryMapped> bucket, BCSyncStatus syncstatus, CancellationToken cancellationToken = default)
		{
			SalesOrder impl = cbapi.GetByID<SalesOrder>(syncstatus.LocalID);
			if (impl == null) return EntityStatus.None;

			TPrimaryMapped obj = bucket.Order = bucket.Order.Set(impl, impl.SyncID, impl.SyncTime);
			EntityStatus status = EnsureStatus(bucket.Order, SyncDirection.Export);

			return status;
		}

		public override async Task SaveBucketExport(AmazonOrderBucketBase<TPrimaryMapped> bucket, IMappedEntity existing, string operation, CancellationToken cancellationToken = default)
		{

		}
		#endregion

		protected abstract BCMappedOrderEntity CreateMappedOrder(Order order);

		protected abstract BCMappedOrderEntity CreateMappedOrder(SalesOrder data);

		protected abstract StringValue GetOrderType(BCBindingAmazon bCBindingAmazon);

		protected abstract DateTime? GetOrderDate(Order orderData);

		protected abstract void MapFreight(SalesOrder impl, decimal? freight, BCBindingAmazon bCBindingAmazon);

		protected virtual (string WarehouseCD, string LocationCD) MarketplaceWarehouseLocation { get; }

		protected string[] GetSOTypeList(BCBindingAmazon bCBindingAmazon)
		{
			List<string> result = new List<string>();
			result.Add(GetOrderType(bCBindingAmazon).Value);
			helper.TryGetCustomOrderTypeMappings(ref result);
			return result.ToArray();
		}
	}
}
