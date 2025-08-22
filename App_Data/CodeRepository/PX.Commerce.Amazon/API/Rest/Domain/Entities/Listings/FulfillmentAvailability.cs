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
    /// Fulfillment availability details for the listings item.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class FulfillmentAvailability
    {
        /// <summary>
        /// Designates which fulfillment network will be used.
        /// </summary>
        /// <value>Designates which fulfillment network will be used.</value>
        [JsonProperty("fulfillmentChannelCode")]
        [CommerceDescription(AmazonCaptions.FulfillmentChannelCode, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
        public string FulfillmentChannelCode { get; set; }

        /// <summary>
        /// The quantity of the item you are making available for sale.
        /// </summary>
        /// <value>The quantity of the item you are making available for sale.</value>
        [JsonProperty("quantity")]
        [CommerceDescription(AmazonCaptions.Quantity, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
        public int? Quantity { get; set; }

    }
}
