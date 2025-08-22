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
    /// A Item Buyer buyer information.
    /// </summary>
    public  class ItemBuyerInfo : BCAPIEntity
    {
       
        /// <summary>
        /// Buyer information for custom orders from the Amazon Custom program.
        /// </summary>
        /// <value>Buyer information for custom orders from the Amazon Custom program.</value>
        [JsonProperty("BuyerCustomizedInfo")]
        public BuyerCustomizedInfoDetail BuyerCustomizedInfo { get; set; }

        /// <summary>
        /// The gift wrap price of the item.
        /// </summary>
        /// <value>The gift wrap price of the item.</value>
        [JsonProperty("GiftWrapPrice")]
        public Money GiftWrapPrice { get; set; }

        /// <summary>
        /// The tax on the gift wrap price.
        /// </summary>
        /// <value>The tax on the gift wrap price.</value>
        [JsonProperty("GiftWrapTax")]
        public Money GiftWrapTax { get; set; }

        /// <summary>
        /// A gift message provided by the buyer.
        /// </summary>
        /// <value>A gift message provided by the buyer.</value>
        [JsonProperty("GiftMessageText")]
        public string GiftMessageText { get; set; }

        /// <summary>
        /// The gift wrap level specified by the buyer.
        /// </summary>
        /// <value>The gift wrap level specified by the buyer.</value>
        [JsonProperty("GiftWrapLevel")]
        public string GiftWrapLevel { get; set; }

    }
}
