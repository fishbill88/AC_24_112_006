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

using PX;
using PX.Commerce;
using PX.Commerce.Amazon;
using PX.Commerce.Amazon.API.Rest;
using PX.Commerce.Amazon.Domain;
using PX.Commerce.Amazon.Domain.Orders;
using PX.Commerce.Core;
using PX.Commerce.Core.API;
using PX.Commerce.Objects;
using PX.Objects.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon.Domain.Orders
{
	interface IExpectSalesOrderInitialInfo
	{
		IExpectSalesOrderSummary WithSummary();
	}

	interface IExpectSalesOrderSummary 
	{
		IExpectSalesOrderSummary WithType(string orderType);
		IExpectSalesOrderSummary WithCustomer(string customerID);
		IExpectSalesOrderSummary WithCurrency(string currencyCode);
		IExpectSalesOrderSummary WithDate(DateTime? purchaseDate);
		IExpectSalesOrderSummary WithDescription(string description);
		IExpectSalesOrderSummary WithExternalReferences(string externalRef, string externalOrigin);

		IExpectSalesOrderSummary WithBranch(string branchCD);

		IExpectSalesOrderDetails WithDetails();
	}

	interface IExpectSalesOrderDetails
	{
		IExpectSalesOrderDetails WithDetails(IEnumerable<OrderItem> OrderItems, IMappedEntity existing, IDictionary<string, (string InventoryCD, string UOM)> inventoryInfoForDetails, (string MarketplaceWarehouse, string MarketplaceLocation) marketplaceDefaults);

		[Obsolete(BCMessages.ItemIsObsoleteAndWillBeRemoved2025R1)]
		IExpectSalesOrderDetails WithDetails(IEnumerable<OrderItem> OrderItems, IMappedEntity existing, IDictionary<string, (string InventoryCD, string UOM)> inventoryInfoForDetails);
		IExpectSalesOrderDetails WithGiftWrapDetail(string giftInventoryCD, string giftUOM, decimal giftPrice);
		IExpectSalesOrderTaxes WithTaxes();
	}

	interface IExpectSalesOrderTaxes
	{
		IExpectSalesOrderTaxes WithTaxes(decimal taxAmount, decimal taxableAmount, string DefaultTaxZoneID, string DefaultTaxID, bool? taxSynchronization);

		IExpectSalesOrderDiscounts WithDiscounts();
	}

	interface IExpectSalesOrderDiscounts
	{
		IExpectSalesOrderDiscounts WithDisabledAutomaticDiscountUpdate();
		IExpectSalesOrderDiscounts WithShippingDiscount(decimal shippingDiscount, string PostDiscounts);
		IExpectSalesOrderDiscounts WithPromotionalDiscount(decimal promotionalDiscount, string PostDiscounts);
		IExpectSalesOrderShippingSettings WithShippingSettings();
	}

	interface IExpectSalesOrderShippingSettings
	{
		IExpectSalesOrderShippingSettings WithCancelByDate(DateTime? latestShipDate);
		IExpectSalesOrderShippingSettings WithShippingSettings(BCShippingMappings shippingMapping);
		IExpectSalesOrderShipToAddressAndContact WithShipToInfo();
	}

	interface IExpectSalesOrderShipToAddressAndContact
	{
		IExpectSalesOrderShipToAddressAndContact WithShipToAddressAndContact(API.Rest.Address shippingAddress, BuyerInfo buyerInfo, string guestCustomerCountryID);
		IExpectSalesOrderShipToAddressAndContact WithStateSubstitution(State state, string stateSubstitution);
		IExpectSalesOrderBillToAddressAndContact WithBillToInfo();

	}

	interface IExpectSalesOrderBillToAddressAndContact
	{
		IExpectSalesOrderBillToAddressAndContact WithSameBillToAdressAndContactFromShipTo();
		IExpectSalesOrderOptionalInfo WithOptionalInfo();
	}

	interface IExpectSalesOrderOptionalInfo
	{
		IExpectSalesOrderOptionalInfo WithAdjustmentsForExistingOrder(SalesOrder presented);
		SalesOrder Build();
	}
}
