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
using System.ComponentModel;

namespace PX.Commerce.Amazon.API.Rest
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class FilterOrders :Filter
	{
		[Description("CreatedAfter")]
		public DateTime? CreatedAfter { get; set; }

		[Description("CreatedBefore")]
		public DateTime? CreatedBefore { get; set; }

		[Description("LastUpdatedAfter")]
		public DateTime? LastUpdatedAfter { get; set; }

		[Description("LastUpdatedBefore")]
		public DateTime? LastUpdatedBefore { get; set; }

		[Description("AmazonOrderIds")]
		public string AmazonOrderIds { get; set; }

		[Description("OrderStatuses")]
		public List<string> OrderStatuses { get; set; }

		[Description("MarketplaceIds")]
		public List<string> MarketplaceIds { get; set; }

		[Description("FulfillmentChannels")]
		public string FulfillmentChannels { get; set; }

		[Description("PaymentMethods")]
		public string PaymentMethods { get; set; }

		[Description("BuyerEmail")]
		public string BuyerEmail { get; set; }

		[Description("SellerOrderId")]
		public string SellerOrderId { get; set; }
		[Description("MaxResultsPerPage")]
		public string MaxResultsPerPage { get; set; }

		[Description("EasyShipShipmentStatuses")]
		public string EasyShipShipmentStatuses { get; set; }

		[Description("NextToken")]
		public string NextToken { get; set; }
	}
}
