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

namespace PX.Objects.AR.CCPaymentProcessing.Common
{
	/// <summary>
	/// The class that contains the processing Level 3 data of line item.
	/// </summary>
	public class TranProcessingL3DataLineItemInput
	{
		/// <summary>
		/// Description of the item.
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Total discount amount applied against the line item total.
		/// </summary>
		public decimal? DiscountAmount { get; set; }
		/// <summary>
		/// Merchant-defined description code of the item.
		/// </summary>
		public string ProductCode { get; set; }
		/// <summary>
		/// Quantity of the item.
		/// </summary>
		public decimal? Quantity { get; set; }
		/// <summary>
		/// Amount of any value added taxes.
		/// </summary>
		public decimal? TaxAmount { get; set; }
		/// <summary>
		/// Tax rate used to calculate the sales tax amount.
		/// </summary>
		public decimal? TaxRate { get; set; }
		/// <summary>
		/// Units of measurement as used in international trade.
		/// </summary>
		public string UnitCode { get; set; }
		/// <summary>
		/// Unit cost of the item.
		/// </summary>
		public decimal UnitCost { get; set; }

		// Visa specific fields
		/// <summary>
		/// An international description code of the individual good or service being supplied.
		/// </summary>
		public string CommodityCode { get; set; }
		/// <summary>
		/// Used if city or multiple county taxes need to be broken out separately.
		/// </summary>
		public decimal? OtherTaxAmount { get; set; }

		// Mastercard specific fields
		/// <summary>
		/// Tax identification number of the merchant that reported the alternate tax amount.
		/// </summary>
		public string AlternateTaxId { get; set; }
		/// <summary>
		/// Indicator used to reflect debit (D) or credit (C) transaction. Allowed values: "D", "C".
		/// </summary>
		public string DebitCredit { get; set; }
		/// <summary>
		/// Discount rate for the line item 
		/// </summary>
		public decimal? DiscountRate { get; set; }
		/// <summary>
		/// Type of value-added taxes that are being used (Conditional If tax amount is supplied)
		/// This field is only required when Merchant is directed to include by Mastercard.
		/// </summary>
		public string TaxTypeApplied { get; set; }
		/// <summary>
		/// Indicates the type of tax collected in relationship to a specific tax amount (Conditional If tax amount is supplied).
		/// </summary>
		public string TaxTypeId { get; set; }
	}
}
