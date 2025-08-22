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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PX;
using PX.Commerce;
using PX.Commerce.Amazon;
using PX.Commerce.Amazon.API;
using PX.Commerce.Amazon.API.Rest;
using PX.Commerce.Amazon.Domain.Orders;
using PX.Commerce.Core;

namespace PX.Commerce.Amazon.Domain.Orders
{
	public class SalesOrderTotals
	{
		private List<OrderItem> OrderItems { get; }
		public decimal Freight => OrderItems.Select(orderItem => orderItem.ShippingPrice?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0).Sum();
		public decimal ShippingDiscount => OrderItems.Select(orderItem => orderItem.ShippingDiscount?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0).Sum();
		public decimal PromotionalDiscount => OrderItems.Select(orderItem => orderItem.PromotionDiscount?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0).Sum();
		public decimal GiftPrice => OrderItems.Select(x => x.BuyerInfo).Sum(x => x.GiftWrapPrice?.Amount.ToDecimal(NumberStyles.Currency) ?? 0);
		private decimal GiftTax => OrderItems.Select(x => x.BuyerInfo).Sum(x => x.GiftWrapTax?.Amount.ToDecimal(NumberStyles.Currency) ?? 0);
		public decimal TaxAmount => OrderItems.Select(orderItem =>
				(orderItem.ItemTax?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0)
				+ (orderItem.ShippingTax?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0)
				+ (orderItem.ShippingDiscountTax?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0)
				+ (orderItem.PromotionDiscountTax?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0))
				.Sum() + GiftTax;

		public decimal TaxableAmount => OrderItems.Select(orderItem => orderItem.ItemPrice?.Amount?.ToDecimal(NumberStyles.Currency) ?? 0).Sum() + Freight + GiftPrice - PromotionalDiscount - ShippingDiscount;

		public SalesOrderTotals(List<OrderItem> data)
		{
			OrderItems = data;
		}
	}
}
