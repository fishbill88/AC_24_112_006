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
    /// Issues associated with the listings item.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ItemIssues
    {
        /// <summary>
        /// An issue code that identifies the type of issue.
        /// </summary>
        /// <value>An issue code that identifies the type of issue.</value>
        [JsonProperty("code")]
        [CommerceDescription(AmazonCaptions.Code, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
        public string Code { get; set; }

        /// <summary>
        /// A message that describes the issue.
        /// </summary>
        /// <value>A message that describes the issue.</value>
        [JsonProperty("message")]
        [CommerceDescription(AmazonCaptions.Message, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
        public string Message { get; set; }

        /// <summary>
        /// The severity of the issue.
        /// </summary>
        /// <value>The severity of the issue.</value>
        //[JsonProperty("severity")]
        //[CommerceDescription(AmazonCaptions.Severity, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
        //public Severity Severity { get; set; }

        /// <summary>
        /// Names of the attributes associated with the issue, if applicable.
        /// </summary>
        /// <value>Names of the attributes associated with the issue, if applicable.</value>
        [JsonProperty("attributeNames")]
        [CommerceDescription(AmazonCaptions.AttributeNames, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
        public List<string> AttributeNames { get; set; }
    }
}
