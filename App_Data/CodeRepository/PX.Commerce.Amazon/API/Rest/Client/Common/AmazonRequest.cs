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

using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using PX.Data;

namespace PX.Commerce.Amazon.API.Rest.Client.Common
{
	public class AmazonRequest
	{
		public int Attempts { get; private set; }

		public string BaseUri { get; set; }

		public object Content { get; set; }

		public string ContentType { get; set; }

		public Dictionary<string, string> Headers { get; set; }

		public HttpMethod Method { get; set; }

		public Dictionary<string, string> QueryParameters { get; set; }

		public string RequestUri { get; set; }

		public AmazonRequest()
		{
			this.Headers = new Dictionary<string, string>();
			this.QueryParameters = new Dictionary<string, string>();
			this.RequestUri = string.Empty;
		}

		public HttpRequestMessage ToHttpRequestMessage()
		{
			var request = new HttpRequestMessage(this.Method, this.BaseUri);

			foreach (var header in this.Headers)
				request.Headers.Add(header.Key, header.Value);

			request.RequestUri = new Uri(QueryHelpers.AddQueryString(this.RequestUri, this.QueryParameters), UriKind.Relative);

			if (this.Content != null)
			{
				if (this.ContentType == PX.Commerce.Amazon.API.ContentType.Json)
				{
					if (this.Content is string)
						request.Content = new StringContent(this.Content as string);
					else
						request.Content = new StringContent(JsonConvert.SerializeObject(this.Content), Encoding.UTF8, this.ContentType);

					//
					// The specific of a service, we should avoid using charset. Perhaps the service logic for fetching content headers should be changed.
					//
					request.Content.Headers.ContentType = new MediaTypeHeaderValue(this.ContentType);
				}
				else
				{
					if (!(this.Content is string))
						throw new PXArgumentException(nameof(this.Content), ErrorMessages.ArgumentNullException);

					request.Content = new StringContent(this.Content as string);
				}
			}

			this.Attempts++;

			return request;
		}

		public void ResetAttempts() => this.Attempts = 0;
	}
}
