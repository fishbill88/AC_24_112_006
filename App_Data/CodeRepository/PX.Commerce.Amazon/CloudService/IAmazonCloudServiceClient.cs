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

using IdentityModel.Client;
using PX.Commerce.Amazon.API.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon
{
	public interface IAmazonCloudServiceClient
	{
		Task<string> Authorize(AmazonStateParameters cloudServiceParameters);
		Task<ServiceURl> GetServiceUrlAndHeader();
		Task<TokenResponse> ProcessAuthorizationCode(string code, string seller, int? bindingId);
		Task<Token> GetAccessToken(BCBindingAmazon bindingAmazon, CancellationToken cancellationToken = default);
		Task<Uri> RefreshServiceUrlAsync();
		Task<AuthenticationHeaderValue> GetFreshAuthorizationHeaderAsync(HttpResponseMessage HttpResponseMessage);
	}

	public class ServiceURl
	{
		public Uri Uri { get; set; }
		public AuthenticationHeaderValue AuthenticationHeaderValue { get; set; }
	}
}
