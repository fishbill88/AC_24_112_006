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

using Autofac;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PX.CloudServices;
using PX.CloudServices.Auth;
using PX.CloudServices.Diagnostic;
using PX.CloudServices.Discovery;
using PX.Commerce.Amazon.API.Rest;
using PX.Commerce.Amazon.API.Rest.Client.Utility;
using PX.Commerce.Core;
using PX.Data;
using PX.Objects.CS;
using PX.Owin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Compilation;

namespace PX.Commerce.Amazon
{
	internal class AmazonCloudServiceClient : IAmazonCloudServiceClient, ICloudServiceClientWithDiagnostics
	{
		internal const string DiscoveryServiceName = "amazon-spapi-auth";

		// TODO: Add logging
		private readonly ILogger _logger;
		private readonly ICloudServiceClient _cloudServiceClient;

		string ICloudServiceClientWithDiagnostics.ServiceName => DiscoveryServiceName;

		string ICloudServiceClientWithDiagnostics.Description => PXMessages.LocalizeNoPrefix(AmazonMessages.DescriptionForDiagnostic);

		bool ICloudServiceClientWithDiagnostics.IsActive => PXAccess.FeatureInstalled(typeof(FeaturesSet.amazonIntegration).FullName);

		string ICloudServiceClientWithDiagnostics.HowToActivate => PXMessages.LocalizeNoPrefix(AmazonMessages.ActivateFeatureForDiagnostic);

		public AmazonCloudServiceClient(ILogger logger, ICloudServiceClientFactory cloudServiceClientFactory)
		{
			_logger = logger;
			_cloudServiceClient = cloudServiceClientFactory.CreateClient(typeof(AmazonCloudServiceClient), DiscoveryServiceName);
		}

		public async Task<string> Authorize(AmazonStateParameters cloudServiceParameters)
		{
			ServiceURl service = await GetServiceUrlAndHeader();
			Uri baseUrl = service.Uri;
			AuthenticationHeaderValue authorization = service.AuthenticationHeaderValue;


			using (var httpClient = _cloudServiceClient.CreateHttpClient())
			{
				bool lastCall = false;
				while (true)
				{
					var serviceResponse = await httpClient.SendAsync(new HttpRequestMessage
					{
						Method = HttpMethod.Get,
						RequestUri = new Uri(new RequestUrl(new Uri(baseUrl, "oauth2/authorize").ToString())
						.CreateAuthorizeUrl(clientId: null,
							responseType: OidcConstants.ResponseTypes.Code,
							scope: cloudServiceParameters.MarketplaceURL,
							redirectUri: cloudServiceParameters.RedirectURI,
							state: cloudServiceParameters.State
						   )),
						Headers = { Authorization = authorization },
					});

					switch (serviceResponse.StatusCode)
					{
						case HttpStatusCode.Redirect:
							return serviceResponse.Headers.Location.ToString();
						case HttpStatusCode.Forbidden:
						case HttpStatusCode.Unauthorized:
							if (lastCall) throw new PXException(AmazonMessages.NoValidToken);
							authorization = await _cloudServiceClient.GetFreshAuthorizationHeaderAsync(serviceResponse);
							lastCall = true;
							break;
						case HttpStatusCode.NotFound:
							baseUrl = await _cloudServiceClient.RefreshServiceUrlAsync();
							lastCall = false;
							break;
						default:
							await ThrowStoreAuthorizationException(serviceResponse);
							break;
					}
				}
			}
		}

		public async Task<TokenResponse> ProcessAuthorizationCode(string code, string seller, int? bindingId)
		{
			var redirectUri = AmazonAuthenticationHandler.ReturnUrl;
			_logger.Information("RedirectUri: {0}", redirectUri);

			ServiceURl service = await GetServiceUrlAndHeader();
			Uri baseUrl = service.Uri;
			AuthenticationHeaderValue authorization = service.AuthenticationHeaderValue;

			using (var httpClient = _cloudServiceClient.CreateHttpClient())
			{
				bool lastCall = false;
				while (true)
				{
					var serviceResponse = await httpClient.RequestAuthorizationCodeTokenAsync(
					new AuthorizationCodeTokenRequest()
					{
						Address = new Uri(baseUrl, "oauth2/token").ToString(),
						Headers = { Authorization = authorization },
						Code = code,
						RedirectUri = redirectUri
					});

					switch (serviceResponse.HttpResponse.StatusCode)
					{
						case HttpStatusCode.OK:
							return serviceResponse;
						case HttpStatusCode.Forbidden:
						case HttpStatusCode.Unauthorized:
							if (lastCall) throw new PXException(AmazonMessages.NoValidToken);
							authorization = await _cloudServiceClient.GetFreshAuthorizationHeaderAsync(serviceResponse.HttpResponse);
							lastCall = true;
							break;
						case HttpStatusCode.NotFound:
							baseUrl = await _cloudServiceClient.RefreshServiceUrlAsync();
							lastCall = false;
							break;
						default:
							await ThrowStoreAuthorizationException(serviceResponse.HttpResponse);
							break;
					}
				}
			}
		}

        public async Task<ServiceURl> GetServiceUrlAndHeader()
        {
			_logger
                .ForContext("Scope", new BCLogTypeScope(GetType()))
                .Information("{CommerceCaption}: Getting Cloud Service Url and authorization header", BCCaptions.CommerceLogCaption);

            var baseUrl = await _cloudServiceClient.GetServiceUrlAsync();
            var authorization = await _cloudServiceClient.GetAuthorizationHeaderAsync();
            return new ServiceURl { Uri = baseUrl, AuthenticationHeaderValue = authorization };
        }

		public async Task<Token> GetAccessToken(BCBindingAmazon bindingAmazon, CancellationToken cancellationToken = default)
		{
			return await GetAccessTokenImpl(bindingAmazon.RefreshToken, cancellationToken);
		}

        private async Task<Token> GetAccessTokenImpl(string refreshToken, CancellationToken cancellationToken = default)
        {
            ServiceURl service = await GetServiceUrlAndHeader();
			_logger
                .ForContext("Scope", new BCLogTypeScope(GetType()))
                .Information("{CommerceLogCaption}: Requesting an access token", BCCaptions.CommerceLogCaption);

			Uri baseUrl= service.Uri;
			AuthenticationHeaderValue authorization = service.AuthenticationHeaderValue;
			using (var httpClient = _cloudServiceClient.CreateHttpClient())
			{
				bool lastCall = false;
				while (true)
				{
					TokenResponse tokenResult = await httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
					{
						Address = new Uri(baseUrl, "oauth2/token").ToString(),
						RefreshToken = refreshToken,
						Headers = { Authorization = authorization },
					}, cancellationToken);

					switch (tokenResult.HttpResponse.StatusCode)
					{
						case HttpStatusCode.OK:
							return new Token(tokenResult.AccessToken, DateTimeOffset.UtcNow.AddSeconds(tokenResult.ExpiresIn), tokenResult.ExpiresIn, tokenResult.RefreshToken);
						case HttpStatusCode.Forbidden:
						case HttpStatusCode.Unauthorized:
							if (lastCall) throw new PXException(AmazonMessages.NoValidToken);
							authorization = await _cloudServiceClient.GetFreshAuthorizationHeaderAsync(tokenResult.HttpResponse);
							lastCall = true;
							break;
						case HttpStatusCode.NotFound:
							baseUrl = await _cloudServiceClient.RefreshServiceUrlAsync();
							lastCall = false;
							break;
						default:
							if (tokenResult.IsError)
							{
								string errorMessage = string.Format("Error: {0}, ErrorDescription: {1}, HttpStatusCode: {2}, RequestUri: {3}", tokenResult.Error, tokenResult.ErrorDescription, tokenResult.HttpStatusCode, tokenResult.HttpResponse?.RequestMessage?.RequestUri);
								throw new AccessTokenResponseException(errorMessage);
							}
							break;
					}
				}
			}
		}

		Task<IEnumerable<(bool Success, string Message)>> ICloudServiceClientWithDiagnostics.TestAsync() =>
			Task.FromResult(Enumerable.Empty<(bool, string)>());

		public async Task<Uri> RefreshServiceUrlAsync()
		{
			_logger
				.ForContext("Scope", new BCLogTypeScope(GetType()))
				.Information("{CommerceLogCaption}: Get Refresh Service Url", BCCaptions.CommerceLogCaption);
			return await _cloudServiceClient.RefreshServiceUrlAsync();
		}

		public async Task<AuthenticationHeaderValue> GetFreshAuthorizationHeaderAsync(HttpResponseMessage HttpResponseMessage)
		{
			_logger
				.ForContext("Scope", new BCLogTypeScope(GetType()))
				.Information("{CommerceLogCaption}: Get Fresh Authorization Header", BCCaptions.CommerceLogCaption);
			return await _cloudServiceClient.GetFreshAuthorizationHeaderAsync(HttpResponseMessage);
		}

		private async Task ThrowStoreAuthorizationException(HttpResponseMessage serviceResponse)
		{
			var response = await serviceResponse.Content.ReadAsStringAsync();

			var requestID = serviceResponse.Headers.GetValueFromHeaders("x-amzn-RequestId");
			var errorType = serviceResponse.Headers.GetValueFromHeaders("x-amzn-ErrorType");
			var apigw = serviceResponse.Headers.GetValueFromHeaders("x-amz-apigw-id");
			var traceID = serviceResponse.Headers.GetValueFromHeaders("X-Amzn-Trace-Id");
			var serverDate = serviceResponse.Headers.GetValueFromHeaders("Date");

			_logger
				.ForContext("Scope", new BCLogTypeScope(GetType()))
				.Error($"{BCCaptions.CommerceLogCaption}: Authorization Error :{serviceResponse.RequestMessage.RequestUri}," +
				 $" Status code: {serviceResponse.StatusCode}, ReguestID: {requestID}, Error Type: {errorType}, API Gateway: {apigw}," +
				 $" TraceID: {traceID}, Server Date: {serverDate}");

			throw new PXInvalidOperationException(AmazonMessages.AuthorizationError);
		}
	}
}
