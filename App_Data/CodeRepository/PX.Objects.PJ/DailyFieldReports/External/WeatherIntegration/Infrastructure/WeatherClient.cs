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

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using PX.Common;
using PX.Data;
using PX.Objects.PJ.DailyFieldReports.External.WeatherIntegration.Descriptor;
using PX.Objects.PJ.DailyFieldReports.External.WeatherIntegration.Exceptions;
using PX.Objects.PJ.DailyFieldReports.PJ.DAC;

namespace PX.Objects.PJ.DailyFieldReports.External.WeatherIntegration.Infrastructure
{
	public class WeatherClient
	{
		private readonly Dictionary<string, string> _queryParameters = new Dictionary<string, string>();

		private readonly string _baseUrl;

		private readonly IHttpClientFactory _httpClientFactory;

		private readonly PXGraph _graph;

		private readonly bool _isLogEnabled;

		public WeatherClient(
			PXGraph graph,
			string baseUrl,
			IHttpClientFactory httpClientFactory,
			bool? isLogEnabled
		)
		{
			_graph = graph;
			_baseUrl = baseUrl;
			_httpClientFactory = httpClientFactory;
			_isLogEnabled = isLogEnabled == true;
		}

		public async Task<TModel> ExecuteAsync<TModel>(CancellationToken cancellationToken) where TModel : new()
        {
			if (_isLogEnabled) return await ExecuteWithLogAsync<TModel>(cancellationToken);

			var response = await SendRequestAsync(cancellationToken);
			ValidateResponse(response);

			return DeserializeResponse<TModel>(response);
		}

        public WeatherClient AddQueryParameter(string name, object value)
        {
            if (value != null)
            {
				_queryParameters.Add(name, value.ToString());
            }

            return this;
        }

		private async Task<TModel> ExecuteWithLogAsync<TModel>(CancellationToken cancellationToken)
		{
			var cache = _graph.Caches<WeatherProcessingLog>();
			var weatherProcessingLog = InsertWeatherProcessingLog(cache);
			WeatherResponse response = new WeatherResponse();

			try
			{
				response = await SendRequestAsync(cancellationToken);
				ValidateResponse(response);

				return DeserializeResponse<TModel>(response);
			}
			catch (NoContentException e)
			{
				response.Content = e.Message;
				throw;
			}
			finally
			{
				UpdateWeatherProcessingLog(weatherProcessingLog, response);
				cache.Persist(weatherProcessingLog, PXDBOperation.Insert);
				cache.Clear();
			}
		}

		private async Task<WeatherResponse> SendRequestAsync(CancellationToken cancellationToken)
		{
			var client = _httpClientFactory.CreateClient();
			var requestUri = BuildRequestUri();

			var result = await client.GetAsync(requestUri, cancellationToken);
			var content = await result.Content.ReadAsStringAsync();

			return new WeatherResponse
			{
				StatusCode = result.StatusCode,
				Content = content,
				ResponseUri = result.RequestMessage.RequestUri
			};
		}

        private string BuildRequestUri()
        {
			var query = HttpUtility.ParseQueryString(string.Empty);

			foreach (var kvp in _queryParameters)
			{
				query[kvp.Key] = kvp.Value;
			}

			return $"{_baseUrl}?{query}";
		}

        private TModel DeserializeResponse<TModel>(WeatherResponse response)
        {
	        using (var stringReader = new System.IO.StringReader(response.Content))
	        {
		        using (var jsonTextReader = new JsonTextReader(stringReader))
		        {
			        return new JsonSerializer().Deserialize<TModel>(jsonTextReader);
				}
	        }
        }

        private void ValidateResponse(WeatherResponse response)
        {
			switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    throw new WeatherApiKeyIsNotCorrectException();
                case HttpStatusCode.BadRequest:
                    throw new NothingToGeocodeException();
                case HttpStatusCode.NotFound:
                    throw new CountryOrCityNotFoundException();
                case HttpStatusCode.Forbidden:
                    throw new WeatherApiKeyIsNotCorrectException();
                case HttpStatusCode.ServiceUnavailable:
                    throw new NumberOfRequestsHasBeenExceededException();
                case HttpStatusCode.NoContent:
                    throw new NoContentException();
                case (HttpStatusCode) 429:
                    throw new NumberOfRequestsHasBeenExceededException();
            }

			if (response.Content == WeatherIntegrationConstants.NoContentResponse)
			{
				throw new NoContentException();
			}
		}

		private static WeatherProcessingLog InsertWeatherProcessingLog(PXCache cache)
		{
			var weatherProcessingLog = (WeatherProcessingLog)cache.Insert();
			weatherProcessingLog.RequestTime = PXTimeZoneInfo.Now;

			return weatherProcessingLog;
		}

		private void UpdateWeatherProcessingLog(WeatherProcessingLog weatherProcessingLog, WeatherResponse response)
		{
			weatherProcessingLog.ResponseTime = PXTimeZoneInfo.Now;
			weatherProcessingLog.RequestBody = response.ResponseUri.AbsoluteUri;
			weatherProcessingLog.ResponseBody = response.Content;
			weatherProcessingLog.RequestStatusIcon = GetRequestStatusIcon(response);
		}

		private string GetRequestStatusIcon(WeatherResponse response)
		{
			return response.StatusCode == HttpStatusCode.OK
				? WeatherIntegrationConstants.RequestStatusIcons.RequestStatusSuccessIcon
				: WeatherIntegrationConstants.RequestStatusIcons.RequestStatusFailIcon;
		}
	}
}
