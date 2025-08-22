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
using PX.Objects.PJ.DailyFieldReports.External.WeatherIntegration.Descriptor;
using PX.Objects.PJ.DailyFieldReports.PJ.DAC;
using PX.Objects.PJ.DailyFieldReports.PJ.Descriptor.Attributes;
using PX.Api;
using PX.Data;
using PX.CS.Contracts.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace PX.Objects.PJ.DailyFieldReports.External.WeatherIntegration.Services
{
    public class WeatherIntegrationService : IWeatherIntegrationService
    {
		public IWeatherIntegrationStrategy WeatherIntegrationStrategy;

		private readonly PXGraph _graph;

        private readonly PXCache _cache;

		private readonly PXCache<WeatherIntegrationSetup> _weatherIntegrationSetupCache;

		private readonly IHttpClientFactory _httpClientFactory;

		private IAddressLocation _location;

		private WeatherIntegrationSetup WeatherIntegrationSetup => (WeatherIntegrationSetup)_weatherIntegrationSetupCache.Current;

		public WeatherIntegrationService(PXGraph graph, IHttpClientFactory factory)
		{
			_graph = graph;
			_weatherIntegrationSetupCache = graph.Caches<WeatherIntegrationSetup>();
			_cache = graph.Caches<DailyFieldReport>();
			_httpClientFactory = factory;
		}

        public async Task<DailyFieldReportWeather> GetDailyFieldReportWeatherAsync(CancellationToken cancellationToken)
        {
            _location = (IAddressLocation) _cache.Current;
            ValidateWeatherIntegrationSetup();
			await InitializeWeatherIntegrationStrategyAsync(cancellationToken);

			WeatherIntegrationStrategy.InitializeRequest(_graph, WeatherIntegrationSetup);
            AddQueryParameters();

            return await WeatherIntegrationStrategy.GetDailyFieldReportWeatherAsync(cancellationToken);
        }

		public async Task TestConnectionAsync(CancellationToken cancellationToken)
		{
			_location = new DailyFieldReport
			{
				Latitude = 51.510000M,
				Longitude = -0.130000M,
			};
			await InitializeWeatherIntegrationStrategyAsync(cancellationToken);

			WeatherIntegrationStrategy.InitializeRequest(_graph, WeatherIntegrationSetup);
			AddQueryParameters();

			await WeatherIntegrationStrategy.TestConnectionAsync(cancellationToken);
		}

		private async Task InitializeWeatherIntegrationStrategyAsync(CancellationToken cancellationToken)
		{
			if (WeatherIntegrationStrategy != null) return;

			switch (WeatherIntegrationSetup.WeatherApiService)
			{
				case WeatherApiService.OpenWeatherMap:
					WeatherIntegrationStrategy = new OpenWeatherMapIntegrationStrategy(_httpClientFactory);
					break;
				case WeatherApiService.WeatherBit:
					WeatherIntegrationStrategy = new WeatherBitIntegrationStrategy(_httpClientFactory);
					break;
				case WeatherApiService.AccuWeather:
					WeatherIntegrationStrategy = await CreateAccuWeatherIntegrationStrategyAsync(cancellationToken);
					break;
				default:
					throw new Exception(WeatherIntegrationMessages
						.ToLoadWeatherConditionsMustBeSpecifiedOnWeatherIntegrationSettingsTab);
			}
		}

		private void AddQueryParameters()
        {
            switch (WeatherIntegrationSetup.RequestParametersType)
            {
                case WeatherRequestParameters.CityAndCountry:
                    ValidateFields(nameof(_location.City), nameof(_location.CountryID));
					WeatherIntegrationStrategy.AddCityAndCountryParameters(_location);
                    break;
                case WeatherRequestParameters.ZipCodeAndCountry:
                    ValidateFields(nameof(_location.PostalCode), nameof(_location.CountryID));
					WeatherIntegrationStrategy.AddZipCodeAndCountryParameters(_location);
                    break;
                case WeatherRequestParameters.GeographicLocation:
                    ValidateFields(nameof(_location.Latitude), nameof(_location.Longitude));
					WeatherIntegrationStrategy.AddGeographicLocationParameters(_location);
                    break;
            }
			WeatherIntegrationStrategy.AddRequiredParameters(WeatherIntegrationSetup);
        }

        private void ValidateWeatherIntegrationSetup()
        {
            if (WeatherIntegrationSetup.RequestParametersType.IsNullOrEmpty() ||
				WeatherIntegrationSetup.WeatherApiKey.IsNullOrEmpty())
            {
                throw new Exception(WeatherIntegrationMessages
                    .ToLoadWeatherConditionsMustBeSpecifiedOnWeatherIntegrationSettingsTab);
            }
        }

        private void ValidateFields(string fieldName1, string fieldName2)
        {
            if (_cache.GetValue(_location, fieldName1) == null ||
				_cache.GetValue(_location, fieldName2) == null)
            {
                var message = string.Format(WeatherIntegrationMessages
                        .ToLoadWeatherConditionsMustBeSpecifiedForDailyFieldReport,
                    PXUIFieldAttribute.GetDisplayName(_cache, fieldName1).ToLower(),
                    PXUIFieldAttribute.GetDisplayName(_cache, fieldName2).ToLower());
                throw new Exception(message);
            }
        }

        private async Task<IWeatherIntegrationStrategy> CreateAccuWeatherIntegrationStrategyAsync(CancellationToken cancellationToken)
        {
            ValidateFields(nameof(_location.Latitude), nameof(_location.Longitude));
			var locationKey = await AccuWeatherIntegrationStrategy.GetLocationKeyAsync(_httpClientFactory, _location, _graph, WeatherIntegrationSetup, cancellationToken);

			return new AccuWeatherIntegrationStrategy(_httpClientFactory, locationKey);
        }
    }
}
