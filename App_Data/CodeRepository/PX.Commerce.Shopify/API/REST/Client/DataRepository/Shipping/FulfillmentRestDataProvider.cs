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

namespace PX.Commerce.Shopify.API.REST
{
	public class FulfillmentRestDataProviderFactory : ISPRestDataProviderFactory<IFulfillmentRestDataProvider>
	{
		public virtual IFulfillmentRestDataProvider CreateInstance(IShopifyRestClient restClient) => new FulfillmentRestDataProvider(restClient);
	}

	public class FulfillmentRestDataProvider : RestDataProviderBase, IChildRestDataProvider<FulfillmentData>, IFulfillmentRestDataProvider
	{
		protected override string GetListUrl { get; } = "fulfillments.json";
		protected override string GetSingleUrl { get; } = "orders/{parent_id}/fulfillments/{id}.json";
		protected override string GetSearchUrl => throw new NotImplementedException();
		private string GetUpdateTrackingUrl { get; } = "fulfillments/{id}/update_tracking.json";
		private string GetCancelUrl { get; } = "fulfillments/{id}/cancel.json";
		protected string GetListByFulfillOrderUrl { get; } = "fulfillment_orders/{parent_id}/fulfillments.json";
		protected string GetListByOrderUrl { get; } = "orders/{parent_id}/fulfillments.json";

		public FulfillmentRestDataProvider(IShopifyRestClient restClient) : base()
		{
			ShopifyRestClient = restClient;
		}

		public virtual async Task<FulfillmentData> Create(FulfillmentData entity, string orderId = null)
		{
			return await Create<FulfillmentData, FulfillmentResponse>(entity);
		}

		public virtual async Task<FulfillmentData> Update(FulfillmentData entity, string orderId, string fulfillmentId) => throw new NotImplementedException();

		public virtual async Task<bool> Delete(string fulfillmentId, string orderId = null)
		{
			return await CancelFulfillment(orderId, fulfillmentId) != null;			
		}

		public virtual async IAsyncEnumerable<FulfillmentData> GetAll(string orderId, IFilter filter = null, CancellationToken cancellationToken = default)
		{
			var segments = MakeUrlSegments(orderId);
			var request = BuildRequest(GetListByOrderUrl, nameof(this.GetAll), segments, filter);
			await  foreach (var data in ShopifyRestClient.GetAll<FulfillmentData, FulfillmentsResponse>(request, cancellationToken))
				yield return data;
		}

		public virtual async Task< FulfillmentData> GetByID(string orderId, string fulfillmentId)
		{
			var segments = MakeUrlSegments(fulfillmentId, orderId);
			return await GetByID<FulfillmentData, FulfillmentResponse>(segments);
		}

		public virtual async Task<FulfillmentData> UpdateFulfillmentTracking(FulfillmentData entity, string orderId, string fulfillmentId)
		{
			var request = BuildRequest(GetUpdateTrackingUrl, nameof(UpdateFulfillmentTracking), MakeUrlSegments(fulfillmentId), null);
			return await ShopifyRestClient.Post<FulfillmentData, FulfillmentResponse>(request, entity);
		}

		public virtual async Task<FulfillmentData> CancelFulfillment(string orderId, string fulfillmentId)
		{
			var request = BuildRequest(GetCancelUrl, nameof(CancelFulfillment), MakeUrlSegments(fulfillmentId), null);
			return await ShopifyRestClient.Post<FulfillmentData, FulfillmentResponse>(request, new FulfillmentData() { }, false);
		}

		public virtual async IAsyncEnumerable<FulfillmentData> GetAllByFulfillmentOrder(string fulfillmentOrderId)
		{
			var request = BuildRequest(GetListByFulfillOrderUrl, nameof(GetAllByFulfillmentOrder), MakeParentUrlSegments(fulfillmentOrderId), null);
			 await foreach(var data in ShopifyRestClient.GetAll<FulfillmentData, FulfillmentsResponse>(request))
				yield return data;
		}

		public IAsyncEnumerable<FulfillmentData> GetAllWithoutParent(IFilter filter = null)
		{
			throw new NotImplementedException();
		}
	}
}
