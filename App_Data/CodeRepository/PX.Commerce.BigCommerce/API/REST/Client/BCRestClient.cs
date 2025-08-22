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

using Newtonsoft.Json;
using PX.Commerce.Core;
using PX.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PX.Commerce.BigCommerce.API.REST
{
	public class BCRestClient : BCRestClientBase, IBigCommerceRestClient
	{
		private JsonSerializerSettings _serializer;
		public BCRestClient(IHttpClientFactory clientFactory, JsonSerializerSettings serializer, IRestOptions options, Serilog.ILogger logger) : base(clientFactory, serializer, options, logger)
		{
			_serializer = serializer;
		}

		#region API version 2
		public async Task<T> Post<T>(HttpRequestMessage request, T obj)
			where T : class, new()
		{
			request.Method = HttpMethod.Post;
			AddBody<T>(request, obj);
			var response = await Execute<T>(request);
			response.RequestBody = obj;
			if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
			{
				T result = response.Data;

				if (result != null && result is BCAPIEntity) (result as BCAPIEntity).JSON = response.Content;

				return result;
			}

			LogError(response);
			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				throw new Exception($"Cannot insert {obj.GetType().Name}");
			}

			throw new RestException(response);
		}

		public async Task<T> Put<T>(HttpRequestMessage request, T obj)
			where T : class, new()
		{
			request.Method = HttpMethod.Put;
			AddBody<T>(request, obj);

			var response = await Execute<T>(request);
			response.RequestBody = obj;
			if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent)
			{
				T result = response.Data;

				if (result != null && result is BCAPIEntity) (result as BCAPIEntity).JSON = response.Content;

				return result;
			}

			LogError(response);

			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				throw new Exception($"Cannot update {obj.GetType().Name}");
			}

			throw new RestException(response);
		}

		public async Task<bool> Delete(HttpRequestMessage request)
		{
			request.Method = HttpMethod.Delete;
			var response = await Execute(request);
			if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.NotFound)
			{
				return true;
			}

			LogError(response);
			throw new RestException(response);
		}

		public async Task<T> Get<T>(HttpRequestMessage request)
			where T : class, new()
		{
			request.Method = HttpMethod.Get;
			var response = await Execute<T>(request);

			if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.NotFound)
			{
				T result = response.Data;

				if (result != null && result is BCAPIEntity) (result as BCAPIEntity).JSON = response.Content;
				if (result != null && result is IEnumerable<BCAPIEntity>) (result as IEnumerable<BCAPIEntity>).ForEach(e => e.JSON = response.Content);

				return result;
			}

			LogError(response);

			if (response.StatusCode == HttpStatusCode.InternalServerError && string.IsNullOrEmpty(response.Content))
			{
				throw new Exception(BigCommerceMessages.InternalServerError);
			}
			throw new RestException(response);
		}
		#endregion

		#region API version 3
		public async Task<TE> Post<T, TE>(HttpRequestMessage request, T entity)
			where T : class, new()
			where TE : IEntityResponse<T>, new()
		{
			request.Method = HttpMethod.Post;
			AddBody<T>(request, entity);
			var response = await Execute<TE>(request);
			response.RequestBody = entity;
			if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
			{
				TE result = response.Data;

				if (result?.Data != null && result?.Data is BCAPIEntity) (result?.Data as BCAPIEntity).JSON = response.Content;

				return result;
			}

			LogError(response);
			throw new RestException(response);
		}
		public async Task<TE> Post<T, TE>(HttpRequestMessage request, List<T> entities)
			where T : class, new()
			where TE : IEntitiesResponse<T>, new()
		{
			request.Method = HttpMethod.Post;
			AddBody(request, entities);
			var response = await Execute<TE>(request);
			response.RequestBody = entities;
			if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
			{
				TE result = response.Data;

				if (result?.Data != null && result?.Data is IEnumerable<BCAPIEntity>) (result?.Data as IEnumerable<BCAPIEntity>).ForEach(e => e.JSON = response.Content);

				return result;
			}

			LogError(response);
			throw new RestException(response);
		}
		public async Task<TE> Post<T, TE>(HttpRequestMessage request, TE entity)
			where T : class, new()
			where TE : IEntityResponse<T>, new()
		{
			request.Method = HttpMethod.Post;
			AddBody(request, entity.Data);
			var response = await Execute<TE>(request);
			response.RequestBody = entity.Data;
			if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
			{
				TE result = response.Data;

				if (result?.Data != null && result?.Data is BCAPIEntity) (result?.Data as BCAPIEntity).JSON = response.Content;

				return result;
			}

			LogError(response);
			throw new RestException(response);
		}

		public async Task<TE> Put<T, TE>(HttpRequestMessage request, T entity)
			where T : class, new()
			where TE : IEntityResponse<T>, new()
		{
			request.Method = HttpMethod.Put;
			AddBody(request, entity);

			var response = await Execute<TE>(request);
			response.RequestBody = entity;
			if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent)
			{
				TE result = response.Data;

				if (result?.Data != null && result?.Data is BCAPIEntity) (result?.Data as BCAPIEntity).JSON = response.Content;

				return result;
			}

			LogError(response);
			throw new RestException(response);
		}
		public async Task<TE> Put<T, TE>(HttpRequestMessage request, List<T> entities)
			where T : class, new()
			where TE : IEntitiesResponse<T>, new()
		{
			request.Method = HttpMethod.Put;
			AddBody(request, entities);

			var response = await Execute<TE>(request);
			response.RequestBody = entities;
			if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent)
			{
				TE result = response.Data;

				if (result?.Data != null && result?.Data is IEnumerable<BCAPIEntity>) (result?.Data as IEnumerable<BCAPIEntity>).ForEach(e => e.JSON = response.Content);

				return result;
			}

			LogError(response);
			throw new RestException(response);
		}
		public async Task<TE> Put<T, TE>(HttpRequestMessage request, TE entity)
			where T : class, new()
			where TE : IEntityResponse<T>, new()
		{
			request.Method = HttpMethod.Put;
			AddBody(request, entity.Data);

			var response = await Execute<TE>(request);
			response.RequestBody = entity.Data;
			if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent)
			{
				TE result = response.Data;

				if (result?.Data != null && result?.Data is BCAPIEntity) (result?.Data as BCAPIEntity).JSON = response.Content;

				return result;
			}

			LogError(response);
			throw new RestException(response);
		}

		public async Task<TE> PutList<T, TE>(HttpRequestMessage request, TE entity)
			where T : class, new()
			where TE : IEntitiesResponse<T>, new()
		{
			request.Method = HttpMethod.Put;
			AddBody(request, entity.Data);

			var response = await Execute<TE>(request);
			response.RequestBody = entity.Data;
			if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent)
			{
				TE result = response.Data;

				if (result != null && result is IEnumerable<BCAPIEntity>) (result as IEnumerable<BCAPIEntity>).ForEach(e => e.JSON = response.Content);

				return result;
			}

			LogError(response);
			throw new RestException(response);
		}

		public async Task<TE> Get<T, TE>(HttpRequestMessage request)
			where T : class, new()
			where TE : IEntityResponse<T>, new()
		{
			request.Method = HttpMethod.Get;
			var response = await Execute<TE>(request);

			if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.NotFound)
			{
				TE result = response.Data;

				if (result?.Data != null && result?.Data is BCAPIEntity) (result?.Data as BCAPIEntity).JSON = response.Content;

				return result;
			}

			LogError(response);
			throw new RestException(response);
		}
		public async Task<TE> GetList<T, TE>(HttpRequestMessage request)
			where T : class, new()
			where TE : IEntitiesResponse<T>, new()
		{
			request.Method = HttpMethod.Get;
			var response = await Execute<TE>(request);

			if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent)
			{
				TE result = response.Data;

				if (result != null && result is IEnumerable<BCAPIEntity>) (result as IEnumerable<BCAPIEntity>).ForEach(e => e.JSON = response.Content);

				return result;
			}

			LogError(response);
			throw new RestException(response);
		}

		private void AddBody<T>(HttpRequestMessage request, T obj)
			where T : class, new()
		{
			request.Content = new StringContent(JsonConvert.SerializeObject(obj, _serializer));
			request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
		}

		#endregion
	}
}
