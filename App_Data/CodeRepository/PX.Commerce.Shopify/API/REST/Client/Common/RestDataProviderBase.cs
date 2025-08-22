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

using PX.Commerce.Core;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Shopify.API.REST
{
	public abstract class RestDataProviderBase
	{
		protected const string ID_STRING = "id";
		protected const string PARENT_ID_STRING = "parent_id";
		protected const string ApiPrefix = "api";

		protected IShopifyRestClient ShopifyRestClient;
		protected string ApiVersion = SPHelper.GetAPIDefaultVersion();
		protected abstract string GetListUrl { get; }
		protected abstract string GetSingleUrl { get; }
		protected abstract string GetSearchUrl { get; }
		public RestDataProviderBase()
		{

		}
		public virtual async Task<T> Create<T, TR>(T entity, UrlSegments urlSegments = null) where T : class, new() where TR : class, IEntityResponse<T>, new()
		{
			var request = BuildRequest(GetListUrl, nameof(this.Create), urlSegments, null);
			APIHelper.LogIntoProfiler(ShopifyRestClient.Logger,
				string.Format("{0}: creating new {1} entry", BCCaptions.CommerceLogCaption, entity.GetType().ToString()),
				new BCLogTypeScope(GetType()), entity);

			HandleRequesetHeader<T>(request);
			return await ShopifyRestClient.Post<T, TR>(request, entity);
		}

		public virtual async Task<T> Update<T, TR>(T entity, UrlSegments urlSegments) where T : class, new() where TR : class, IEntityResponse<T>, new()
		{
			var request = BuildRequest(GetSingleUrl, nameof(this.Update), urlSegments, null);
			APIHelper.LogIntoProfiler(ShopifyRestClient.Logger,
				string.Format("{0}: updating {1} entry with an ID", BCCaptions.CommerceLogCaption, entity.GetType().ToString()),
				new BCLogTypeScope(GetType()), entity);

			HandleRequesetHeader<T>(request);
			return await ShopifyRestClient.Put<T, TR>(request, entity);
		}

		public virtual async Task<bool> Delete(UrlSegments urlSegments)
		{
			var request = BuildRequest(GetSingleUrl, nameof(this.Delete), urlSegments, null);
			return await ShopifyRestClient.Delete(request);
		}

		public virtual async Task<T> GetByID<T, TR>(UrlSegments urlSegments) where T : class, new() where TR : class, IEntityResponse<T>, new()
		{
			APIHelper.LogIntoProfiler(ShopifyRestClient.Logger,
				string.Format("{0}: Shopify REST API - Getting by ID {1} entry with parameters {2}", BCCaptions.CommerceLogCaption, typeof(T).ToString(), urlSegments?.ToString() ?? "none"),
				new BCLogTypeScope(GetType()));

			HttpRequestMessage request = BuildRequest(GetSingleUrl, nameof(this.GetByID), urlSegments, null);
			T result = await ShopifyRestClient.Get<T, TR>(request);

			APIHelper.LogIntoProfiler(ShopifyRestClient.Logger,
				string.Format("{0}: Shopify REST API - Returned By ID", BCCaptions.CommerceLogCaption),
				new BCLogTypeScope(GetType()), result);

			return result;
		}


		public virtual async IAsyncEnumerable<T> GetAll<T, TR>(IFilter filter = null, UrlSegments urlSegments = null, CancellationToken cancellationToken = default) where T : class, new() where TR : class, IEntitiesResponse<T>, new()
		{
			var request = BuildRequest(GetListUrl, nameof(this.GetAll), urlSegments, filter);
			await foreach (var data in ShopifyRestClient.GetAll<T, TR>(request, cancellationToken))
				yield return data;
		}

		protected static UrlSegments MakeUrlSegments(long id) => MakeUrlSegments(id.ToString());

		protected static UrlSegments MakeUrlSegments(string id)
		{
			var segments = new UrlSegments();
			segments.Add(ID_STRING, id);
			return segments;
		}

		protected static UrlSegments MakeParentUrlSegments(long parentId) => MakeParentUrlSegments(parentId.ToString());

		protected static UrlSegments MakeParentUrlSegments(string parentId)
		{
			var segments = new UrlSegments();
			segments.Add(PARENT_ID_STRING, parentId);
			return segments;
		}

		protected static UrlSegments MakeUrlSegments(long id, long parentId) => MakeUrlSegments(id.ToString(), parentId.ToString());

		protected static UrlSegments MakeUrlSegments(string id, string parentId)
		{
			var segments = new UrlSegments();
			segments.Add(PARENT_ID_STRING, parentId);
			segments.Add(ID_STRING, id);
			return segments;
		}

		protected void ValidationUrl(string url, string methodName)
		{
			if (string.IsNullOrWhiteSpace(url))
				throw new PXNotSupportedException(ShopifyMessages.DataProviderNotSupportMethod, this.GetType().Name, methodName);
		}

		protected virtual string BuildUrl(string url)
		{
			return ApiPrefix + "/" + ApiVersion + "/" + url.TrimStart('/');
		}

		protected void HandleRequesetHeader<T>(HttpRequestMessage request) where T : class
		{
			foreach (var propertyInfo in typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
			{
				var attr = propertyInfo.GetCustomAttribute<ApiHeaderRequestAttribute>();
				if (attr == null) continue;
				String name = attr.HeaderParameterName;
				String value = attr.HeaderParameterValue;
				if (!string.IsNullOrEmpty(name))
				{
					request.Headers.Add(name, value);
				}
			}
		}

		protected HttpRequestMessage BuildRequest(string url, string methodName, UrlSegments urlSegments = null, IFilter filter = null)
		{
			var builtUrl = BuildUrl(url);
			ValidationUrl(builtUrl, methodName);
			var segments = urlSegments?.GetUrlSegments();
			if (segments != null)
			{
				foreach (var segment in segments)
				{
					builtUrl = builtUrl.Replace("{" + segment.Key + "}", segment.Value);
				}
			}
			Dictionary<string, string> queryParameters = null;

			if (filter != null)
				queryParameters = filter?.AddFilter();
			var request = ShopifyRestClient.MakeHttpRequest(builtUrl, queryParameters);

			return request;
		}
	}
}
