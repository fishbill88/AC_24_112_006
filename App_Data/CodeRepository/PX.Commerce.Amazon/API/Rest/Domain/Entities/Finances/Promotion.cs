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
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Promotion
    {
        /// <summary>
        /// The seller-specified identifier for the promotion.
        /// </summary>
        /// <value>The seller-specified identifier for the promotion.</value>
        [JsonProperty("PromotionId")]
        public string PromotionId { get; set; }

        /// <summary>
        /// The type of promotion.
        /// </summary>
        /// <value>The type of promotion.</value>
        [JsonProperty("PromotionType")]
        public string PromotionType { get; set; }

        /// <summary>
        /// The amount of promotional discount applied to the item.
        /// </summary>
        /// <value>The amount of promotional discount applied to the item.</value>
        [JsonProperty("PromotionAmount")]
        public Currency PromotionAmount { get; set; }
    }
}
