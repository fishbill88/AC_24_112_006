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

namespace PX.Commerce.Shopify.API.REST
{
	public abstract class SPRestClientBase
	{
		private const string HEADER_LIMIT = "X-Shopify-Shop-Api-Call-Limit";
		private readonly string apiIdentifyId;
		private readonly int maxAttemptRecallAPI = WebConfig.GetInt(ShopifyConstants.CommerceShopifyMaxAttempts, 1000);
		private readonly int delayApiCallTime = WebConfig.GetInt(ShopifyConstants.CommerceShopifyDelayTimeIfFailed, 500); //500ms
		protected readonly int commerceRetryCount = WebConfig.GetInt(BCConstants.COMMERCE_RETRY_COUNT, 3);
		protected int retryCount;
		protected string _baseUrl;
		IHttpClientFactory _clientFactory = null;
		public Serilog.ILogger Logger { get; set; } = null;
		private JsonSerializerSettings _serializerSettings;
		protected SPRestClientBase(IHttpClientFactory clientFactory, JsonSerializerSettings serializer, IRestOptions options, Serilog.ILogger logger)
		{
			apiIdentifyId = options.ApiToken;
			_clientFactory = clientFactory;
			_serializerSettings = serializer;
			_baseUrl = options.BaseUri;

			Logger = logger;
		}
		public HttpRequestMessage MakeHttpRequest(string uri, Dictionary<string, string> urlSegments = null)
		{
			var request = new HttpRequestMessage();

			request.RequestUri = urlSegments != null ? new Uri(QueryHelpers.AddQueryString(Path.Combine(_baseUrl.ToString() + uri), urlSegments))
			: new Uri(Path.Combine(_baseUrl.ToString() + uri));
			return request;
		}

		/// <summary>
		/// ExecuteRequest with httpclient and return Bcrestresponse
		/// </summary>
		/// <param name="request"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		protected async Task<IBCRestResponse> ExecuteRequest(HttpRequestMessage originalRequest, CancellationToken cancellationToken = default)
		{
			var requestRateController = RequestRateControllers.GetController(apiIdentifyId);
			if (requestRateController != null)
			{
				int attemptRecallAPI = 1;
				while (attemptRecallAPI <= maxAttemptRecallAPI)
				{
					IBCRestResponse restResponse = new BCRestResponse();
					cancellationToken.ThrowIfCancellationRequested();
					requestRateController.GrantAccess();
					try
					{
						using (var client = _clientFactory.CreateClient())
						{
							HttpRequestMessage request = await originalRequest.CloneAsync();
							request.Headers.AddOrReplace(ShopifyConstants.XShopifyAccessToken, apiIdentifyId);
							client.BaseAddress = new Uri(_baseUrl);
							using (var response = await client.SendAsync(request, cancellationToken))
							{
								requestRateController.UpdateController(response.Headers?.FirstOrDefault(x => string.Equals(x.Key, HEADER_LIMIT, StringComparison.OrdinalIgnoreCase)).Value);
								restResponse.StatusCode = response.StatusCode;
								restResponse.Headers = response.Headers;
								var content = await response.Content.ReadAsStringAsync();
								restResponse.Content = content;
								restResponse.IsSuccessStatusCode = response.IsSuccessStatusCode;
								if (response.IsSuccessStatusCode)
								{
									return restResponse;
								}
								else
								{
									restResponse.BCRestError = DeserializeJsonFromString<BCRestError>(content);
									CheckResponse(response, restResponse.BCRestError.Message);
								}
							}
						}
						return restResponse;
					}
					catch (RestShopifyApiCallLimitException ex)
					{
						attemptRecallAPI++;
						await Task.Delay(delayApiCallTime);
					}
				}
			}
			throw new Exception(ShopifyMessages.TooManyApiCalls);
		}

		/// <summary>
		/// Execute request with httpclient and returns deserialized object in BC restresponse
		/// </summary>
		/// <typeparam name="TR"></typeparam>
		/// <param name="request"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// <exception cref="PXException"></exception>
		protected async Task<BCRestResponse<TR>> ExecuteRequest<TR>(HttpRequestMessage request, CancellationToken cancellationToken = default) where TR : class, new()
		{
			var response = await ExecuteRequest(request, cancellationToken);
			BCRestResponse<TR> restResponse = BCRestResponse<TR>.FromResponse(response);
			if (response.IsSuccessStatusCode)
			{
				try
				{
					restResponse.Data = DeserializeJsonFromString<TR>(restResponse.Content);
				}
				catch
				{
					restResponse.Data = default(TR);
				}
			}

			return restResponse;
		}
		protected void LogError(IBCRestResponse response)
		{
			//Set up the information message with the URL, the status code, and the parameters.
			var description = "Response content: " + response.Content;

			//Log the exception and info message
			Logger?.ForContext("Scope", new BCLogTypeScope(GetType()))
				.Error(new RestException(response), "{CommerceCaption}: {ResponseError}", BCCaptions.CommerceLogCaption, description);
		}

		protected void CheckResponse(HttpResponseMessage response, string message)
		{
			if (!string.IsNullOrEmpty(response?.StatusCode.ToString()) && int.TryParse(response?.StatusCode.ToString(), out var intCode) && intCode == 429)
			{

				throw new RestShopifyApiCallLimitException(message);
			}
			else if (response?.StatusCode == default(System.Net.HttpStatusCode))
			{
				if (retryCount < commerceRetryCount)
				{
					retryCount++;
					Thread.Sleep(1000 * retryCount);
				}

				else
				{
					throw new PXException(BCMessages.RetryLimitIsExceeded, message);
				}
			}
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
