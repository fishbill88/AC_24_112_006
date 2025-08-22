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

using PX.Api.ContractBased.Common.Model;
using PX.Api.ContractBased.Models;
using PX.Commerce.Core;
using PX.Commerce.Core.API;
using PX.Commerce.Objects;
using PX.Commerce.Shopify.API.GraphQL;
using PX.Commerce.Shopify.API.REST;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static PX.Data.BQL.BqlPlaceholder;

namespace PX.Commerce.Shopify
{
	public abstract class SPOrderBaseProcessor<TGraph, TEntityBucket, TPrimaryMapped> : OrderProcessorBase<TGraph, TEntityBucket, TPrimaryMapped>
		where TGraph : PXGraph
		where TEntityBucket : class, IEntityBucket, new()
		where TPrimaryMapped : class, IMappedEntity, new()
	{

		public SPHelper helper = PXGraph.CreateInstance<SPHelper>();
		protected InventoryItem refundItem;
		protected Lazy<InventoryItem> giftCertificateItem;

		public override async Task Initialise(IConnector iconnector, ConnectorOperation operation, CancellationToken cancellationToken = default)
		{
			await base.Initialise(iconnector, operation);

			BCBindingExt bindingExt = GetBindingExt<BCBindingExt>();
			refundItem = bindingExt.RefundAmountItemID != null ? PX.Objects.IN.InventoryItem.PK.Find((PXGraph)this, bindingExt.RefundAmountItemID) : throw new PXException(ShopifyMessages.NoRefundItem);
			giftCertificateItem = new Lazy<InventoryItem>(delegate { return InventoryItem.PK.Find(this, bindingExt.GiftCertificateItemID); }, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
		}

		#region Refunds
		public virtual SalesOrderDetail InsertRefundAmountItem(decimal amount, StringValue branch)
		{
			decimal quantity = amount < 0m ? -1 : 1;
			BCBindingExt bindingExt = GetBindingExt<BCBindingExt>();
			SalesOrderDetail detail = new SalesOrderDetail();
			detail.InventoryID = refundItem.InventoryCD?.TrimEnd().ValueField();
			detail.OrderQty = quantity.ValueField();
			detail.UOM = refundItem.BaseUnit.ValueField();
			detail.Branch = branch;
			//Unit price is not allowed to be negative. We take the absolute value and use -1 for quantity instead.
			detail.UnitPrice = Math.Abs(amount).ValueField();
			detail.ManualPrice = true.ValueField();
			detail.ReasonCode = bindingExt?.ReasonCode?.ValueField();
			return detail;
		}

		/// <summary>
		/// Calculates the discount for the given refunded item.
		/// </summary>
		/// <param name="item">The refunded Item</param>
		public virtual decimal? CalculateRefundItemDiscount(RefundLineItem item)
		{
			return (item.OrderLineItem.PricePresentment * item.Quantity) - item.SubTotalPresentment;
		}
		#endregion

		#region POS Order

		/// <summary>
		/// Check the external order whether is a POS order
		/// </summary>
		/// <param name="data">External order data</param>
		/// <param name="checkPOSFeatureEnabled">Check the POS feature is enabled or not</param>
		/// <returns>True if it's a POS order, false if it's not.</returns>
		/// <exception cref="PXException">If it's a POS order and checkPOSFeatureEnabled is true, but POS feature is disabled, it will throw the exception message </exception>
		protected virtual bool IsPOSOrder(OrderData data, bool checkPOSFeatureEnabled = false)
		{
			if (string.Equals(data?.SourceName, ShopifyConstants.POSSource, StringComparison.OrdinalIgnoreCase))
			{
				if (checkPOSFeatureEnabled && (PXAccess.FeatureInstalled<FeaturesSet.shopifyPOS>() != true || GetBindingExt<BCBindingShopify>().ShopifyPOS != true))
					throw new PXException(ShopifyMessages.POSOrderNotSupported, $"{data?.Name}({data?.Id})");
				return true;
			}
			return false;
		}

		/// <summary>
		/// Check the Transaction whether is the new payment for POS exchange item
		/// </summary>
		/// <param name="data">External Order data</param>
		/// <param name="transactionId">Transaction ID</param>
		/// <returns></returns>
		public virtual bool IsPOSExchangePayment(OrderData data, string transactionId)
		{
			return IsPOSOrder(data) &&
				(data?.ExchangeOrders?.SelectMany(x => x?.Transactions)?.Any(t => string.Equals(t?.Id?.ConvertGidToId(), transactionId)) ?? false);
		}

		public virtual bool IsPOSExchangeOrder(OrderData data)
		{
			return IsPOSOrder(data) && (data?.ExchangeOrders?.Any() ?? false);
		}

		public virtual bool IsPOSExchangeNewItem(OrderData data, string lineItemId)
		{
			return IsPOSOrder(data) &&
				(data?.ExchangeOrders?.SelectMany(x => x?.Additions?.LineItems)?.Any(t => string.Equals(t?.LineItem?.Id?.ConvertGidToId(), lineItemId)) ?? false);
		}

		public virtual bool IsRefundWithPOSExchangeNewItem(OrderData data, string refundId, string lineItemId)
		{
			return IsPOSOrder(data) &&
				(data?.ExchangeOrders?.Any(x => x.Refunds?.Any(refund => string.Equals(refund?.Id?.ConvertGidToId(), refundId)) == true
					&& x.Additions?.LineItems.Any(item => string.Equals(item?.LineItem?.Id?.ConvertGidToId(), lineItemId)) == true)) == true;
		}

		/// <summary>
		/// Based on the order source, refunds info to determine whether fetch POS Exchange data
		/// </summary>
		/// <param name="data">External Order data</param>
		/// <returns></returns>
		public virtual bool ShouldGetPOSExchangeData(OrderData data)
		{
			if(data == null) return false;

			return IsPOSOrder(data) && data.Refunds?.Count > 0 && data.Refunds.Any(x => x.RefundLineItems?.Count > 0 && x.RefundLineItems.Any(r => r.RestockType != RestockType.Cancel));
		}

		/// <summary>
		/// Defines if the POS order is directly sold or shipped.
		/// </summary>
		/// <param name="data">The external order.</param>
		/// <returns>True if the POS order is directly sold; otherwise - shipped.</returns>
		protected bool IsDirectPosOrder(OrderData data) => data?.FulfillmentStatus == OrderFulfillmentStatus.Fulfilled;

		#endregion

		/// <summary>
		/// Based on the order source, get the order/ refund order type from store setting
		/// </summary>
		/// <param name="data">External Order data</param>
		/// <param name="isRefundOrder">If true returns the Refund order type, otherwise returns the order type</param>
		/// <returns></returns>
		protected virtual string GetOrderType(OrderData data, bool isRefundOrder = false)
		{
			var bindingExt = GetBindingExt<BCBindingExt>();
			var bindingShopify = GetBindingExt<BCBindingShopify>();
			string orderType = null;
			//POS order
			if (IsPOSOrder(data, true))
			{
				if (this.IsDirectPosOrder(data))
				{
					orderType = isRefundOrder ? bindingExt.ReturnOrderType : bindingShopify.POSDirectOrderType;
					if (isRefundOrder && data?.ExchangeOrders?.Any() == true)
						orderType = bindingShopify.POSDirectExchangeOrderType;
				}
				else
				{
					orderType = isRefundOrder ? bindingExt.ReturnOrderType : bindingShopify.POSShippingOrderType;
					if (isRefundOrder && data?.ExchangeOrders?.Any() == true)
						orderType = bindingShopify.POSShippingExchangeOrderType;
				}
			}
			else
				orderType = isRefundOrder ? bindingExt.ReturnOrderType : bindingExt.OrderType;

			return orderType;
		}

		/// <summary>
		/// Retrieves the specified Warehouse and Location from the "Warehouse mapping for order import" mapping list.
		/// </summary>
		/// <param name="extOrderItemLocationId">The external LocationId at the order item level. If a mapping matches this LocationId, it will be prioritized.</param>
		/// <param name="extOrderLocationId">The external LocationId at the order level. This will be used to find the mapping if no match is found for <paramref name="extOrderItemLocationId"/>.</param>
		/// <returns>A tuple containing the WarehouseCD (<see cref="string"/> WarehouseCD) and LocationCD (<see cref="string"/> LocationCD) based on the mapping, or an empty tuple if no mapping is found.</returns>
		protected virtual (string WarehouseCD, string LocationCD) GetWarehouseLocationMappingForImport(string extOrderItemLocationId, string extOrderLocationId)
		{
			(string WarehouseCD, string LocationCD) mappingResult = new();

			IEnumerable<BCLocations> importLocationMappings = BCLocationSlot.GetBCLocations(Operation.Binding)?.Where(x => string.Equals(x.MappingDirection, BCMappingDirectionAttribute.Import));
			if (importLocationMappings?.Any() != true)
				return mappingResult;

			BCLocations matchedMapping = null;
			if (!string.IsNullOrEmpty(extOrderItemLocationId))
			{
				matchedMapping = importLocationMappings?.FirstOrDefault(x => string.Equals(x.ExternalLocationID, extOrderItemLocationId));
			}
			if (matchedMapping is null && !string.IsNullOrEmpty(extOrderLocationId))
			{
				matchedMapping = importLocationMappings?.FirstOrDefault(x => string.Equals(x.ExternalLocationID, extOrderLocationId));
			}

			if (matchedMapping != null)
			{
				if (matchedMapping.SiteID.HasValue && BCLocationSlot.GetWarehouses(Operation.Binding).TryGetValue(matchedMapping.SiteID.Value, out INSite warehouse))
					mappingResult.WarehouseCD = warehouse?.SiteCD;
				if (matchedMapping.LocationID.HasValue && BCLocationSlot.GetLocations(Operation.Binding).TryGetValue(matchedMapping.LocationID.Value, out INLocation loc))
					mappingResult.LocationCD = loc?.LocationCD;
			}

			return mappingResult;
		}

		[Obsolete("Starting from 2022R2 the determination of taxes names are done in the scope of helpers. Please delete this method in 2024R1.")]
		protected string DetermineTaxName(OrderData data, OrderTaxLine tax)
		{
			string TaxName = tax.TaxName;
			OrderAddressData taxAddress = data.ShippingAddress ?? data.BillingAddress ?? new OrderAddressData();
			string taxNameWithLocation = TaxName + (taxAddress.CountryCode ?? String.Empty) + (taxAddress.ProvinceCode ?? String.Empty);
			string mappedTaxName = null;
			//Check substituion list for taxCodeWithLocation
			mappedTaxName = GetHelper<SPHelper>().GetSubstituteLocalByExtern(GetBindingExt<BCBindingExt>().TaxSubstitutionListID, taxNameWithLocation, null);
			if (mappedTaxName is null)
			{
				//If not found check taxCodes for taxCodeWithLocation
				GetHelper<SPHelper>().TaxCodes.TryGetValue(taxNameWithLocation, out mappedTaxName);
			}
			if (mappedTaxName is null)
			{
				//If not found check substitution list for taxName
				mappedTaxName = GetHelper<SPHelper>().GetSubstituteLocalByExtern(GetBindingExt<BCBindingExt>().TaxSubstitutionListID, TaxName, null);
			}
			if (mappedTaxName is null)
			{
				//if not found just use tax name
				mappedTaxName = TaxName;
			}
			//Trim found tax name
			mappedTaxName = GetHelper<SPHelper>().TrimAutomaticTaxNameForAvalara(mappedTaxName);
			//check substitution list for trimmed tax name, otherwise use trimmed tax name
			mappedTaxName = GetHelper<SPHelper>().GetSubstituteLocalByExtern(GetBindingExt<BCBindingExt>().TaxSubstitutionListID, mappedTaxName, mappedTaxName);
			return mappedTaxName;
		}
	}
}
