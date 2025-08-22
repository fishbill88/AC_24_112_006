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

using CommonServiceLocator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.BigCommerce.API.REST
{
	/// <summary>
	/// BCRestClientFactory
	///  </summary>
	public class BCRestClientFactory : IBCRestClientFactory
	{

		private readonly IHttpClientFactory _clientFactory;
		/// <summary>
		/// BCRestClientFactory
		/// </summary>
		/// <param name="clientFactory"></param>
		public BCRestClientFactory(IHttpClientFactory clientFactory)
		{
			_clientFactory = clientFactory;
		}
		/// <summary>
		/// returns BigcommerceRestClient instance
		/// </summary>
		/// <param name="binding"></param>
		/// <returns></returns>
		public IBigCommerceRestClient GetRestClient(BCBindingBigCommerce binding)
		{
			RestOptions options = new RestOptions
			{
				BaseUri = binding.StoreBaseUrl,
				XAuthClient = binding.StoreXAuthClient,
				XAuthTocken = binding.StoreXAuthToken
			};
			JsonSerializerSettings serializer = new JsonSerializerSettings
			{
				MissingMemberHandling = MissingMemberHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore,
				DefaultValueHandling = DefaultValueHandling.Ignore,
				DateFormatHandling = DateFormatHandling.IsoDateFormat,
				DateTimeZoneHandling = DateTimeZoneHandling.Unspecified,
				ContractResolver = new Core.REST.GetOnlyContractResolver()
			};
			BCRestClient client = new BCRestClient(_clientFactory, serializer, options,
				ServiceLocator.Current.GetInstance<Serilog.ILogger>());

			return client;
		}
	}
}
