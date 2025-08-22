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
using System.Collections.Generic;

namespace PX.Commerce.Amazon.API.Rest
{
	[JsonObject("header")]
	public class Header
	{
		private const string DocVersion = "1.01";

		private const string Issuelocale = "en_US";

		[JsonProperty("version")]
		public string DocumentVersion { get; set; }

		[JsonProperty("sellerId")]
		public string MerchantIdentifier { get; set; }


		[JsonProperty("issueLocale")]
		public string IssueLocale { get; set; }

		public Header(string merchantIdentifier)
		{
			this.DocumentVersion = DocVersion;
			this.MerchantIdentifier =  merchantIdentifier ;
		}
		public Header(string merchantIdentifier, string docVersion)
		{
			this.DocumentVersion = docVersion;
			this.MerchantIdentifier = merchantIdentifier;
			this.IssueLocale = Issuelocale;

		}

		private Header()
		{

		}
	}
}
