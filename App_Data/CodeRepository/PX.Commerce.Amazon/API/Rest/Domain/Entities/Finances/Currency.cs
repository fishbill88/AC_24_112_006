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

namespace PX.Commerce.Amazon.API.Rest
{
    /// <summary>
	/// A currency type and amount.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Currency
    {
        /// <summary>
        /// The three-digit currency code. In ISO 4217 format.
        /// </summary>
        /// <value>The three-digit currency code. In ISO 4217 format.</value>
        [JsonProperty("CurrencyCode")]
        [CommerceDescription(AmazonCaptions.CurrencyCode, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
        public string CurrencyCode { get; set; }

        /// <summary>
        /// The currency amount.
        /// </summary>
        /// <value>The currency amount.</value>
        [JsonProperty("CurrencyAmount")]
        [CommerceDescription(AmazonCaptions.Amount, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
        public string CurrencyAmount { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid => !string.IsNullOrWhiteSpace(this.CurrencyCode) && !string.IsNullOrWhiteSpace(this.CurrencyAmount);
	}
}
