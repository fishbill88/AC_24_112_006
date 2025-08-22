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

using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using PX.CloudServices;
using PX.Commerce.Amazon.API.Rest.Client.Utility;
using PX.Commerce.Core;
using PX.Common;
using PX.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon.API.Rest.Client.Common
{
	public class AmazonRestClient : AmazonRestClientBase, IAmazonRestClient
	{
		private const string AccessTokenHeaderName = "x-amz-access-token";

		private const string NextTokenParamName = "NextToken";

		private const string WWWAuthHeader = "WWW-Authenticate";

		private readonly IAmazonCloudServiceClient _cloudServiceClient;

		private readonly IEnumerable<HttpStatusCode> _httpStatusesForReauthorization;

		public AmazonRestClient(IAmazonCloudServiceClient cloudServiceClient, IHttpClientFactory httpClientFactory, JsonSerializerSettings serializerSettings, IRestOptions options, ILogger logger)
			: base(httpClientFactory, serializerSettings, options, logger)
		{
			_cloudServiceClient = cloudServiceClient;
			_httpStatusesForReauthorization = new List<HttpStatusCode> { HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden };
		}

		public async IAsyncEnumerable<T> GetAll<T, TP, TR>(AmazonRequest request, string url, CreateRestrictedDataTokenRequest createRestrictedDataToken = null, CancellationToken cancellationToken = default)
			where T : class, new()
			where TP : class, IEntityPayloadResponse<TR>, new()
			where TR : class, IEntityListResponse<T>, new()
		{
			await foreach (T entity in this.GetListOfAllEntities<T, TP, TR>(request, url, createRestrictedDataToken, cancellationToken))
				yield return entity;
		}

		private async IAsyncEnumerable<T> GetListOfAllEntities<T, TP, TR>(AmazonRequest request, string url, CreateRestrictedDataTokenRequest createRestrictedDataToken, CancellationToken cancellationToken = default)
			where T : class, new()
			where TP : class, IEntityPayloadResponse<TR>, new()
			where TR : class, IEntityListResponse<T>, new()
		{
			bool needGet = true;
			while (needGet)
			{
				cancellationToken.ThrowIfCancellationRequested();
				AmazonRestResponse<TP> response = await this.ExecuteRequestAsync<TP>(request, url, cancellationToken);

				if (response.StatusCode == HttpStatusCode.OK)
				{
					var payload = response.Data?.Payload;

					if (payload.Data is null && string.IsNullOrWhiteSpace(response.RestError.Message))
						throw new ApiException(response);

					if (payload?.Data != null)
					{
						foreach (var item in payload.Data)
							yield return item;

						if (payload.NextToken != null)
						{
							request.QueryParameters[AmazonRestClient.NextTokenParamName] = payload.NextToken;
							request.ResetAttempts();
							needGet = true;
						}
						else
						{
							needGet = false;
						}
					}
					else
						needGet = false;
				}
				else if (response.StatusCode.IsIn(_httpStatusesForReauthorization) && request.Attempts < this.MaximumNumberOfRepeatedRequest)
				{
					await ReauthorizeRequestAsync(request, response, createRestrictedDataToken);
					needGet = true;
				}
				else if (response.StatusCode == HttpStatusCode.NotFound && request.Attempts < MaximumNumberOfRepeatedRequest)
				{
					await RefreshServiceURI(request);
				}
				else
				{
					this.LogError(response);
					if (request.Attempts >= this.MaximumNumberOfRepeatedRequest)
					{
						throw new ApiException(response);
					}
				}
			}
		}

		public async Task RefreshServiceURI(AmazonRequest request)
		{
			Logger.ForContext("Scope", new BCLogTypeScope(GetType())).Error("{CommerceCaption}:Api call failed with code NotFound. Retry Count:{retry}", BCCaptions.CommerceLogCaption, request.Attempts);
			request.BaseUri = (await _cloudServiceClient.RefreshServiceUrlAsync())?.ToString();
		}

		private HttpResponseMessage GetHttpResponseFromRestResponse(AmazonRestResponse restResponse)
		{
			var httpResponseMessage = new HttpResponseMessage(restResponse.StatusCode);
			httpResponseMessage.Headers.Add(WWWAuthHeader, restResponse.Headers.Where(x => x.Key == WWWAuthHeader).FirstOrDefault().Value?.ToString());
			return httpResponseMessage;
		}

		public async Task<T> Get<T, TR>(AmazonRequest request, string url, CreateRestrictedDataTokenRequest createRestrictedDataToken = null, CancellationToken cancellationToken = default)
			where T : class, new()
			where TR : class, IEntityResponse<T>, new()
		{
			while (true)
			{
				// make the HTTP request
				var response = await this.ExecuteRequestAsync<TR>(request, url, cancellationToken);
				if (response.StatusCode == HttpStatusCode.OK)
				{
					T result = response.Data?.Payload;

					if (result is null && string.IsNullOrWhiteSpace(response.RestError.Message))
						throw new ApiException(response);

					return result;
				}
				else if (response.StatusCode.IsIn(_httpStatusesForReauthorization) && request.Attempts < this.MaximumNumberOfRepeatedRequest)
				{
					await this.ReauthorizeRequestAsync(request, response, createRestrictedDataToken);
				}
				else if (response.StatusCode == HttpStatusCode.NotFound && request.Attempts < MaximumNumberOfRepeatedRequest)
				{
					await RefreshServiceURI(request);
				}
				else
				{
					this.LogError(response);
					if (request.Attempts >= this.MaximumNumberOfRepeatedRequest)
					{
						throw new ApiException(response);
					}
				}
			}
		}

		public async Task<T> Get<T>(AmazonRequest request, string url, CreateRestrictedDataTokenRequest createRestrictedDataToken = null, CancellationToken cancellationToken = default)
			where T : class, new()
		{
			while (true)
			{
				var response = await this.ExecuteRequestAsync<T>(request, url, cancellationToken);
				if (response.StatusCode == HttpStatusCode.OK)
				{
					T result = response.Data;

					if (result is null && string.IsNullOrWhiteSpace(response.RestError.Message))
						throw new ApiException(response);

					return result;
				}
				else if (response.StatusCode.IsIn(_httpStatusesForReauthorization) && request.Attempts < this.MaximumNumberOfRepeatedRequest)
				{
					await this.ReauthorizeRequestAsync(request, response, createRestrictedDataToken);
				}
				else if (response.StatusCode == HttpStatusCode.NotFound && request.Attempts < MaximumNumberOfRepeatedRequest)
				{
					await RefreshServiceURI(request);
				}
				else
				{
					this.LogError(response);
					if (request.Attempts >= this.MaximumNumberOfRepeatedRequest)
					{
						throw new ApiException(response);
					}
				}
			}
		}

		public async Task<string> GetExternalTime(AmazonRequest request, string url, CreateRestrictedDataTokenRequest createRestrictedDataToken = null, CancellationToken cancellationToken = default)
		{
			while (true)
			{
				var response = await this.ExecuteRequestAsync(request, url, cancellationToken);

				if (response.StatusCode == HttpStatusCode.OK)
				{
					return response.Headers.FirstOrDefault(header => header.Key == "Date").Value.ToString();
				}
				else if (response.StatusCode.IsIn(_httpStatusesForReauthorization) && request.Attempts < MaximumNumberOfRepeatedRequest)
				{
					await ReauthorizeRequestAsync(request, response, createRestrictedDataToken);
				}
				else if (response.StatusCode == HttpStatusCode.NotFound && request.Attempts < MaximumNumberOfRepeatedRequest)
				{
					await RefreshServiceURI(request);
				}
				else
				{
					this.LogError(response);
					if (request.Attempts >= MaximumNumberOfRepeatedRequest)
					{
						throw new ApiException(response);
					}
				}
			}
		}

		public async Task ReauthorizeRequestAsync(AmazonRequest request, AmazonRestResponse restResponse, CreateRestrictedDataTokenRequest createRestrictedDataToken, CancellationToken cancellationToken = default)
		{
			Logger.ForContext("Scope", new BCLogTypeScope(GetType())).Error("{CommerceCaption}:Api call failed with Unauthorized Status code. Retry Count:{retry}", BCCaptions.CommerceLogCaption, request.Attempts);

			System.Net.Http.Headers.AuthenticationHeaderValue cloudAuthentication;

			if (restResponse.Headers.Any(x => x.Key.Equals(WWWAuthHeader, StringComparison.OrdinalIgnoreCase)))
			{
				HttpResponseMessage httpResponseMessage = GetHttpResponseFromRestResponse(restResponse);
				cloudAuthentication = (await _cloudServiceClient.GetFreshAuthorizationHeaderAsync(httpResponseMessage));
			}
			else
				cloudAuthentication = (await _cloudServiceClient.GetServiceUrlAndHeader()).AuthenticationHeaderValue;

			request.Headers[HeaderNames.Authorization] = cloudAuthentication?.ToString();

			string accessToken = await GetAccessToken(createRestrictedDataToken, true, cancellationToken);
			request.Headers[AccessTokenHeaderName] = accessToken;
		}

		public async Task<AmazonRequest> PrepareRequest(
			string path,
			HttpMethod method,
			bool withAuthentication = true,
			List<(string, string)> queryParams = null,
			object postBody = null,
			Dictionary<string, string> pathParams = null,
			string contentType = ContentType.Json,
			CreateRestrictedDataTokenRequest createRestrictedDataToken = null,
			string serviceEndpoint = "spapi",
			CancellationToken cancellationToken = default)
		{
			// add path parameter, if any
			if (pathParams != null)
			{
				foreach (var pathParam in pathParams)
					path = path.Replace("{" + pathParam.Key + "}", pathParam.Value);
			}

			var request = new AmazonRequest();
			request.Method = method;

			if (withAuthentication)
			{
				// Get Cloud Service Url and authorization header
				var service = await _cloudServiceClient.GetServiceUrlAndHeader();
				string completePath = serviceEndpoint == "spapi" ?
					string.Format("{0}/{1}/{2}", serviceEndpoint, this.Options.BindingAmazon.Region, path.Trim('/'))
					: string.Format("{0}/{1}", serviceEndpoint, this.Options.BindingAmazon.Region);

				request.RequestUri = completePath;
				request.Headers[HeaderNames.Accept] = ContentType.Json;

				string accessToken = await this.GetAccessToken(createRestrictedDataToken, false, cancellationToken);

				request.Headers[AmazonRestClient.AccessTokenHeaderName] = accessToken;

				//Add authorization  header, needed for cloud service
				request.Headers[HeaderNames.Authorization] = service.AuthenticationHeaderValue.ToString();

				//Base Url of clould service
				request.BaseUri = service.Uri.OriginalString;
			}
			else
			{
				request.BaseUri = path;
			}

			// add query parameter, if any
			if (queryParams != null)
			{
				foreach ((string Name, string Value) queryParameter in queryParams)
					request.QueryParameters[queryParameter.Name] = queryParameter.Value;
			}

			if (postBody != null) // http body (model or byte[]) parameter
			{
				request.Content = postBody;
				request.ContentType = contentType;
			}

			Logger.ForContext("Scope", new BCLogTypeScope(GetType())).Information("{CommerceCaption}: Request prepared for {path}", BCCaptions.CommerceLogCaption, path);
			return request;
		}

		public async Task<T> Post<T>(AmazonRequest request, string url, CreateRestrictedDataTokenRequest createRestrictedDataToken = null, CancellationToken cancellationToken = default)
			 where T : class, new()
		{
			while (true)
			{
				var response = await this.ExecuteRequestAsync<T>(request, url, cancellationToken);
				if (response.StatusCode == HttpStatusCode.OK
					|| response.StatusCode == HttpStatusCode.Accepted
					|| response.StatusCode == HttpStatusCode.Created)
				{
					T result = response.Data;

					if (result is null && string.IsNullOrWhiteSpace(response.RestError.Message))
						throw new ApiException(response);

					return result;
				}
				else if (response.StatusCode.IsIn(_httpStatusesForReauthorization) && request.Attempts < this.MaximumNumberOfRepeatedRequest)
				{
					await this.ReauthorizeRequestAsync(request, response, createRestrictedDataToken);
				}
				else if (response.StatusCode == HttpStatusCode.NotFound && request.Attempts < MaximumNumberOfRepeatedRequest)
				{
					await RefreshServiceURI(request);
				}
				else
				{
					this.LogError(response);
					if (request.Attempts >= this.MaximumNumberOfRepeatedRequest)
					{
						throw new ApiException(response);
					}
				}
			}
		}

		public async Task Put(AmazonRequest request, string url, CancellationToken cancellationToken = default)
		{
			while (true)
			{
				var response = await this.ExecuteRequestAsync(request, url, cancellationToken);

				if (response.StatusCode == HttpStatusCode.OK) return;
				else if (response.StatusCode.IsIn(_httpStatusesForReauthorization) && request.Attempts < this.MaximumNumberOfRepeatedRequest)
				{
					await this.ReauthorizeRequestAsync(request, response, null);
				}
				else if (response.StatusCode == HttpStatusCode.NotFound && request.Attempts < MaximumNumberOfRepeatedRequest)
				{
					await RefreshServiceURI(request);
				}
				else
				{
					this.LogError(response);
					if (request.Attempts >= this.MaximumNumberOfRepeatedRequest)
					{
						throw new ApiException(response);
					}
				}
			}
		}

		private async Task<string> GetAccessToken(CreateRestrictedDataTokenRequest createRestrictedDataToken, bool forceNewToken, CancellationToken cancellationToken)
		{
			string accessToken = null;

			if (createRestrictedDataToken != null)
			{
				Logger.ForContext("Scope", new BCLogTypeScope(GetType())).Information("{CommerceCaption}: Adding RDT.", BCCaptions.CommerceLogCaption);
				accessToken = await TokenManager.GetRestrictedDataToken(this.Options.Binding, Logger, forceNewToken, GetRDTToken, createRestrictedDataToken, cancellationToken);
			}

			if (accessToken == null)
			{
				Logger.ForContext("Scope", new BCLogTypeScope(GetType())).Information("{CommerceCaption}: Adding an access token.", BCCaptions.CommerceLogCaption);
				accessToken = await TokenManager.GetAccessToken(this.Options.BindingAmazon, this.Options.Binding, _cloudServiceClient.GetAccessToken, Logger, forceNewToken, cancellationToken);
			}

			return accessToken;
		}

		private async Task<CreateRestrictedDataTokenResponse> GetRDTToken(CreateRestrictedDataTokenRequest tokenRequest, CancellationToken cancellationToken)
		{
			string rdtPath = "tokens/2021-03-01/restrictedDataToken";

			var request = await this.PrepareRequest(rdtPath, HttpMethod.Post, postBody: tokenRequest, serviceEndpoint: "rdt", cancellationToken: cancellationToken);
			while (true)
			{
				var response = await this.ExecuteRequestAsync<CreateRestrictedDataTokenResponse>(request, rdtPath, cancellationToken);
				if (response.StatusCode == HttpStatusCode.OK)
					return response.Data;
				else if (response.StatusCode.IsIn(_httpStatusesForReauthorization) && request.Attempts < this.MaximumNumberOfRepeatedRequest)
				{
					await this.ReauthorizeRequestAsync(request, response, tokenRequest);
				}
				else if (response.StatusCode == HttpStatusCode.NotFound && request.Attempts < MaximumNumberOfRepeatedRequest)
				{
					await RefreshServiceURI(request);
				}
				else
				{
					this.LogError(response);
					if (request.Attempts >= this.MaximumNumberOfRepeatedRequest)
					{
						// when BadRequest code is returned it means API credentials are not authorized to obtain restricted data token
						if (response.StatusCode == HttpStatusCode.BadRequest)
							throw new RestrictedDataTokenNotAuthorizedException(response.RestError.Message);
						else
							throw new ApiException(response);
					}
				}
			}
		}
	}
}
