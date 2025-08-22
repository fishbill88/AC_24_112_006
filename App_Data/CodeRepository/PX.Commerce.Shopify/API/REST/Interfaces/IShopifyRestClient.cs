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
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Shopify.API.REST
{
	public interface IShopifyRestClient
	{
		/// <summary>
		/// Post data to Shopify
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TR"></typeparam>
		/// <param name="request"></param>
		/// <param name="obj">The data obj that posts to Shopify API, it should be a T object</param>
		/// <param name="usingTRasBodyObj">True : system will auto convert T data obj to TR response obj first, and then posts to Shopify API; False : system will post T data obj to Shopify API directly. Please follow the Shopify API documents to determine this value, default is true.</param>
		/// <returns>The response data from Shopify API, it is a TR object</returns>
		Task<T> Post<T, TR>(HttpRequestMessage request, T obj, bool usingTRasBodyObj = true, CancellationToken cancellationToken = default) where T : class, new() where TR : class, IEntityResponse<T>, new();

		/// <summary>
		/// Update data to Shopify
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TR"></typeparam>
		/// <param name="request"></param>
		/// <param name="obj">The data obj that udpates to Shopify API, it should be a T object</param>
		/// <param name="usingTRasBodyObj">True : system will auto convert T data obj to TR response obj first, and then posts to Shopify API; False : system will post T data obj to Shopify API directly. Please follow the Shopify API documents to determine this value, default is true.</param>
		/// <returns>The response data from Shopify API, it is a TR object</returns>
		Task<T> Put<T, TR>(HttpRequestMessage request, T obj, bool usingTRasBodyObj = true,CancellationToken cancellationToken = default) where T : class, new() where TR : class, IEntityResponse<T>, new();
		Task<bool> Delete(HttpRequestMessage request,CancellationToken cancellationToken = default);
		Task<T> Get<T, TR>(HttpRequestMessage request,CancellationToken cancellationToken = default) where T : class, new() where TR : class, IEntityResponse<T>, new();
		Task<(T,HttpResponseHeaders)> GetWithHeaders<T, TR>(HttpRequestMessage request,CancellationToken cancellationToken = default) where T : class, new() where TR : class, IEntityResponse<T>, new();
		IAsyncEnumerable<T> GetAll<T, TR>(HttpRequestMessage request, CancellationToken cancellationToken = default) where T : class, new() where TR : class, IEntitiesResponse<T>, new();
		HttpRequestMessage MakeHttpRequest(string url, Dictionary<string, string> urlSegments = null);
		Task<bool> Post(HttpRequestMessage request, CancellationToken cancellationToken = default);
		Serilog.ILogger Logger { set; get; }
	}
}
