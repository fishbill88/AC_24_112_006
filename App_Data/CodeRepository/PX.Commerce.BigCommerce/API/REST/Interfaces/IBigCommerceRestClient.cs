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
using System.Net.Http;
using System.Threading.Tasks;

namespace PX.Commerce.BigCommerce.API.REST
{
	public interface IBigCommerceRestClient
    {
        Task<T> Post<T>(HttpRequestMessage request, T obj) where T : class, new();
        Task<T> Put<T>(HttpRequestMessage request, T obj) where T : class, new();
        Task<bool> Delete(HttpRequestMessage request);
        Task<T> Get<T>(HttpRequestMessage request) where T : class, new();   
		
        HttpRequestMessage MakeHttpRequest(string url, Dictionary<string, string> urlSegments = null);

		Task<TE> Post<T, TE>(HttpRequestMessage request, T entity) where T : class, new() where TE : IEntityResponse<T>, new();
		Task<TE> Post<T, TE>(HttpRequestMessage request, TE entity) where T : class, new() where TE: IEntityResponse<T>, new();
		Task<TE> Post<T, TE>(HttpRequestMessage request, List<T> entities) where T : class, new() where TE : IEntitiesResponse<T>, new();
		Task<TE> Put<T, TE>(HttpRequestMessage request, T entity) where T : class, new() where TE : IEntityResponse<T>, new();
		Task<TE>Put<T, TE>(HttpRequestMessage request, TE entity) where T : class, new() where TE : IEntityResponse<T>, new();
		Task<TE> Put<T, TE>(HttpRequestMessage request, List<T> entities) where T : class, new() where TE : IEntitiesResponse<T>, new();
		Task<TE> PutList<T, TE>(HttpRequestMessage request, TE entity) where T : class, new() where TE : IEntitiesResponse<T>, new();
		Task<TE> Get<T, TE>(HttpRequestMessage request) where T : class, new() where TE: IEntityResponse<T>, new();
		Task<TE> GetList<T, TE>(HttpRequestMessage request) where T : class, new() where TE : IEntitiesResponse<T>, new();
		Serilog.ILogger Logger { set; get; }
	}
}
