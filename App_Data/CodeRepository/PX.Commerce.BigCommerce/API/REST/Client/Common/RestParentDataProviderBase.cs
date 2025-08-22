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
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.BigCommerce.API.REST
{
	public abstract class RestDataProviderBase
	{
		protected const string ID_STRING = "id";
		protected const string PARENT_ID_STRING = "parent_id";
		protected const string OTHER_PARAM = "other_param";
		protected IBigCommerceRestClient _restClient;

		protected abstract string GetListUrl { get; }
		protected abstract string GetSingleUrl { get; }

		public RestDataProviderBase()
		{
		}

		public virtual async Task<T> Create<T>(T entity, UrlSegments urlSegments = null)
			where T : class, new()
		{
			APIHelper.LogIntoProfiler(_restClient.Logger,
				string.Format("{0}: BigCommerce REST API - Creating new {1} entity with parameters {2}", BCCaptions.CommerceLogCaption, typeof(T).ToString(), urlSegments?.ToString() ?? "none"),
				new BCLogTypeScope(GetType()), entity);

			var request = BuildRequest(GetListUrl, urlSegments: urlSegments);


			return await _restClient.Post(request, entity);
		}
		public virtual async Task<TE> Create<T, TE>(T entity, UrlSegments urlSegments = null)
			where T : class, new()
			where TE : IEntityResponse<T>, new()
		{
			APIHelper.LogIntoProfiler(_restClient.Logger,
				string.Format("{0}: BigCommerce REST API - Creating new {1} entity with parameters {2}", BCCaptions.CommerceLogCaption, typeof(T).ToString(), urlSegments?.ToString() ?? "none"),
				new BCLogTypeScope(GetType()), entity);
			var request = BuildRequest(GetListUrl, urlSegments: urlSegments);

			TE result = await _restClient.Post<T, TE>(request, entity);

			return result;

		}
		public virtual async Task<TE> Create<T, TE>(List<T> entities, UrlSegments urlSegments = null)
			where T : class, new()
			where TE : IEntitiesResponse<T>, new()
		{
			APIHelper.LogIntoProfiler(_restClient.Logger,
				string.Format("{0}: BigCommerce REST API - Creating new {1} entity with parameters {2}", BCCaptions.CommerceLogCaption, typeof(T).ToString(), urlSegments?.ToString() ?? "none"),
				new BCLogTypeScope(GetType()), entities);
			var request = BuildRequest(GetListUrl, urlSegments: urlSegments);

			TE result = await _restClient.Post<T, TE>(request, entities);

			return result;
		}

		public virtual async Task<T> Update<T>(T entity, UrlSegments urlSegments)
			where T : class, new()
		{
			APIHelper.LogIntoProfiler(_restClient.Logger, 
				string.Format("{0}: BigCommerce REST API - Updating {1} entity with parameters {2}", BCCaptions.CommerceLogCaption, typeof(T).ToString(), urlSegments?.ToString() ?? "none"),
				new BCLogTypeScope(GetType()), entity);
			var request = BuildRequest(GetSingleUrl, urlSegments: urlSegments);
			T result = await _restClient.Put(request, entity);

			return result;
		}

		public virtual async Task<TE> Update<T, TE>(T entity, UrlSegments urlSegments)
			where T : class, new()
			where TE : class, IEntityResponse<T>, new()
		{
			APIHelper.LogIntoProfiler(_restClient.Logger, 
				string.Format("{0}: BigCommerce REST API - Updating {1} entity with parameters {2}", BCCaptions.CommerceLogCaption, typeof(T).ToString(), urlSegments?.ToString() ?? "none"),
				new BCLogTypeScope(GetType()), entity);
			var request = BuildRequest(GetSingleUrl, urlSegments: urlSegments);

			TE result = await _restClient.Put<T, TE>(request, entity);

			return result;
		}
		public virtual async Task<TE> Update<T, TE>(List<T> entities, UrlSegments urlSegments = null)
			where T : class, new()
			where TE : class, IEntitiesResponse<T>, new()
		{
			APIHelper.LogIntoProfiler(_restClient.Logger, 
				string.Format("{0}: BigCommerce REST API - Updating {1} entity with parameters {2}", BCCaptions.CommerceLogCaption, typeof(T).ToString(), urlSegments?.ToString() ?? "none"),
				new BCLogTypeScope(GetType()), entities);
			var request = BuildRequest(GetSingleUrl, urlSegments: urlSegments);

			return await _restClient.Put<T, TE>(request, entities);
		}

		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		public virtual async Task<bool> Delete(UrlSegments urlSegments)
		{
			APIHelper.LogIntoProfiler(_restClient.Logger, 
				string.Format("{0}: BigCommerce REST API - Deleting {1} entity with parameters {2}", BCCaptions.CommerceLogCaption, this.GetType().ToString(), urlSegments?.ToString() ?? "none"),
				new BCLogTypeScope(GetType()));
			var request = BuildRequest(GetSingleUrl, urlSegments: urlSegments);

			var result = await _restClient.Delete(request);

			return result;
		}

		protected static UrlSegments MakeUrlSegments(string id)
		{
			var segments = new UrlSegments();
			segments.Add(ID_STRING, id);
			return segments;
		}

		protected static UrlSegments MakeParentUrlSegments(string parentId)
		{
			var segments = new UrlSegments();
			segments.Add(PARENT_ID_STRING, parentId);

			return segments;
		}

		protected HttpRequestMessage BuildRequest(string url, IFilter filter = null, UrlSegments urlSegments = null)
		{
			var segments = urlSegments?.GetUrlSegments();
			var builtUrl = url;
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
			var request = _restClient.MakeHttpRequest(builtUrl, queryParameters);
			return request;
		}
		protected static UrlSegments MakeUrlSegments(string id, string parentId)
		{
			var segments = new UrlSegments();
			segments.Add(PARENT_ID_STRING, parentId);
			segments.Add(ID_STRING, id);
			return segments;
		}
		protected static UrlSegments MakeUrlSegments(string id, string parentId, string param)
		{
			var segments = new UrlSegments();
			segments.Add(PARENT_ID_STRING, parentId);
			segments.Add(ID_STRING, id);
			segments.Add(OTHER_PARAM, param);
			return segments;
		}
	}

	public abstract class RestDataProviderV2 : RestDataProviderBase
	{
		public RestDataProviderV2() : base()
		{

		}

		protected virtual async Task<List<T>> Get<T>(IFilter filter = null, UrlSegments urlSegments = null)
			where T : class, new()
		{
			APIHelper.LogIntoProfiler(_restClient.Logger, 
				string.Format("{0}: BigCommerce REST API - Getting {1} entity with parameters {2}", BCCaptions.CommerceLogCaption, this.GetType().ToString(), urlSegments?.ToString() ?? "none"),
				new BCLogTypeScope(GetType()));
			HttpRequestMessage request = BuildRequest(GetListUrl, filter, urlSegments);
			var entity = await _restClient.Get<List<T>>(request);
			return entity;
		}


		public virtual async IAsyncEnumerable<T> GetAll<T>(IFilter filter = null, UrlSegments urlSegments = null, CancellationToken cancellationToken = default)
			where T : class, new()
		{
			var localFilter = filter ?? new Filter();
			var needGet = true;

			localFilter.Page = 1;
			localFilter.Limit = 50;

			while (needGet)
			{
				cancellationToken.ThrowIfCancellationRequested();

				List<T> entities = await Get<T>(localFilter, urlSegments);

				if (entities == null) yield break;
				foreach (T entity in entities)
				{
					yield return entity;
				}
				localFilter.Page++;
				needGet = localFilter.Limit == entities.Count;
			}
		}

		public virtual async Task<T> GetByID<T>(UrlSegments urlSegments) where T : class, new()
		{
			APIHelper.LogIntoProfiler(_restClient.Logger, 
				string.Format("{0}: BigCommerce REST API - Getting by ID {1} entity with parameters {2}", BCCaptions.CommerceLogCaption, this.GetType().ToString(), urlSegments?.ToString() ?? "none"),
				new BCLogTypeScope(GetType()));
			var request = BuildRequest(GetSingleUrl, urlSegments: urlSegments);

			var entity = await _restClient.Get<T>(request);

			return entity;
		}
	}

	public abstract class RestDataProviderV3 : RestDataProviderBase
	{
		protected const int DEFAULT_BATCH_SIZE = 10;
		protected virtual int BatchSize
		{
			get { return DEFAULT_BATCH_SIZE; }
		}
		public RestDataProviderV3() : base()
		{

		}

		protected virtual async Task<TE> Get<T, TE>(IFilter filter = null, UrlSegments urlSegments = null)
			where T : class, new()
			where TE : IEntitiesResponse<T>, new()
		{
			APIHelper.LogIntoProfiler(_restClient.Logger, 
				string.Format("{0}: BigCommerce REST API - Getting {1} entity with parameters {2}", BCCaptions.CommerceLogCaption, this.GetType().ToString(), urlSegments?.ToString() ?? "none"),
				new BCLogTypeScope(GetType()));

			var request = BuildRequest(GetListUrl, filter, urlSegments);

			var response = await _restClient.GetList<T, TE>(request);
			return response;
		}

		public virtual async IAsyncEnumerable<T> GetAll<T, TE>(IFilter filter = null, UrlSegments urlSegments = null, CancellationToken cancellationToken = default)
			where T : class, new()
			where TE : IEntitiesResponse<T>, new()
		{
			var localFilter = filter ?? new Filter();
			var needGet = true;

			localFilter.Page = 1;
			localFilter.Limit = 50;
			TE entity = default;
			while (needGet)
			{
				cancellationToken.ThrowIfCancellationRequested();

				entity = await Get<T, TE>(localFilter, urlSegments);

				if (entity?.Data == null) yield break;
				foreach (T data in entity.Data)
				{
					yield return data;
				}

				needGet = localFilter.Page < entity?.Meta?.Pagination.TotalPages;
				localFilter.Page++;
			}
		}

		public virtual async Task<TE> GetByID<T, TE>(UrlSegments urlSegments, IFilter filter = null)
			where T : class, new()
			where TE : IEntityResponse<T>, new()
		{
			APIHelper.LogIntoProfiler(_restClient.Logger, 
				string.Format("{0}: BigCommerce REST API - Getting by ID {1} entity with parameters {2}", BCCaptions.CommerceLogCaption, typeof(T).ToString(), urlSegments?.ToString() ?? "none"),
				new BCLogTypeScope(GetType()));

			var request = BuildRequest(GetSingleUrl, filter, urlSegments);

			TE result = await _restClient.Get<T, TE>(request);

			APIHelper.LogIntoProfiler(_restClient.Logger, 
				string.Format("{0}: BigCommerce REST API - Returned By ID", BCCaptions.CommerceLogCaption),
				new BCLogTypeScope(GetType()), result);

			return result;
		}

		public virtual async Task<TE> Create<T, TE>(TE entity, UrlSegments urlSegments = null)
			where T : class, new()
			where TE : IEntityResponse<T>, new()
		{
			APIHelper.LogIntoProfiler(_restClient.Logger, 
				string.Format("{0}: BigCommerce REST API - Creating of ID {1} entry with parameters {2}", BCCaptions.CommerceLogCaption, typeof(T).ToString(), urlSegments?.ToString() ?? "none"),
				new BCLogTypeScope(GetType()), entity);

			var request = BuildRequest(GetListUrl, urlSegments: urlSegments);


			TE result = await _restClient.Post<T, TE>(request, entity);

			return result;
		}

		public virtual async Task<TE> Update<T, TE>(TE entity, UrlSegments urlSegments = null)
			where T : class, new()
			where TE : IEntityResponse<T>, new()
		{
			APIHelper.LogIntoProfiler(_restClient.Logger,
				string.Format("{0}: BigCommerce REST API - Updating {1} entry with parameters {2}", BCCaptions.CommerceLogCaption, typeof(T).ToString(), urlSegments?.ToString() ?? "none"),
				new BCLogTypeScope(GetType()), entity);

			var request = BuildRequest(GetSingleUrl, urlSegments: urlSegments);
			TE result = await _restClient.Put<T, TE>(request, entity);
			return result;
		}

		public virtual async Task<TE> UpdateAll<T, TE>(TE entities, UrlSegments urlSegments = null)
			where T : class, new()
			where TE : IEntitiesResponse<T>, new()
		{
			APIHelper.LogIntoProfiler(_restClient.Logger,
				string.Format("{0}: BigCommerce REST API - Updating {1} entry with parameters {2}", BCCaptions.CommerceLogCaption, typeof(T).ToString(), urlSegments?.ToString() ?? "none"),
				new BCLogTypeScope(GetType()), entities);

			var request = BuildRequest(GetSingleUrl, urlSegments: urlSegments);
			return await _restClient.PutList<T, TE>(request, entities);
		}
		public virtual async Task UpdateAll<T, TE>(TE entities, UrlSegments urlSegments, Func<ItemProcessCallback<T>, Task> callback)
			where T : class, new()
			where TE : IEntitiesResponse<T>, new()
		{
			TE batch = new TE();
			batch.Meta = entities.Meta;

			int index = 0;
			for (; index < entities.Data.Count; index++)
			{
				if (index % BatchSize == 0 && batch.Data.Count > 0)
				{
					await UpdateBatch<T, TE>(batch, urlSegments, index - batch.Data.Count, callback);

					batch.Data.Clear();
				}
				batch.Data.Add(entities.Data[index]);
			}
			if (batch.Data.Count > 0)
			{
				await UpdateBatch<T, TE>(batch, urlSegments, index - batch.Data.Count, callback);
			}


		}

		protected async Task UpdateBatch<T, TE>(TE batch, UrlSegments urlSegments, Int32 startIndex,Func<ItemProcessCallback<T>, Task> callback)
			where T : class, new()
			where TE : IEntitiesResponse<T>, new()
		{
			APIHelper.LogIntoProfiler(_restClient.Logger,
				string.Format("{0}: BigCommerce REST API - Batch Updating of {1} entry with parameters {2}", BCCaptions.CommerceLogCaption, typeof(T).ToString(), urlSegments?.ToString() ?? "none"),
				new BCLogTypeScope(GetType()), batch);

			while (true)
				try
				{
					var request = BuildRequest(GetListUrl, urlSegments: urlSegments);
					TE response = await _restClient.PutList<T, TE>(request, batch);
					if (response == null) return;
					for (int i = 0; i < response.Data.Count; i++)
					{
						T item = response.Data[i];
						await callback(new ItemProcessCallback<T>(startIndex + i, item));
					}
					break;
				}
				catch (RestException ex)
				{
					if (ex?.ResponceStatusCode == default(HttpStatusCode).ToString())
					{
						throw;
					}
					else
					{
						for (int i = 0; i < batch.Data.Count; i++)
						{
							await callback(new ItemProcessCallback<T>(startIndex + i, ex, batch.Data));
						}
						break;
					}
				}
		}

		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		public virtual async Task<bool> Delete(IFilter filter = null)
		{
			var request = BuildRequest(GetSingleUrl, filter);
			var response = await _restClient.Delete(request);
			return response;
		}
	}
}
