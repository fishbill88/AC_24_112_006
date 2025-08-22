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
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace PX.Commerce.Amazon.API
{
    /// <summary>
    /// Information required for the feed processing report.
    /// </summary>
    public class XmlProcessingReport
    {
        public string DocumentTransactionID;
        public string StatusCode;
        public ProcessingSummary ProcessingSummary;
        [XmlElement("Result")]
        public List<Result> Result;
    }
    public class ProcessingSummary
    {
        public int MessagesProcessed;
        public int MessagesSuccessful;
        public int MessagesWithError;
        public int MessagesWithWarning;
    }
    public class Result
    {
        public string MessageID;
        public string ResultCode;
        public string ResultMessageCode;
        public string ResultDescription;
        public AdditionalInfo AdditionalInfo;
    }
    public class AdditionalInfo
    {
        public string AmazonOrderID;
    }
}
