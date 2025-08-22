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

#nullable enable

using Newtonsoft.Json;
using PX;
using PX.Commerce;
using PX.Commerce.Amazon;
using PX.Commerce.Amazon.API;
using PX.Commerce.Amazon.API.Rest;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace PX.Commerce.Amazon.API
{
	public class JsonProcessingReport
	{
		[JsonProperty("issues")]
		public List<Issues> Issues { get; set; }

		[JsonProperty("header")]
		public Header Header { get; set; }

		[JsonProperty("summary")]
		public Summary Summary { get; set; }
	}
	public class Summary
	{
		[JsonProperty("error")]
		public int Errors { get; set; }

		[JsonProperty("warnings")]
		public int Warnings { get; set; }

		[JsonProperty("messagesProcessed")]
		public int MessagesProcessed { get; set; }

		[JsonProperty("messagesAccepted")]
		public int MessagesAccepted { get; set; }

		[JsonProperty("messagesInvalid")]
		public int MessagesInvalid { get; set; }
	}

	public class Issues
	{
		[JsonProperty("messageId")]
		public int? MessageId { get; set; }

		[JsonProperty("code")]
		public string Code { get; set; }

		[JsonProperty("severity")]
		public string Severity { get; set; }

		[JsonProperty("message")]
		public string Message { get; set; }

	}
}
