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

using System;
using System.Collections.Generic;
using System.Linq;
using PX.Objects.PJ.DailyFieldReports.External.WeatherIntegration.Infrastructure;
using PX.Objects.PJ.DailyFieldReports.External.WeatherIntegration.Models.AccuWeatherModels;
using PX.Objects.PJ.DailyFieldReports.PJ.DAC;
using PX.Data;
using PX.CS.Contracts.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace PX.Objects.PJ.DailyFieldReports.External.WeatherIntegration.Services
{
    public class AccuWeatherIntegrationStrategy : WeatherIntegrationBaseStrategy<List<AccuWeatherResponse>>
    {
        private const string IconUrl = "https://developer.accuweather.com/sites/default/files/{0}-s.png";
		private const string ApiUrl = "http://dataservice.accuweather.com/currentconditions/v1/";
		private const string LocationsUrl = "http://dataservice.accuweather.com/locations/v1/cities/geoposition/search";

		public AccuWeatherIntegrationStrategy(IHttpClientFactory factory, string locationKey): base(factory)
        {
            BaseUrl = ApiUrl + locationKey;
		}

        protected override string BaseUrl { get; }

		public override void AddRequiredParameters(WeatherIntegrationSetup weatherIntegrationSetup)
        {
			WeatherClient.AddQueryParameter("apikey", weatherIntegrationSetup.WeatherApiKey);
			WeatherClient.AddQueryParameter("details", "True");
        }

        public override void AddCityAndCountryParameters(IAddressLocation location)
        {
        }

        public override void AddZipCodeAndCountryParameters(IAddressLocation location)
        {
        }

        public override void AddGeographicLocationParameters(IAddressLocation location)
        {
        }

		public static async Task<string> GetLocationKeyAsync(
			IHttpClientFactory factory,
			IAddressLocation location,
			PXGraph graph,
			WeatherIntegrationSetup weatherIntegrationSetup,
			CancellationToken cancellationToken
		)
		{
			var weatherClient = new WeatherClient(graph, LocationsUrl, factory, weatherIntegrationSetup.IsWeatherProcessingLogEnabled);

			weatherClient.AddQueryParameter("apikey", weatherIntegrationSetup.WeatherApiKey);
			weatherClient.AddQueryParameter("q", $"{location.Latitude}, {location.Longitude}");

			var result = await weatherClient.ExecuteAsync<AccuWeatherLocationKey>(cancellationToken);
			return result.Key;
		}

		protected override DailyFieldReportWeather CreateDailyFieldReportWeather(List<AccuWeatherResponse> model)
        {
            var response = model.First();
            var iconNumber = GetIconNumber(response.Icon);
            return new DailyFieldReportWeather
            {
                Icon = string.Format(IconUrl, iconNumber),
                TimeObserved = DateTimeOffset.FromUnixTimeSeconds(response.TimeObserved).ToUniversalTime().DateTime,
                TemperatureLevel = response.Temperature.Imperial.Value,
                WindSpeed = response.Wind.Speed.Imperial.Value,
                Cloudiness = response.Cloudiness,
                Humidity = response.Humidity,
                PrecipitationAmount = response.Precipitation.PastHour.Imperial.Value,
                LocationCondition = response.SiteCondition
            };
        }

        private static string GetIconNumber(string icon)
        {
            return icon.Length == 1
                ? $"0{icon}"
                : icon;
        }
    }
}
