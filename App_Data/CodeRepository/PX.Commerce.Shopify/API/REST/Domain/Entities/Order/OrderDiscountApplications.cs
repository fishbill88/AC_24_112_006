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

using Newtonsoft.Json;
using PX.Commerce.Core;

namespace PX.Commerce.Shopify.API.REST
{

	[JsonObject(Description = "discount_applications")]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class OrderDiscountApplications : BCAPIEntity
	{
		/// <summary>
		/// The method by which the discount application value has been allocated to entitled lines. 
		/// </summary>
		[JsonProperty("allocation_method")]
		public DiscountAllocationMethods? AllocationMethod { get; set; }

		/// <summary>
		/// The discount code that was used to apply the discount. Available only for discount code applications.
		/// </summary>
		[JsonProperty("code")]
		public string Code { get; set; }

		/// <summary>
		/// The description of the discount application, as defined by the merchant or the Shopify Script. Available only for manual and script discount applications.
		/// </summary>
		[JsonProperty("description")]
		public string Description { get; set; }

		/// <summary>
		/// The lines on the order, of the type defined by target_type, that the discount is allocated over. 
		/// </summary>
		[JsonProperty("target_selection")]
		public DiscountTargetSelection? TargetSelection { get; set; }

		/// <summary>
		/// The type of line on the order that the discount is applicable on.
		/// </summary>
		[JsonProperty("target_type")]
		public DiscountTargetType? TargetType { get; set; }

		/// <summary>
		/// The title of the discount application, as defined by the merchant. Available only for manual discount applications.
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; set; }

		/// <summary>
		/// The discount application type.
		/// </summary>
		[JsonProperty("type")]
		public DiscountApplicationType? Type { get; set; }

		/// <summary>
		/// The value of the discount application as a decimal. This represents the intention of the discount application. 
		/// For example, if the intent was to apply a 20% discount, then the value will be 20.0. If the intent was to apply a $15 discount, then the value will be 15.0.
		/// </summary>
		[JsonProperty("value")]
		public string Value { get; set; }

		/// <summary>
		/// The type of the value.
		/// </summary>
		[JsonProperty("value_type")]
		public DiscountValueType? ValueType { get; set; }
	}

}
