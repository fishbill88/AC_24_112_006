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
using PX.CCProcessingBase.Interfaces.V2;
using PX.Commerce.Core;
using PX.Commerce.Shopify.ShopifyPayments;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using static PX.Commerce.Shopify.ShopifyPayments.ShopifyPluginHelper;

namespace PX.Commerce.Shopify.API.REST
{
	public class ShopifyRestClientFactory : IShopifyRestClientFactory
	{
		private readonly IHttpClientFactory _clientFactory;
		public ShopifyRestClientFactory(IHttpClientFactory clientFactory)
		{
			_clientFactory = clientFactory;
		}
		/// <summary>
		/// gets Shopify rest Client
		/// </summary>
		/// <param name="binding"></param>
		/// <returns></returns>
		public IShopifyRestClient GetRestClient(BCBindingShopify binding)
		{
			RestOptions options = new RestOptions
			{
				BaseUri = binding.ShopifyApiBaseUrl.Trim(),
				ApiKey = binding.ShopifyApiKey,
				ApiToken = binding.ShopifyAccessToken,
				SharedSecret = binding.StoreSharedSecret,
				ApiCallLimit = binding.ApiCallLimit.Value
			};
			
			JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
			{
				MissingMemberHandling = MissingMemberHandling.Ignore,
				DateFormatHandling = DateFormatHandling.IsoDateFormat,
				DateTimeZoneHandling = DateTimeZoneHandling.Unspecified,
				Formatting = Formatting.Indented,
				ContractResolver = new Core.REST.GetOnlyContractResolver(),
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			};
			SPRestClient client = new SPRestClient(_clientFactory, jsonSerializerSettings, options,
				ServiceLocator.Current.GetInstance<Serilog.ILogger>()
				);

			return client;
		}
	}
}
