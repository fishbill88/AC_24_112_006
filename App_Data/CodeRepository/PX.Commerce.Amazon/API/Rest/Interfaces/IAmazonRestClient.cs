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

using PX.Commerce.Amazon.API.Rest.Client.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon.API.Rest
{
	public interface IAmazonRestClient
	{
		/// <summary>
		/// To get data by ID
		/// </summary>
		/// <typeparam name="T">Response type</typeparam>
		/// <typeparam name="TR">EntityResponse</typeparam>
		/// <param name="request">HttpRequestMessage</param>
		/// <returns>Response type</returns>
		Task<T> Get<T, TR>(AmazonRequest request, string url, CreateRestrictedDataTokenRequest createRestrictedDataToken = null, CancellationToken cancellationToken = default)
			where T : class, new()
			where TR : class, IEntityResponse<T>, new();

		/// <summary>
		/// To get data by ID
		/// </summary>
		/// <typeparam name="T">Response type</typeparam>
		/// <param name="request">HttpRequestMessage</param>
		/// <returns>Response type</returns>
		Task<T> Get<T>(AmazonRequest request, string url, CreateRestrictedDataTokenRequest createRestrictedDataToken = null, CancellationToken cancellationToken = default)
			where T : class, new();

		/// <summary>
		/// To get all the data with paging
		/// </summary>
		/// <typeparam name="T">Response type</typeparam>
		/// <typeparam name="TP">EntityPayloadResponse</typeparam>
		/// <typeparam name="TR">IEntityListPageResponse</typeparam>
		/// <param name="request"></param>
		/// <returns>IEnumerable of response type</returns>
		IAsyncEnumerable<T> GetAll<T, TP, TR>(AmazonRequest request, string url, CreateRestrictedDataTokenRequest createRestrictedDataToken = null,
			CancellationToken cancellationToken = default)
			where T : class, new()
			where TP : class, IEntityPayloadResponse<TR>, new()
			where TR : class, IEntityListResponse<T>, new();

		/// <summary>
		/// To prepare Rest request
		/// </summary>
		/// <param name="path">API resource</param>
		/// <param name="method">Http Method</param>
		/// <param name="queryParams">Query String</param>
		/// <param name="postBody">Body</param>
		/// <param name="pathParams">Path Parameters</param>
		/// <param name="contentType">Content type</param>
		/// <returns>HttpRequestMessage</returns>
		Task<AmazonRequest> PrepareRequest(String path, HttpMethod method, bool withAuthentication = true, List<(string, string)> queryParams = null, object postBody = null,
			Dictionary<String, String> pathParams = null, string contentType = "application/json", CreateRestrictedDataTokenRequest createRestrictedDataToken = null, string serviceEndpoint = "spapi"
			, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get External system time
		/// </summary>
		/// <param name="request">Rest Request</param>
		/// <returns>Datetime string</returns>
		Task<string> GetExternalTime(AmazonRequest request, string url, CreateRestrictedDataTokenRequest createRestrictedDataToken = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Posts the specified <paramref name="request"/>.
		/// </summary>
		/// <typeparam name="T">A return type of a result.</typeparam>
		/// <param name="request">The request to be sent.</param>
		/// <param name="url">The url address the request to be sent.</param>
		/// <param name="createRestrictedDataToken">The restricted data token DTO.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The data in response.</returns>
		Task<T> Post<T>(AmazonRequest request, string url, CreateRestrictedDataTokenRequest createRestrictedDataToken = null, CancellationToken cancellationToken = default)
			where T : class, new ();

		/// <summary>
		/// Puts the specified <paramref name="request"/>.
		/// </summary>
		/// <param name="request">The request to be sent.</param>
		/// <param name="url">The url address the request to be sent.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task Put(AmazonRequest request, string url, CancellationToken cancellationToken = default);
	}
}
