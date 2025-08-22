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

using PX.Commerce.Core.REST;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http;
using System.Threading;
using PX.Data;

namespace PX.Commerce.Amazon.API.Rest
{
	public abstract class RestDataProviderBase
	{
		protected const string ID_STRING = "id";

		protected const string PARENT_ID_STRING = "parent_id";

		protected IAmazonRestClient RestClient { get; }

		protected abstract string GetListUrl { get; }

		protected abstract string GetSingleUrl { get; }

		public RestDataProviderBase(IAmazonRestClient restClient)
		{
			if (restClient is null)
				throw new PXArgumentException(nameof(restClient), ErrorMessages.ArgumentNullException);

			this.RestClient = restClient;
		}

		public virtual async IAsyncEnumerable<T> GetAll<T, TP, TR>(Filter filter = null, UrlSegments urlSegments = null, string url = null, CreateRestrictedDataTokenRequest createRestrictedDataToken = null,
			CancellationToken cancellationToken = default)
			where T : class, new()
			where TP : class, IEntityPayloadResponse<TR>, new()
			where TR : class, IEntityListResponse<T>, new()
		{
			url = url ?? GetListUrl;

			var request = await this.RestClient.PrepareRequest(url, HttpMethod.Get, queryParams: filter?.AddFilter(), pathParams: urlSegments?.GetUrlSegments(), createRestrictedDataToken: createRestrictedDataToken);
			await foreach (var order in this.RestClient.GetAll<T, TP, TR>(request, url, cancellationToken : cancellationToken))
				yield return order;
		}

		public virtual async Task<string> GetServerTime()
		{
			var request = await this.RestClient.PrepareRequest(GetListUrl, HttpMethod.Get);

			return await this.RestClient.GetExternalTime(request, GetListUrl);
		}

		public virtual async Task<T> GetByID<T, TR>(UrlSegments urlSegments, string url = null, CreateRestrictedDataTokenRequest createRestrictedDataToken = null, Filter filter = null) where T : class, new() where TR : class, IEntityResponse<T>, new()
		{
			url = url ?? GetSingleUrl;

			var request = await this.RestClient.PrepareRequest(url, HttpMethod.Get, queryParams: filter?.AddFilter(), pathParams: urlSegments?.GetUrlSegments(), createRestrictedDataToken: createRestrictedDataToken);

			return await this.RestClient.Get<T, TR>(request, url);
		}
		public virtual async Task<T> GetByID<T>(UrlSegments urlSegments, string url = null, Filter filter = null) where T : class, new()
		{
			url = url ?? GetSingleUrl;

			var request = await this.RestClient.PrepareRequest(url, HttpMethod.Get, queryParams: filter?.AddFilter(), pathParams: urlSegments?.GetUrlSegments());

			return await this.RestClient.Get<T>(request, url);
		}

		public virtual async Task<TR> Post<T, TR>(T obj, string url = null) where T : class, new() where TR : class, new()
		{
			var request = await this.RestClient.PrepareRequest(url, HttpMethod.Post, postBody: obj);

			return await this.RestClient.Post<TR>(request, url);
		}

		protected static UrlSegments MakeUrlSegments(string id)
		{
			var segments = new UrlSegments();
			segments.Add(ID_STRING, id);

			return segments;
		}

		protected async Task UploadFeedAsync(string data, string url, string contentType)
		{
			var request = await this.RestClient.PrepareRequest(url, HttpMethod.Put, false, contentType: contentType, postBody: data);

			await this.RestClient.Put(request, url);
		}
	}
}
