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
using System.Collections.Generic;
using System.Text;

namespace PX.Commerce.Amazon.API.Rest
{
    //TODO: ADD THE REST OF THE FIELDS
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [CommerceDescription("Listing", FieldFilterStatus.Filterable, FieldMappingStatus.Import)]
    public class MerchantListingData : BCAPIEntity
    {
        [JsonProperty("item-name")]
        public string ItemName { get; set; }

        [JsonProperty("item-description")]
        public string ItemDescription { get; set; }

        [JsonProperty("seller-sku")]
        [CommerceDescription("Seller SKU", FieldFilterStatus.Filterable, FieldMappingStatus.Import)]
        public string SellerSku { get; set; }

        [JsonProperty("fulfillment-channel")]
        public string FulfillmentChannel { get; set; }

        [JsonProperty("asin1")]
        public string Asin1 { get; set; }

        [JsonProperty("asin2")]
        public string Asin2 { get; set; }

        [JsonProperty("asin3")]
        public string Asin3 { get; set; }

        [JsonProperty("product-id")]
        public string ProductId { get; set; }
    }
}