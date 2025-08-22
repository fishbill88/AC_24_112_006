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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Shopify.API.REST
{
	public class SPRestClient : SPRestClientBase, IShopifyRestClient
	{
		JsonSerializerSettings _serializer;
		public SPRestClient(IHttpClientFactory clientFactory, JsonSerializerSettings serializer, IRestOptions options, Serilog.ILogger logger) : base(clientFactory, serializer, options, logger)
		{
			_serializer = serializer;
		}

		#region API Request

		public async Task<T> Post<T, TR>(HttpRequestMessage request, T obj, bool usingTRasBodyObj = true, CancellationToken cancellationToken = default)
			where T : class, new()
			where TR : class, IEntityResponse<T>, new()
		{
			request.Method = HttpMethod.Post;
			AddBody<T, TR>(request, obj, usingTRasBodyObj);
			var response = await ExecuteRequest<TR>(request, cancellationToken);
			if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
			{
				T result = response.Data?.Data;
				if (result != null && result is BCAPIEntity) (result as BCAPIEntity).JSON = response.Content;
				return result;
			}

			LogError(response);
			throw new RestException(response);
		}

		public async Task<bool> Post(HttpRequestMessage request, CancellationToken cancellationToken = default)
		{
			request.Method = HttpMethod.Post;
			var response = await ExecuteRequest(request, cancellationToken);
			if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
			{
				return true;
			}

			LogError(response);
			throw new RestException(response);
		}

		public async Task<T> Put<T, TR>(HttpRequestMessage request, T obj, bool usingTRasBodyObj = true, CancellationToken cancellationToken = default)
			where T : class, new() where TR : class, IEntityResponse<T>, new()
		{
			request.Method = HttpMethod.Put;
			AddBody<T, TR>(request, obj, usingTRasBodyObj);
			var response = await ExecuteRequest<TR>(request, cancellationToken);
			if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
			{
				T result = response.Data?.Data;
				 if (result != null && result is BCAPIEntity) (result as BCAPIEntity).JSON = response.Content;
				return result;
			}

			LogError(response);
			throw new RestException(response);
		}

		private void AddBody<T, TR>(HttpRequestMessage request, T obj, bool usingTRasBodyObj)
			where T : class, new()
			where TR : class, IEntityResponse<T>, new()
		{
			object _obj = usingTRasBodyObj ? (object)(new TR() { Data = obj }) : (object)obj;
			request.Content = new StringContent(JsonConvert.SerializeObject(_obj, _serializer));
			request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
		}

		public async Task<bool> Delete(HttpRequestMessage request, CancellationToken cancellationToken = default)
		{
			request.Method = HttpMethod.Delete;
			var response = await ExecuteRequest(request, cancellationToken);
			if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound)
			{
				return true;
			}

			LogError(response);
			throw new RestException(response);
		}

		public async Task<T> Get<T, TR>(HttpRequestMessage request, CancellationToken cancellationToken = default)
			where T : class, new() where TR : class, IEntityResponse<T>, new()
		{
			request.Method = HttpMethod.Get;
			var response = await ExecuteRequest<TR>(request, cancellationToken);

			if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound)
			{
				T result = response.Data?.Data;
				 if (result != null && result is BCAPIEntity) (result as BCAPIEntity).JSON = response.Content;
				return result;
			}

			LogError(response);
			throw new RestException(response);
		}


		public async Task<(T, HttpResponseHeaders)> GetWithHeaders<T, TR>(HttpRequestMessage request, CancellationToken cancellationToken = default)
			where T : class, new() where TR : class, IEntityResponse<T>, new()
		{
			request.Method = HttpMethod.Get;
			var response = await ExecuteRequest<TR>(request, cancellationToken);
			if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound)
			{
				T result = response.Data?.Data;


				return (result, response.Headers);
			}

			LogError(response);
			throw new RestException(response);
		}

		public async IAsyncEnumerable<T> GetAll<T, TR>(HttpRequestMessage request, CancellationToken cancellationToken = default) where T : class, new() where TR : class, IEntitiesResponse<T>, new()
		{
			request.Method = HttpMethod.Get;
			bool needGet = true;
			while (needGet)
			{
				cancellationToken.ThrowIfCancellationRequested();
				var response = await ExecuteRequest<TR>(request, cancellationToken);
				if (response.StatusCode == HttpStatusCode.OK)
				{
					var entities = response.Data?.Data;

					if (entities != null && entities.Any())
					{
						foreach (T entity in entities)
						{
							yield return entity;

						}
						if (TryGetNextPageUrl(response.Headers, out _, out var nextUrl))
						{
							request = MakeHttpRequest(nextUrl);
							request.Method = HttpMethod.Get;
							needGet = true;
						}
						else
							needGet = false;
					}
					else
						yield break;
				}
				else
				{
					LogError(response);
					throw new RestException(response);
				}
			}
		}

		private bool TryGetNextPageUrl(HttpResponseHeaders header, out string previousUrl, out string nextUrl)
		{
			previousUrl = nextUrl = default;
			if (header == null || header.Count() == 0) return false;
			var linkStr = header.FirstOrDefault(x => string.Equals(x.Key, "Link", StringComparison.InvariantCultureIgnoreCase)).Value?.FirstOrDefault()?.ToString();
			if (linkStr != null && !string.IsNullOrWhiteSpace(linkStr))
			{
				Match previousMatch = Regex.Match(linkStr, $@"<{_baseUrl}([^\s]*)>;\s*rel=""previous""", RegexOptions.IgnoreCase);
				if (previousMatch.Success && !string.IsNullOrWhiteSpace(previousMatch.Groups[1].Value))
				{
					previousUrl = previousMatch.Groups[1].Value;
				}
				Match nextMatch = Regex.Match(linkStr, $@"<{_baseUrl}([^\s]*)>;\s*rel=""next""", RegexOptions.IgnoreCase);
				if (nextMatch.Success && !string.IsNullOrWhiteSpace(nextMatch.Groups[1].Value))
				{
					nextUrl = nextMatch.Groups[1].Value;
					if (Regex.IsMatch(nextUrl, $@"limit=\d+", RegexOptions.IgnoreCase))
						nextUrl = Regex.Replace(nextUrl, $@"limit=\d+", "limit=250", RegexOptions.IgnoreCase);
					else
						nextUrl += "&limit=250";
					return true;
				}
			}
			return false;
		}
		#endregion
	}
}
