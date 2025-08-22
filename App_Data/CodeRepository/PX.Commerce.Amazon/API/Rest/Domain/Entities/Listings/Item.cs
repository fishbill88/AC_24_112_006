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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon.API.Rest
{
    /// <summary>
	/// A listings item.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [JsonObject("Item")]
    public class ListingItem
    {
        /// <summary>
        /// A selling partner provided identifier for an Amazon listing.
        /// </summary>
        /// <value>A selling partner provided identifier for an Amazon listing.</value>
        [JsonProperty("sku")]
        [CommerceDescription(AmazonCaptions.CurrencyCode, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
        public string SKU { get; set; }

        /// <summary>
        /// Summary details of a listings item.
        /// </summary>
        /// <value>Summary details of a listings item.</value>
        [JsonProperty("summaries")]
        [CommerceDescription(AmazonCaptions.Summaries, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
        public List<ItemSummaries> Summaries { get; set; }

        /// <summary>
        /// JSON object containing structured listings item attribute data keyed by attribute name.
        /// </summary>
        /// <value>JSON object containing structured listings item attribute data keyed by attribute name.</value>
        //[JsonProperty("attributes")]
        //[CommerceDescription(AmazonCaptions.Attributes, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
        //public ItemAttributes Attributes { get; set; }

        /// <summary>
        /// Issues associated with the listings item.
        /// </summary>
        /// <value>Issues associated with the listings item.</value>
        [JsonProperty("issues")]
        [CommerceDescription(AmazonCaptions.Issues, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
        public List<ItemIssues> Issues { get; set; }

        /// <summary>
        /// Offer details for the listings item.
        /// </summary>
        /// <value>Offer details for the listings item.</value>
        [JsonProperty("offers")]
        [CommerceDescription(AmazonCaptions.Offers, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
        public List<ItemOffers> Offers { get; set; }

        /// <summary>
        /// Fulfillment availability for the listings item.
        /// </summary>
        /// <value>Fulfillment availability for the listings item.</value>
        [JsonProperty("fulfillmentAvailability")]
        [CommerceDescription(AmazonCaptions.FulfillmentAvailability, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
        public List<FulfillmentAvailability> FulfillmentAvailability { get; set; }

        /// <summary>
        /// Vendor procurement information for the listings item.
        /// </summary>
        /// <value>Vendor procurement information for the listings item.</value>
        //[JsonProperty("procurement")]
        //[CommerceDescription(AmazonCaptions.Procurement, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
        //public ItemProcurement Procurement { get; set; }
    }
}
