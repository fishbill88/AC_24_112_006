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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.BigCommerce.API.REST
{
	public interface IChildReadOnlyRestDataProvider<T> where T : class
    {
        Task<T> GetByID(string id, string parentId);
        IAsyncEnumerable<T> GetAll(string externID, CancellationToken cancellationToken = default);
    }

    public interface IChildRestDataProvider<T> : IChildReadOnlyRestDataProvider<T> where T : class
    {
        Task<T> Create(T entity, string parentId);
        Task<T> Update(T entity, string id, string parentId);
        Task<bool> Delete(string id, string parentId);        
    }

	public interface IChildUpdateAllRestDataProvider<T> where T : class
	{
		Task UpdateAll(List<T> productDatas, Func<ItemProcessCallback<T>,Task> callback);
		Task<List<T>> GetVariants(string parentId, CancellationToken cancellationToken = default);
	}

	public interface ISubChildRestDataProvider<T>  where T : class
    {
        IAsyncEnumerable<T> GetAll(string parentId, string subId, CancellationToken cancellationToken = default);
        Task<T> GetByID(string parentId, string subId, string id);

        Task<T> Create(T entity, string parentId, string subId);
        Task<T> Update(T entity, string parentId, string subId, string id);
        Task<bool> Delete(string parentId, string subId, string id);
    }
}
