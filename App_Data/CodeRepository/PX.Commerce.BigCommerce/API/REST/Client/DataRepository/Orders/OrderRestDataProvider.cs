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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.BigCommerce.API.REST
{
	public class OrderRestDataProviderFactory : IBCRestDataProviderFactory<IOrderRestDataProvider>
	{
		public virtual IOrderRestDataProvider CreateInstance(IBigCommerceRestClient restClient) => new OrderRestDataProvider(restClient);
	}

	public class OrderRestDataProvider : RestDataProviderV2, IOrderRestDataProvider
	{
        protected override string GetListUrl { get; } = "v2/orders";
        protected override string GetSingleUrl { get; } = "v2/orders/{id}";

		public OrderRestDataProvider(IBigCommerceRestClient restClient) : base()
		{
            _restClient = restClient;

		}

		public async Task<OrderData> Create(OrderData order)
		{
			var newOrder = await Create<OrderData>(order);
			return newOrder;
		}

		public virtual async Task<OrderData> Update(OrderData order, string id)
		{
			var segments = MakeUrlSegments(id);
			var updated = await Update(order, segments);
			return updated;
		}

		public virtual async Task<OrderStatus> Update(OrderStatus order, string id)
		{
			var segments = MakeUrlSegments(id);
			var updated = await Update(order, segments);
			return updated;
		}

        public async Task<bool> Delete(string id)
        {
            var segments = MakeUrlSegments(id.ToString());
            return await base.Delete(segments);
        }

		public virtual async Task<bool> Delete(string id, OrderData order)
		{
			return await Delete(id);
		}

		public virtual async Task<List<OrderData>> Get(IFilter filter = null)
		{
			return await base.Get<OrderData>(filter);
        }

		public virtual async IAsyncEnumerable<OrderData> GetAll(IFilter filter = null, CancellationToken cancellationToken = default)
		{
			await foreach(var data in base.GetAll<OrderData>(filter, cancellationToken: cancellationToken))
				yield return data;
        }

		public virtual async Task<OrderData> GetByID(string id)
		{
			var segments = MakeUrlSegments(id);
            var orderData = await GetByID<OrderData>(segments);

			return orderData;
        }
	}
}
