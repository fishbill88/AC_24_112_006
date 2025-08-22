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
	/// Information about a sub-payment method used to pay for a COD order.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [CommerceDescription(AmazonCaptions.Payment)]
	public  class PaymentExecutionDetailItem : BCAPIEntity
	{
        
        /// <summary>
        /// Gets or Sets Payment
        /// </summary>
        [JsonProperty("Payment")]
        [CommerceDescription(AmazonCaptions.Payment, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
        public Money Payment { get; set; }

        /// <summary>
        /// A sub-payment method for a COD order.  Possible values:  * COD - Cash On Delivery.  * GC - Gift Card.  * PointsAccount - Amazon Points.
        /// </summary>
        /// <value>A sub-payment method for a COD order.  Possible values:  * COD - Cash On Delivery.  * GC - Gift Card.  * PointsAccount - Amazon Points.</value>
        [JsonProperty("PaymentMethod")]
        [CommerceDescription(AmazonCaptions.PaymentMethod, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
        public string PaymentMethod { get; set; }

      
    }

}
