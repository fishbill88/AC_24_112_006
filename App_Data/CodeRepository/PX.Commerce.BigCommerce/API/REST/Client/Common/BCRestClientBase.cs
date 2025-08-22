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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PX.Commerce.Core;
using PX.Commerce.Core.Utilities;
using PX.Common;
using PX.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.BigCommerce.API.REST
{
	public abstract class BCRestClientBase
	{
		private IHttpClientFactory _clientFactory = null;
		private JsonSerializerSettings _serializerSettings;
		private IRestOptions _options;
		public Serilog.ILogger Logger { get; set; } = null;
		protected readonly int commerceRetryCount = WebConfig.GetInt(BCConstants.COMMERCE_RETRY_COUNT, 3);
		protected BCRestClientBase(IHttpClientFactory clientFactory, JsonSerializerSettings serializer, IRestOptions options, Serilog.ILogger logger)
		{
			_clientFactory = clientFactory;
			_serializerSettings = serializer;
			_options = options;
			Logger = logger;
		}

		public async Task<BCRestResponse<T>> Execute<T>(HttpRequestMessage request, CancellationToken cancellationToken = default)
		{
			var response = await Execute(request, cancellationToken);
			BCRestResponse<T> restResponse = BCRestResponse<T>.FromResponse(response);
			if (restResponse.IsSuccessStatusCode)
			{
				try
				{
					restResponse.Data = DeserializeJsonFromString<T>(restResponse.Content);
				}
				catch
				{
					restResponse.Data = default(T);
				}
			}

			return restResponse;
		}

		public async Task<IBCRestResponse> Execute(HttpRequestMessage originalRequest, CancellationToken cancellationToken = default)
		{
			int retryCount = 0;
			while (true)
			{
				IBCRestResponse restResponse = new BCRestResponse();
				cancellationToken.ThrowIfCancellationRequested();
				using (var client = _clientFactory.CreateClient())
				{
					HttpRequestMessage request = await originalRequest.CloneAsync();
					request.Headers.AddOrReplace("X-Auth-Client", _options.XAuthClient);
					request.Headers.AddOrReplace("X-Auth-Token", _options.XAuthTocken);
					client.DefaultRequestHeaders.AddOrReplace("Accept", "application/json");
					client.BaseAddress = new Uri(_options.BaseUri);
					using (var response = await client.SendAsync(request, cancellationToken))
					{
						restResponse.StatusCode = response.StatusCode;
						restResponse.Headers = response.Headers;
						var content = await response.Content.ReadAsStringAsync();
						restResponse.Content = content;
						restResponse.IsSuccessStatusCode = response.IsSuccessStatusCode;
						if (response.IsSuccessStatusCode)
						{
							return restResponse;
						}
						else if (retryCount < commerceRetryCount)
						{
							//sometimes json is array or object
							if (!string.IsNullOrWhiteSpace(content))
							{
								JToken token = JToken.Parse(content);
								if (token is JArray) { restResponse.BCRestError = DeserializeJsonFromString<List<BCRestError>>(content)?.FirstOrDefault(); }
								else
									restResponse.BCRestError = DeserializeJsonFromString<BCRestError>(content);
							}

							this.Logger?.ForContext("Scope", new BCLogTypeScope(GetType()))
								.Error("{CommerceCaption}: Operation '{OperationType}' for failed, RetryCount {RetryCount}, Exception {ExceptionMessage}",
								BCCaptions.CommerceLogCaption, request.Method, retryCount, restResponse.BCRestError.Message ?? restResponse.Content);

							retryCount++;
							Thread.Sleep(1000 * retryCount);
						}
						else throw new PXException(BCMessages.RetryLimitIsExceeded, restResponse.BCRestError.Message);
						return restResponse;
					}
				}
			}
		}

		public HttpRequestMessage MakeHttpRequest(string uri, Dictionary<string, string> urlSegments = null)
		{
			var request = new HttpRequestMessage();
			var url = Path.Combine(_options.BaseUri.ToString(), uri?.TrimStart(new char[] { '/' }));
			request.RequestUri = urlSegments != null ? new Uri(QueryHelpers.AddQueryString(url, urlSegments)) : new Uri(url);
			return request;
		}

		protected void LogError(IBCRestResponse response)
		{
			var description = "Response content: " + response.Content;

			//Log the exception and info message
			Logger.ForContext("Scope", new BCLogTypeScope(GetType()))
				.ForContext("Exception", response.BCRestError?.Message)
				.Error("{CommerceCaption}: {ResponseError}, Status Code: {StatusCode}", BCCaptions.CommerceLogCaption, description, response.StatusCode);
		}

		private T DeserializeJsonFromString<T>(string content)
		{
			if (string.IsNullOrWhiteSpace(content)) return default;
			try
			{
				JsonSerializer _serializer = JsonSerializer.Create(_serializerSettings);
				return JsonConvert.DeserializeObject<T>(content, _serializerSettings);
			}
			catch (Exception ex)
			{
				throw new Exception($"{ex.Message}. Json data content: {content}", ex);
			}
		}
	}
}
