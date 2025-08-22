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
using PX.Commerce.Core.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon.API.Rest
{
    /// <summary>
    /// Offer details for the listings item.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ItemOffers
    {
        /// <summary>
        /// Amazon marketplace identifier.
        /// </summary>
        /// <value>Amazon marketplace identifier.</value>
        [JsonProperty("marketplaceId")]
        [CommerceDescription(AmazonCaptions.MarketplaceId, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
        public string MarketplaceId { get; set; }

        /// <summary>
        /// Type of offer for the listings item.
        /// </summary>
        /// <value>Type of offer for the listings item.</value>
        //[JsonProperty("offerType")]
        //[CommerceDescription(AmazonCaptions.OfferType, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
        //public OfferType offerType { get; set; }

        /// <summary>
        /// Purchase price of the listings item.
        /// </summary>
        /// <value>Purchase price of the listings item.</value>
        [JsonProperty("price")]
        [CommerceDescription(AmazonCaptions.Price, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
        public Money Price { get; set; }

        /// <summary>
        /// Purchase price of the listings item
        /// </summary>
        /// <value>Purchase price of the listings item</value>
        //[JsonProperty("points")]
        //[CommerceDescription(AmazonCaptions.Points, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
        //public Points Points { get; set; }
    }
}
