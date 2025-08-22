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
using PX.CloudServices;
using PX.Commerce.Core;
using PX.Common;
using PX.Data;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PX.Commerce.Amazon.API.Rest.Client.Common
{
	public abstract class AmazonRestClientBase
	{
		private const string CommerceAMazonMazAttempts = "CommerceAmazonMaxApiCallAttempts";

		private static ConcurrentDictionary<string, LeakyController> _controllers;

		private readonly IHttpClientFactory _httpClientFactory;

		private readonly int _maxAttemptRecallAPI;

		private readonly JsonSerializerSettings _serializerSettings;

		protected ILogger Logger { get; }

		protected int MaximumNumberOfRepeatedRequest { get; }

		protected IRestOptions Options { get; set; }

		static AmazonRestClientBase()
		{
			_controllers = new ConcurrentDictionary<string, LeakyController>();
		}

		public AmazonRestClientBase(IHttpClientFactory httpClientFactory, JsonSerializerSettings serializerSettings, IRestOptions options, Serilog.ILogger logger)
		{
			_httpClientFactory = httpClientFactory;
			_serializerSettings = serializerSettings;
			this.Options = options;
			this.Logger = logger;
			_maxAttemptRecallAPI = WebConfig.GetInt(CommerceAMazonMazAttempts, 10);
			this.MaximumNumberOfRepeatedRequest = WebConfig.GetInt(BCConstants.COMMERCE_RETRY_COUNT, 3);
		}

		protected async Task<AmazonRestResponse> ExecuteRequestAsync(AmazonRequest request, string url, CancellationToken cancellationToken) =>
			await this.ExecuteRequestAsync<object>(request, url, cancellationToken);

		protected async Task<AmazonRestResponse<TR>> ExecuteRequestAsync<TR>(AmazonRequest request, string url, CancellationToken cancellationToken)
			where TR : class, new()
		{
			// generates a key for a current store and API, since every API has it's own limits
			// for example, it can look like this "1_orders/v0/orders/{id}"
			string key = string.Concat(this.Options.Binding.BindingID, "_", url);

			// create an instance of a controller that will allow us to get an available slot and process a request
			var leakyController = _controllers.GetOrAdd(key, _ => new LeakyController());

			var restResponse = new AmazonRestResponse<TR>();
			try
			{
				await leakyController.GetSlotAsync();
				while (request.Attempts <= _maxAttemptRecallAPI)
				{
					cancellationToken.ThrowIfCancellationRequested();

					using (HttpClient client = _httpClientFactory.CreateClient())
					{
						client.BaseAddress = new Uri(request.BaseUri);
						HttpRequestMessage httpRequestMessage = request.ToHttpRequestMessage();

						this.Logger.ForContext("Scope", new BCLogTypeScope(GetType())).Information("Execiting a request. API: {0}, Attempt {1}.", key, request.Attempts);

						using (HttpResponseMessage httpResponseMessage = await client.SendAsync(httpRequestMessage, cancellationToken))
						{
							restResponse = await this.FillRestResponse(restResponse, httpResponseMessage);

							leakyController.UpdateRate(restResponse.Headers);

							this.LogAmazonInfo(restResponse);

							if (restResponse.StatusCode == (HttpStatusCode)429)
							{
								var delayTime = leakyController.GetDelayTime(request.Attempts);

								this.Logger.ForContext("Scope", new BCLogTypeScope(GetType())).Warning("Limits for {0} have been exceeded. Awaiting time is {0} seconds. Attempt {1}.", key, delayTime.TotalSeconds, request.Attempts);

								await Task.Delay(delayTime);

								this.Logger.ForContext("Scope", new BCLogTypeScope(GetType())).Warning("Awaiting for {0} is over", key);
							}
							else if (restResponse.StatusCode == default(HttpStatusCode))
							{
								if (request.Attempts < this.MaximumNumberOfRepeatedRequest)
									await Task.Delay(1000 * request.Attempts);
								else
									throw new PXException(BCMessages.RetryLimitIsExceeded, restResponse.RestError.Message);
							}
							else
							{
								break;
							}
						}
					}
				}
			}
			finally
			{
				leakyController.ReleaseSlot();
				this.Logger.ForContext("Scope", new BCLogTypeScope(GetType())).Information("A slot is released for {0}", key);
			}

			return restResponse;
		}

		protected void LogError(IAmazonRestResponse response)
		{
			var description = $"Response content: {response.Content}";

			this.Logger.ForContext("Scope", new BCLogTypeScope(GetType()))
				.ForContext("Exception", response.RestError?.Message)
				.Error("{CommerceCaption}: {ResponseError}, Status Code: {StatusCode}", BCCaptions.CommerceLogCaption, description, response.StatusCode);
		}

		private async Task<AmazonRestResponse<TR>> FillRestResponse<TR>(AmazonRestResponse<TR> restResponse, HttpResponseMessage httpResponseMessage)
		{
			var content = await httpResponseMessage.Content.ReadAsStringAsync();
			restResponse.StatusCode = httpResponseMessage.StatusCode;
			restResponse.Headers = httpResponseMessage.Headers;
			restResponse.Content = content;

			if (httpResponseMessage.IsSuccessStatusCode)
				restResponse.Data = this.DeserializeJsonFromString<TR>(content);
			else
				restResponse.RestError = this.DeserializeError(restResponse);

			return restResponse;
		}

		private AmazonRestError DeserializeError(IAmazonRestResponse response)
		{
			var restError = this.DeserializeJsonFromString<AmazonRestError>(response.Content);
			if (restError is null)
				return new AmazonRestError { Message = HttpWorkerRequest.GetStatusDescription((int)response.StatusCode) };

			response.RestError = restError;

			return restError;
		}

		private T DeserializeJsonFromString<T>(string content)
		{
			if (string.IsNullOrWhiteSpace(content))
				return default;

			try
			{
				return JsonConvert.DeserializeObject<T>(content, _serializerSettings);
			}
			catch (Exception exception)
			{
				this.Logger.ForContext("Scope", new BCLogTypeScope(GetType()))
					.Error("{CommerceCaption}: {Description}, Content: {Content}", BCCaptions.CommerceLogCaption, "Json deserialization error", content);

				return default;
			}
		}

		private void LogAmazonInfo(AmazonRestResponse response)
		{
			var requestIDParam = response.Headers.FirstOrDefault(header => header.Key == "x-amzn-RequestId");
			var RemappedAmznRequestIdParam = response.Headers.FirstOrDefault(header => header.Key == "x-amzn-Remapped-x-amzn-RequestId");
			var xAmznRemappedDateParam = response.Headers.FirstOrDefault(header => header.Key == "x-amzn-Remapped-Date");
			var apigwParam = response.Headers.FirstOrDefault(header => header.Key == "x-amz-apigw-id");
			var traceIDParam = response.Headers.FirstOrDefault(header => header.Key == "X-Amzn-Trace-Id");
			var serverDateParam = response.Headers.FirstOrDefault(header => header.Key == "Date");

			this.Logger.ForContext("Scope", new BCLogTypeScope(GetType())).Information("{0}: {1}", RemappedAmznRequestIdParam.Key, RemappedAmznRequestIdParam.Value);
			this.Logger.ForContext("Scope", new BCLogTypeScope(GetType())).Information("{0}: {1}", xAmznRemappedDateParam.Key, xAmznRemappedDateParam.Value);
			this.Logger.ForContext("Scope", new BCLogTypeScope(GetType())).Information("{0}: {1}", apigwParam.Key, apigwParam.Value);
			this.Logger.ForContext("Scope", new BCLogTypeScope(GetType())).Information("{0}: {1}", traceIDParam.Key, traceIDParam.Value);
			this.Logger.ForContext("Scope", new BCLogTypeScope(GetType())).Information("{0}: {1}", serverDateParam.Key, serverDateParam.Value);
		}
	}
}
