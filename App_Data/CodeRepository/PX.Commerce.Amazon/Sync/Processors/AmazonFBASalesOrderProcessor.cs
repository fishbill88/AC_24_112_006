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
using PX.Commerce.Amazon.API.Rest.Constants;
using PX.Commerce.Core;
using PX.Commerce.Core.API;
using PX.Commerce.Objects;
using PX.Data;
using System;
using PX.Objects.SO;
using PX.Api.ContractBased.Models;
using System.Threading.Tasks;
using System.Threading;
using PX.Common;
using PX.Objects.IN;

namespace PX.Commerce.Amazon
{
	// Restrictions relevant only for FBA
	public class AmazonNewInvoiceProcessorRestrictor : BCBaseRestrictor, IRestrictor
	{
		public virtual FilterResult RestrictExport(IProcessor processor, IMappedEntity mapped, FilterMode mode)
		{
			return null;
		}

		public virtual FilterResult RestrictImport(IProcessor processor, IMappedEntity mapped, FilterMode mode)
		{
			return base.Restrict<MappedFBAOrder>(mapped, delegate (MappedFBAOrder obj)
			{
				bool orderExistsAsSOInvoice = processor.SelectStatus(BCEntitiesAttribute.SOInvoice, obj.ExternID, false) != null;

				if (orderExistsAsSOInvoice)
					return new FilterResult(FilterStatus.Ignore,
						PXMessages.LocalizeFormatNoPrefixNLA(AmazonMessages.FBAHasAlreadyBeenImportedAsSOInvoice, obj.Extern.AmazonOrderId));

				BCBindingExt bindingExt = processor.GetBindingExt<BCBindingExt>();

				if (obj.Extern != null
					&& (obj.Extern.OrderStatus == OrderStatus.Pending
						|| obj.Extern.OrderStatus == OrderStatus.PendingAvailability
						|| obj.Extern.OrderStatus == OrderStatus.Unfulfillable))
				{
					return new FilterResult(FilterStatus.Filtered,
						PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogOrderSkippedExternStatusNotSupported, obj.Extern.AmazonOrderId,
							PXMessages.LocalizeFormatNoPrefixNLA(obj.Extern.OrderStatus.ToString())));
				}

				return null;
			});
		}
	}
	[BCProcessor(typeof(BCAmazonConnector), BCEntitiesAttribute.OrderOfTypeInvoice, AmazonCaptions.FBAEntity, 211,
		IsInternal = false,
		Direction = SyncDirection.Import,
		PrimaryDirection = SyncDirection.Import,
		PrimarySystem = PrimarySystem.Extern,
		PrimaryGraph = typeof(PX.Objects.SO.SOOrderEntry),
		ExternTypes = new Type[] { typeof(Order) },
		LocalTypes = new Type[] { typeof(SalesOrder) },
		AcumaticaPrimaryType = typeof(PX.Objects.SO.SOOrder),
		AcumaticaPrimarySelect = typeof(Search2<
			PX.Objects.SO.SOOrder.orderNbr,
			InnerJoin<BCBindingExt,
				On<BCBindingExt.orderType, Equal<SOOrder.orderType>>,
			InnerJoin<BCBinding,
				On<BCBindingExt.bindingID, Equal<BCBinding.bindingID>>>>,
			Where<BCBinding.connectorType, Equal<Current<BCSyncStatusEdit.connectorType>>,
				And<BCBinding.bindingID, Equal<Current<BCSyncStatusEdit.bindingID>>>>>),
		URL = "orders-v3/order/{0}"
		)]
	[BCProcessorDetail(EntityType = BCEntitiesAttribute.OrderLine, EntityName = BCCaptions.OrderLine, AcumaticaType = typeof(PX.Objects.SO.SOLine))]
	[BCProcessorDetail(EntityType = BCEntitiesAttribute.OrderAddress, EntityName = BCCaptions.OrderAddress, AcumaticaType = typeof(PX.Objects.SO.SOOrder))]

	public class AmazonFBASalesOrderProcessor : AmazonOrderBaseProcessor<MappedFBAOrder>, IProcessor
	{
		protected override FilterOrders CreateFilter(DateTime? minDateTime, DateTime? maxDateTime)
		{
			FilterOrders filter = base.CreateFilter(minDateTime, maxDateTime);

			filter.FulfillmentChannels = FulfillmentChannel.AFN;
			filter.OrderStatuses = new() { OrderStatus.Shipped, OrderStatus.Canceled };

			return filter;
		}

		protected override void MapFreight(SalesOrder impl, decimal? freight, BCBindingAmazon bCBindingAmazon)
		{
			if (freight > decimal.Zero)
			{
				(string InventoryCD, string UOM) inventoryAndUOM = helper.GetInventoryCDForShippingItem(bCBindingAmazon.ShippingPriceItem);
				SalesOrderDetail shippingDetail = new()
				{
					Branch = impl.FinancialSettings.Branch,
					OrderQty = 1m.ValueField(),
					UnitPrice = freight.ValueField(),
					InventoryID = inventoryAndUOM.InventoryCD.ValueField(),
					UOM = inventoryAndUOM.UOM.ValueField(),
					ManualPrice = true.ValueField()
				};

				impl.Details.Add(shippingDetail);
			}
		}

		protected override DateTime? GetOrderDate(Order orderData)
		{
			var purchaseDate = orderData.PurchaseDate.ToDate(timeZone: PXTimeZoneInfo.FindSystemTimeZoneById(GetBindingExt<Objects.BCBindingExt>().OrderTimeZone));
			var latestShipDate = orderData.LatestShipDate.ToDate(timeZone: PXTimeZoneInfo.FindSystemTimeZoneById(GetBindingExt<Objects.BCBindingExt>().OrderTimeZone));
			// For FBA , assign LatestShipDate to Date in ERP if it is greater than purchase date else assign purchase date
			if (latestShipDate > purchaseDate)
				return latestShipDate;
			else
				return purchaseDate;

		}

		protected override BCMappedOrderEntity CreateMappedOrder(Order externalOrder) =>
			new MappedFBAOrder(externalOrder, externalOrder.AmazonOrderId?.ToString(), externalOrder.AmazonOrderId?.ToString(), externalOrder.LastUpdateDate.ToDate());

		protected override BCMappedOrderEntity CreateMappedOrder(SalesOrder localOrder) =>
			new MappedFBAOrder(localOrder, localOrder.SyncID, localOrder.SyncTime);

		protected override StringValue GetOrderType(BCBindingAmazon bCBindingAmazon) =>
			bCBindingAmazon.AmazonFulfilledOrderType.ValueField();

		protected override (string WarehouseCD, string LocationCD) MarketplaceWarehouseLocation
		{
			get
			{
				BCBindingAmazon bCBindingAmazon = this.GetBindingExt<BCBindingAmazon>();

				string marketplaceWarehouse = string.Empty;
				string marketplaceLocation = string.Empty;

				if (bCBindingAmazon.Warehouse.HasValue)
					marketplaceWarehouse = INSite.PK.Find(this, bCBindingAmazon.Warehouse).SiteCD.Trim();

				if (bCBindingAmazon.LocationID.HasValue)
					marketplaceLocation = INLocation.PK.Find(this, bCBindingAmazon.LocationID).LocationCD.Trim();

				return (marketplaceWarehouse, marketplaceLocation);
			}
		}
	}
}
