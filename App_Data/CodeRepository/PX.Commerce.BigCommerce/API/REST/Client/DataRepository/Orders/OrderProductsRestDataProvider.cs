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
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.BigCommerce.API.REST
{
	public class OrderProductsRestDataProviderFactory : IBCRestDataProviderFactory<IChildRestDataProvider<OrdersProductData>>
	{
		public virtual IChildRestDataProvider<OrdersProductData> CreateInstance(IBigCommerceRestClient restClient) => new OrderProductsRestDataProvider(restClient);
	}

	public class OrderProductsRestDataProvider : RestDataProviderV2, IChildRestDataProvider<OrdersProductData>
    {
        protected override string GetListUrl { get; }   = "v2/orders/{parent_id}/products";
        protected override string GetSingleUrl { get; } = "v2/orders/{parent_id}/products/{id}";

        public OrderProductsRestDataProvider(IBigCommerceRestClient restClient) : base()
		{
            _restClient = restClient;
		}

		public virtual async IAsyncEnumerable<OrdersProductData> GetAll(string parentId, CancellationToken cancellationToken = default)
        {
            var segments = MakeParentUrlSegments(parentId);
            await foreach(var data in GetAll<OrdersProductData>(null, segments, cancellationToken: cancellationToken))
				yield return data;

        }

		public virtual async Task<OrdersProductData> GetByID(string id, string parentId)
        {
            var segments = MakeUrlSegments(id, parentId);
            return await GetByID<OrdersProductData>(segments);
        }

		public virtual async Task<OrdersProductData> Create(OrdersProductData entity, string parentId)
        {
            var segments = MakeParentUrlSegments(parentId);
            return await Create(entity, segments);
        }

		public virtual async Task<OrdersProductData> Update(OrdersProductData entity, string id, string parentId)
        {
            var segments = MakeUrlSegments(id, parentId);
            return await Update(entity, segments);
        }

		public virtual async Task<bool> Delete(string id, string parentId)
        {
            var segments = MakeUrlSegments(id, parentId);
            return await Delete(segments);
        }
    }
}
