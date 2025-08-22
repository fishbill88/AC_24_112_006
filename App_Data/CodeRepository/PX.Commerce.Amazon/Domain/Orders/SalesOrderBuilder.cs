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
using PX.Common;
using PX.Data;
using PX.Objects.CS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PX.Commerce.Amazon.Domain.Orders
{
	internal class SalesOrderBuilder :
		IExpectSalesOrderInitialInfo, IExpectSalesOrderSummary, IExpectSalesOrderDetails, IExpectSalesOrderTaxes, IExpectSalesOrderDiscounts, IExpectSalesOrderShippingSettings, IExpectSalesOrderShipToAddressAndContact, IExpectSalesOrderBillToAddressAndContact, IExpectSalesOrderOptionalInfo
	{
		private SalesOrder _salesOrder;

		public SalesOrderBuilder() => _salesOrder = new();

		public IExpectSalesOrderSummary WithSummary() => this;


		IExpectSalesOrderSummary IExpectSalesOrderSummary.WithType(string orderType)
		{
			_salesOrder.OrderType = orderType.ValueField();
			return this;
		}

		IExpectSalesOrderSummary IExpectSalesOrderSummary.WithCustomer(string customerID)
		{
			_salesOrder.CustomerID = customerID.ValueField();
			return this;
		}

		IExpectSalesOrderSummary IExpectSalesOrderSummary.WithCurrency(string currencyCode)
		{
			_salesOrder.CurrencyID = currencyCode?.ValueField();
			return this;
		}

		IExpectSalesOrderSummary IExpectSalesOrderSummary.WithDate(DateTime? purchaseDate)
		{
			// TODO: check if we need this check here
			if (purchaseDate.HasValue)
				_salesOrder.Date = new DateTime(purchaseDate.Value.Date.Ticks).ValueField();
			_salesOrder.RequestedOn = _salesOrder.Date;
			return this;
		}

		IExpectSalesOrderSummary IExpectSalesOrderSummary.WithDescription(string description)
		{
			_salesOrder.Description = description.ValueField();
			return this;
		}

		IExpectSalesOrderSummary IExpectSalesOrderSummary.WithExternalReferences(string externalRef, string externalOrigin)
		{
			_salesOrder.ExternalRef = externalRef.ValueField();
			_salesOrder.ExternalOrderOriginal = true.ValueField();
			_salesOrder.ExternalOrderOrigin = externalOrigin.ValueField();
			return this;
		}

		IExpectSalesOrderSummary IExpectSalesOrderSummary.WithBranch(string branchCD)
		{
			_salesOrder.FinancialSettings = new();
			_salesOrder.FinancialSettings.Branch = branchCD?.Trim().ValueField();
			return this;
		}

		IExpectSalesOrderDetails IExpectSalesOrderSummary.WithDetails()
		{
			_salesOrder.Details = new();
			return this;
		}

		IExpectSalesOrderDetails IExpectSalesOrderDetails.WithDetails(IEnumerable<OrderItem> OrderItems,
			IMappedEntity existing,
			IDictionary<string, (string InventoryCD, string UOM)> inventoryInfoForDetails,
			(string MarketplaceWarehouse, string MarketplaceLocation) marketplaceDefaults)
		{
			foreach (var orderItem in OrderItems)
			{
				decimal? quantity = orderItem.QuantityOrdered;
				decimal? subTotal = orderItem.ItemPrice?.Amount?.ToDecimal(NumberStyles.Currency) ?? decimal.Zero;
				SalesOrderDetail detail = new();
				detail.DiscountAmount = decimal.Zero.ValueField();
				detail.InventoryID = inventoryInfoForDetails[orderItem.OrderItemId].InventoryCD.ValueField();
				detail.UOM = inventoryInfoForDetails[orderItem.OrderItemId].UOM.ValueField();
				detail.Branch = _salesOrder.FinancialSettings.Branch;
				detail.OrderQty = quantity.ValueField();
				detail.UnitPrice = (quantity > decimal.Zero ? subTotal / quantity : decimal.Zero).ValueField();
				detail.LineDescription = orderItem.Title.ValueField();
				detail.ManualPrice = true.ValueField();
				detail.ExternalRef = orderItem.OrderItemId.ToString().ValueField();

				decimal itemfreight = orderItem.ShippingPrice?.Amount?.ToDecimal(NumberStyles.Currency) ?? decimal.Zero;
				var itemShippingDiscount = orderItem.ShippingDiscount?.Amount?.ToDecimal(NumberStyles.Currency) ?? decimal.Zero;
				var itemPromotionalDiscount = orderItem.PromotionDiscount?.Amount?.ToDecimal(NumberStyles.Currency) ?? decimal.Zero;

				if (!string.IsNullOrWhiteSpace(marketplaceDefaults.MarketplaceWarehouse))
					detail.WarehouseID = marketplaceDefaults.MarketplaceWarehouse.ValueField();

				if (!string.IsNullOrWhiteSpace(marketplaceDefaults.MarketplaceLocation))
					detail.Location = marketplaceDefaults.MarketplaceLocation.ValueField();

				//Check for existing				
				DetailInfo matchedDetail = existing?.Details?.FirstOrDefault(d => d.EntityType == BCEntitiesAttribute.OrderLine && orderItem.OrderItemId.ToString() == d.ExternID);
				if (matchedDetail != null)
					detail.Id = matchedDetail.LocalID; //Search by Details
				else if (existing != null && existing.Details.Any()) //Serach by Existing line
				{
					SalesOrderDetail matchedLine = ((SalesOrder)existing).Details
						.FirstOrDefault(x => x.ExternalRef?.Value != null && x.ExternalRef?.Value == orderItem.OrderItemId.ToString()
										  || x.InventoryID?.Value?.Trim() == detail.InventoryID?.Value?.Trim()
										  && (detail.UOM == null || detail.UOM.Value == x.UOM?.Value));

					if (matchedLine != null && !_salesOrder.Details.Any(i => i.Id == matchedLine.Id))
						detail.Id = matchedLine.Id;
				}

				_salesOrder.Details.Add(detail);
			}

			return this;
		}

		[Obsolete(BCMessages.ItemIsObsoleteAndWillBeRemoved2025R1)]
		IExpectSalesOrderDetails IExpectSalesOrderDetails.WithDetails(IEnumerable<OrderItem> OrderItems, IMappedEntity existing, IDictionary<string, (string InventoryCD, string UOM)> inventoryInfoForDetails)
		{
			foreach (var orderItem in OrderItems)
			{
				decimal? quantity = orderItem.QuantityOrdered;
				decimal? subTotal = orderItem.ItemPrice?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0;
				SalesOrderDetail detail = new();
				detail.DiscountAmount = 0m.ValueField();
				detail.InventoryID = inventoryInfoForDetails[orderItem.OrderItemId].InventoryCD.ValueField();
				detail.UOM = inventoryInfoForDetails[orderItem.OrderItemId].UOM.ValueField();
				detail.Branch = _salesOrder.FinancialSettings.Branch;
				detail.OrderQty = quantity.ValueField();
				detail.UnitPrice = (quantity > 0 ? subTotal / quantity : 0).ValueField();
				detail.LineDescription = orderItem.Title.ValueField();
				detail.ManualPrice = true.ValueField();
				detail.ExternalRef = orderItem.OrderItemId.ToString().ValueField();

				decimal itemfreight = orderItem.ShippingPrice?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0;
				var itemShippingDiscount = orderItem.ShippingDiscount?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0;
				var itemPromotionalDiscount = orderItem.PromotionDiscount?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0;

				//Check for existing				
				DetailInfo matchedDetail = existing?.Details?.FirstOrDefault(d => d.EntityType == BCEntitiesAttribute.OrderLine && orderItem.OrderItemId.ToString() == d.ExternID);
				if (matchedDetail != null) detail.Id = matchedDetail.LocalID; //Search by Details
				else if (existing?.Details.Count() > 0) //Serach by Existing line
				{
					SalesOrderDetail matchedLine = ((SalesOrder)existing).Details.FirstOrDefault(x =>
						x.ExternalRef?.Value != null && x.ExternalRef?.Value == orderItem.OrderItemId.ToString()
						||
						x.InventoryID?.Value?.Trim() == detail.InventoryID?.Value?.Trim() && (detail.UOM == null || detail.UOM.Value == x.UOM?.Value));
					if (matchedLine != null && !_salesOrder.Details.Any(i => i.Id == matchedLine.Id)) detail.Id = matchedLine.Id;
				}

				_salesOrder.Details.Add(detail);
			}

			return this;
		}

		IExpectSalesOrderDetails IExpectSalesOrderDetails.WithGiftWrapDetail(string giftInventoryCD, string giftUOM, decimal giftPrice)
		{
			if (giftPrice > 0)
			{
				if (string.IsNullOrWhiteSpace(giftInventoryCD))
					throw new PXException(AmazonMessages.GiftWrappingItemDoesNotExist);

				SalesOrderDetail giftWrapDetail = new();
				giftWrapDetail.Branch = _salesOrder.FinancialSettings.Branch;
				giftWrapDetail.OrderQty = ((decimal)1).ValueField();
				giftWrapDetail.UnitPrice = giftPrice.ValueField();
				giftWrapDetail.InventoryID = giftInventoryCD.ValueField();
				giftWrapDetail.UOM = giftUOM.ValueField();
				giftWrapDetail.ManualPrice = true.ValueField();
				_salesOrder.Details.Add(giftWrapDetail);
			}
			return this;
		}

		IExpectSalesOrderTaxes IExpectSalesOrderDetails.WithTaxes()
		{
			_salesOrder.TaxDetails = new();
			return this;
		}

		IExpectSalesOrderTaxes IExpectSalesOrderTaxes.WithTaxes(decimal taxAmount, decimal taxableAmount, string DefaultTaxZoneID, string DefaultTaxID, bool? taxSynchronization)
		{
			if (taxSynchronization == true)
			{
				_salesOrder.IsTaxValid = true.ValueField();
				if (string.IsNullOrEmpty(DefaultTaxID)) throw new PXException(AmazonMessages.NoDefaultTaxID);
				_salesOrder.TaxDetails.Add(new()
				{
					TaxID = DefaultTaxID.ValueField(),
					TaxAmount = taxAmount.ValueField(),
					TaxableAmount = taxableAmount.ValueField()
				}); ;
				_salesOrder.FinancialSettings.OverrideTaxZone = true.ValueField();
				_salesOrder.FinancialSettings.CustomerTaxZone = DefaultTaxZoneID.ValueField();
			}
			//Set tax calculation to default mode
			_salesOrder.TaxCalcMode = PX.Objects.TX.TaxCalculationMode.TaxSetting.ValueField();
			return this;
		}

		IExpectSalesOrderDiscounts IExpectSalesOrderTaxes.WithDiscounts()
		{
			_salesOrder.DiscountDetails = new();
			return this;
		}

		IExpectSalesOrderDiscounts IExpectSalesOrderDiscounts.WithDisabledAutomaticDiscountUpdate()
		{
			_salesOrder.DisableAutomaticDiscountUpdate = true.ValueField();
			return this;
		}

		IExpectSalesOrderDiscounts IExpectSalesOrderDiscounts.WithShippingDiscount(decimal shippingDiscount, string PostDiscounts)
		{
			if (PostDiscounts == BCPostDiscountAttribute.DocumentDiscount)
			{
				if (shippingDiscount > 0)
				{
					SalesOrdersDiscountDetails detail = new();
					detail.Type = PX.Objects.Common.Discount.DiscountType.ExternalDocument.ValueField();
					detail.Description = AmazonMessages.ShippingDiscount.ValueField();
					detail.ExternalDiscountCode = AmazonMessages.ShippingDiscount.ValueField();
					detail.DiscountAmount = shippingDiscount.ValueField();
					_salesOrder.DiscountDetails.Add(detail);
				}
			}
			return this;
		}

		IExpectSalesOrderDiscounts IExpectSalesOrderDiscounts.WithPromotionalDiscount(decimal promotionalDiscount, string PostDiscounts)
		{
			if (PostDiscounts == BCPostDiscountAttribute.DocumentDiscount)
			{
				if (promotionalDiscount > 0)
				{
					SalesOrdersDiscountDetails detail = new();
					detail.Type = PX.Objects.Common.Discount.DiscountType.ExternalDocument.ValueField();
					detail.Description = AmazonMessages.PromotionalDiscount.ValueField();
					detail.ExternalDiscountCode = AmazonMessages.PromotionalDiscount.ValueField();
					detail.DiscountAmount = promotionalDiscount.ValueField();
					_salesOrder.DiscountDetails.Add(detail);
				}
			}
			return this;
		}

		IExpectSalesOrderShippingSettings IExpectSalesOrderDiscounts.WithShippingSettings()
		{
			_salesOrder.ShippingSettings = new();
			return this;
		}

		IExpectSalesOrderShippingSettings IExpectSalesOrderShippingSettings.WithCancelByDate(DateTime? latestShipDate)
		{
			if (latestShipDate.HasValue)
				_salesOrder.ShippingSettings.CancelByDate = new DateTime(latestShipDate.Value.Date.Ticks).ValueField();

			return this;
		}

		IExpectSalesOrderShippingSettings IExpectSalesOrderShippingSettings.WithShippingSettings(BCShippingMappings shippingMapping)
		{
			_salesOrder.ShipVia = shippingMapping.CarrierID.ValueField();
			_salesOrder.ShippingSettings.ShipVia = shippingMapping.CarrierID.ValueField();
			_salesOrder.ShippingSettings.ShippingZone = shippingMapping.ZoneID.ValueField();
			_salesOrder.ShippingSettings.ShippingTerms = shippingMapping.ShipTermsID.ValueField();

			return this;
		}

		IExpectSalesOrderShipToAddressAndContact IExpectSalesOrderShippingSettings.WithShipToInfo()
		{
			_salesOrder.ShipToAddress = new();
			_salesOrder.ShipToContact = new();
			return this;
		}

		IExpectSalesOrderShipToAddressAndContact IExpectSalesOrderShipToAddressAndContact.WithShipToAddressAndContact(API.Rest.Address shippingAddress, BuyerInfo buyerInfo, string guestCustomerCountryID)
		{
			if (shippingAddress != null)
			{
				_salesOrder.ShippingSettings.ResidentialDelivery = (string.Equals(shippingAddress.AddressType, AddressType.Residential, StringComparison.OrdinalIgnoreCase) ? true : false).ValueField();
				_salesOrder.ShipToAddressOverride = true.ValueField();
				_salesOrder.ShipToAddress.AddressLine1 = shippingAddress.AddressLine1?.ValueField();
				_salesOrder.ShipToAddress.AddressLine2 = shippingAddress.AddressLine2?.ValueField();
				_salesOrder.ShipToAddress.City = shippingAddress.City?.ValueField();
				_salesOrder.ShipToAddress.Country = shippingAddress?.CountryCode?.ValueField();
				_salesOrder.ShipToAddress.State = shippingAddress.StateOrRegion.ValueField();

				_salesOrder.ShipToAddress.PostalCode = shippingAddress.PostalCode?.ToUpperInvariant()?.ValueField();

				_salesOrder.ShipToContactOverride = true.ValueField();
				_salesOrder.ShipToContact.Phone1 = shippingAddress.Phone?.ValueField();
				_salesOrder.ShipToContact.Email = buyerInfo.BuyerEmail?.ValueField();
				_salesOrder.ShipToContact.Attention = shippingAddress.Name?.ValueField();

			}
			else
			{
				_salesOrder.ShipToAddress.AddressLine1 = string.Empty.ValueField();
				_salesOrder.ShipToAddress.AddressLine2 = string.Empty.ValueField();
				_salesOrder.ShipToAddress.City = string.Empty.ValueField();
				_salesOrder.ShipToAddress.State = string.Empty.ValueField();
				_salesOrder.ShipToAddress.PostalCode = string.Empty.ValueField();
				_salesOrder.ShipToContact.Phone1 = string.Empty.ValueField();
				_salesOrder.ShipToContact.Email = buyerInfo?.BuyerEmail?.ValueField();
				_salesOrder.ShipToContact.Attention = string.Empty.ValueField();
				_salesOrder.ShipToContact.BusinessName = string.Empty.ValueField();
				_salesOrder.ShipToAddressOverride = true.ValueField();
				_salesOrder.ShipToContactOverride = true.ValueField();
				_salesOrder.ShipToAddress.Country = guestCustomerCountryID?.ValueField();
			}

			return this;
		}

		IExpectSalesOrderShipToAddressAndContact IExpectSalesOrderShipToAddressAndContact.WithStateSubstitution(State state, string stateSubstitution)
		{
			if (!string.IsNullOrEmpty(_salesOrder.ShipToAddress.State?.Value))
			{
				if (state == null)
					_salesOrder.ShipToAddress.State = stateSubstitution.ValueField();
				else
					_salesOrder.ShipToAddress.State = state.StateID?.ValueField();
			}
			else
				_salesOrder.ShipToAddress.State = string.Empty.ValueField();

			return this;
		}

		IExpectSalesOrderBillToAddressAndContact IExpectSalesOrderShipToAddressAndContact.WithBillToInfo()
		{
			_salesOrder.BillToContact = new();
			_salesOrder.BillToAddress = new();
			return this;
		}

		IExpectSalesOrderBillToAddressAndContact IExpectSalesOrderBillToAddressAndContact.WithSameBillToAdressAndContactFromShipTo()
		{
			_salesOrder.BillToAddress.AddressLine1 = _salesOrder.ShipToAddress.AddressLine1;
			_salesOrder.BillToAddress.AddressLine2 = _salesOrder.ShipToAddress.AddressLine2;
			_salesOrder.BillToAddress.City = _salesOrder.ShipToAddress.City;
			_salesOrder.BillToAddress.Country = _salesOrder.ShipToAddress.Country;
			_salesOrder.BillToAddress.State = _salesOrder.ShipToAddress.State;
			_salesOrder.BillToAddress.PostalCode = _salesOrder.ShipToAddress.PostalCode;
			_salesOrder.BillToAddressOverride = true.ValueField();
			_salesOrder.BillToContact.Phone1 = _salesOrder.ShipToContact.Phone1;
			_salesOrder.BillToContact.Email = _salesOrder.ShipToContact.Email;
			_salesOrder.BillToContact.BusinessName = _salesOrder.ShipToContact.BusinessName;
			_salesOrder.BillToContact.Attention = _salesOrder.ShipToContact.Attention;
			_salesOrder.BillToContactOverride = true.ValueField();
			return this;
		}

		IExpectSalesOrderOptionalInfo IExpectSalesOrderBillToAddressAndContact.WithOptionalInfo() { return this; }

		IExpectSalesOrderOptionalInfo IExpectSalesOrderOptionalInfo.WithAdjustmentsForExistingOrder(SalesOrder presented)
		{
			if (presented != null)
			{
				_salesOrder.OrderType = presented.OrderType; //Keep the same order Type

				//remap entities if existing
				presented.DiscountDetails?.ForEach(e => _salesOrder.DiscountDetails?.FirstOrDefault(n => n.ExternalDiscountCode.Value == e.ExternalDiscountCode.Value).With(n => n.Id = e.Id));
				//delete unnecessary entities
				_salesOrder.Details?.AddRange(presented.Details == null ? Enumerable.Empty<SalesOrderDetail>()
					: presented.Details.Where(e => _salesOrder.Details == null || !_salesOrder.Details.Any(n => e.Id == n.Id)).Select(n => new SalesOrderDetail() { Id = n.Id, Delete = true }));
				_salesOrder.DiscountDetails?.AddRange(presented.DiscountDetails == null ? Enumerable.Empty<SalesOrdersDiscountDetails>()
					: presented.DiscountDetails.Where(e => _salesOrder.DiscountDetails == null || !_salesOrder.DiscountDetails.Any(n => e.Id == n.Id)).Select(n => new SalesOrdersDiscountDetails() { Id = n.Id, Delete = true }));
			}
			return this;
		}

		SalesOrder IExpectSalesOrderOptionalInfo.Build() => _salesOrder;
	}
}
