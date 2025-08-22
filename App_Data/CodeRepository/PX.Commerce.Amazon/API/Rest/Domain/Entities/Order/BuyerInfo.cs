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

/* 
 * Created class based on https://developer-docs.amazon.com/sp-api/docs/orders-api-v0-model
 */

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using PX.Commerce.Core;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Buyer information for an order.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [CommerceDescription("Buyer Info")]
	public  class BuyerInfo : BCAPIEntity
	{
        /// <summary>
        /// An Amazon-defined order identifier, in 3-7-7 format.
        /// </summary>
        /// <value>An Amazon-defined order identifier, in 3-7-7 format.</value>
        [JsonProperty("AmazonOrderId")]
        public string AmazonOrderId { get; set; }

        /// <summary>
        /// The anonymized email address of the buyer.
        /// </summary>
        /// <value>The anonymized email address of the buyer.</value>
        [JsonProperty("BuyerEmail")]
        [CommerceDescription(AmazonCaptions.BuyerEmail, FieldFilterStatus.Filterable, FieldMappingStatus.Import)]
        public string BuyerEmail { get; set; }

        /// <summary>
        /// The name of the buyer.
        /// </summary>
        /// <value>The name of the buyer.</value>
        [JsonProperty("BuyerName")]
        [CommerceDescription(AmazonCaptions.BuyerName, FieldFilterStatus.Filterable, FieldMappingStatus.Import)]
        public string BuyerName { get; set; }

        /// <summary>
        /// The county of the buyer.
        /// </summary>
        /// <value>The county of the buyer.</value>
        [JsonProperty("BuyerCounty")]
        [CommerceDescription(AmazonCaptions.BuyerCounty, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
        public string BuyerCounty { get; set; }

        /// <summary>
        /// Tax information about the buyer.
        /// </summary>
        /// <value>Tax information about the buyer.</value>
        [JsonProperty("BuyerTaxInfo")]
        [CommerceDescription(AmazonCaptions.BuyerTaxInfo, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
        public BuyerTaxInfo BuyerTaxInfo { get; set; }

        /// <summary>
        /// The purchase order (PO) number entered by the buyer at checkout. Returned only for orders where the buyer entered a PO number at checkout.
        /// </summary>
        /// <value>The purchase order (PO) number entered by the buyer at checkout. Returned only for orders where the buyer entered a PO number at checkout.</value>
        [JsonProperty("PurchaseOrderNumber")]
        [CommerceDescription(AmazonCaptions.PurchaseOrderNumber, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
        public string PurchaseOrderNumber { get; set; }
    }

}
