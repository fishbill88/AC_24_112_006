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

using System.Collections.Generic;

namespace PX.Objects.AR.CCPaymentProcessing.Common
{
	/// <summary>
	/// The class that contains the processing Level 3 data.
	/// </summary>
	public class TranProcessingL3DataInput
	{
		/// <summary>
		/// A previously returned transaction_id that is used for Level 3 transactions.
		/// </summary>
		public string TransactionId { get; set; }
		/// <summary>
		/// Code of the country where the goods are being shipped.
		/// </summary>
		public string DestinationCountryCode { get; set; }
		/// <summary>
		/// Fee amount associated with the import of the purchased goods.
		/// </summary>
		public decimal? DutyAmount { get; set; }
		/// <summary>
		/// Freight or shipping portion of the total transaction amount.
		/// </summary>
		public decimal? FreightAmount { get; set; }
		/// <summary>
		/// National tax for the transaction.
		/// </summary>
		public decimal? NationalTax { get; set; }
		/// <summary>
		/// Sales tax for the transaction.
		/// </summary>
		public decimal? SalesTax { get; set; }
		/// <summary>
		/// Postal/ZIP code of the address from where the purchased goods are being shipped.
		/// </summary>
		public string ShipfromZipCode { get; set; }
		/// <summary>
		/// Postal/ZIP code of the address where purchased goods will be delivered.
		/// </summary>
		public string ShiptoZipCode { get; set; }
		/// <summary>
		/// Amount of any value added taxes.
		/// </summary>
		public decimal? TaxAmount { get; set; }
		/// <summary>
		/// Sales Tax Exempt. Allowed values: "1", "0".
		/// </summary>
		public bool TaxExempt { get; set; }
		/// <summary>
		/// List of line items.
		/// </summary>
		public List<TranProcessingL3DataLineItemInput> LineItems { get; set; }

		// Visa specific fields
		/// <summary>
		/// Tax registration number supplied by the Commercial Card cardholder.
		/// </summary>
		public string CustomerVatRegistration { get; set; }
		/// <summary>
		/// Government assigned tax identification number of the Merchant.
		/// </summary>
		public string MerchantVatRegistration { get; set; }
		/// <summary>
		/// The purchase order date. Format: "YYMMDD".
		/// </summary>
		public string OrderDate { get; set; }
		/// <summary>
		/// International description code of the overall goods or services being supplied.
		/// </summary>
		public string SummaryCommodityCode { get; set; }
		/// <summary>
		/// Tax rate used to calculate the sales tax amount.
		/// </summary>
		public decimal? TaxRate { get; set; }
		/// <summary>
		/// Invoice number that is associated with the VAT invoice.
		/// </summary>
		public string UniqueVatRefNumber { get; set; }
		/// <summary>
		/// Type of a card associated with the customer payment method. 
		/// </summary>
		public virtual CCCardType CardType { get; set; }
	}
}
