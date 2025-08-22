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

using PX.Api;
using PX.Commerce.Core;
using PX.Objects.Common.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon.API.Rest
{
	public class OrderDataProvider : RestDataProviderBase, IOrderDataProvider
	{
		public OrderDataProvider(IAmazonRestClient restClient)
			: base(restClient)
		{
		}

		protected override string GetListUrl { get; } = "/orders/v0/orders";

		protected override string GetSingleUrl { get; } = "orders/v0/orders/{id}";

		private string GetOrderItemsUrl { get; } = "orders/v0/orders/{id}/orderItems";

		private string GetOrderBuyerInfoUrl { get; } = "orders/v0/orders/{id}/buyerInfo";

		private string GetOrderAddressUrl { get; } = "/orders/v0/orders/{id}/address";

		private static CreateRestrictedDataTokenRequest CreateRestrictedTokenRequest()
		{
			return new CreateRestrictedDataTokenRequest()
			{
				RequestType = BCEntitiesAttribute.Order,
				RestrictedResources = new RestrictedResource[]
				{
					new RestrictedResource
					{
						Method= "GET",
						Path = "/orders/v0/orders/{orderId}/orderItems",
						DataElements= new string[]{"buyerInfo"}
					},
					new RestrictedResource
					{
						Method= "GET",
						Path = "/orders/v0/orders/{orderId}",
						DataElements= new string[]{ "buyerInfo", "shippingAddress" }
					}
				}
			};
		}

		public virtual async Task<Order> GetById(string orderId)
		{
			var entity = await base.GetByID<Order, GetOrderResponse>(MakeUrlSegments(orderId), createRestrictedDataToken: CreateRestrictedTokenRequest());
			if (entity?.AmazonOrderId != null)
			{
				// It needs to use Task.Yield to avoid situations when async code runs synchroniously. This instruction forces the code to be asynchronous
				await Task.Yield();
				List<OrderItem> listOfOrders = new List<OrderItem>();
				await foreach (var item in GetOrderItems(orderId))
				{
					listOfOrders.Add(item);
				}

				entity.OrderItems = listOfOrders ?? new List<OrderItem>();

				if (entity.BuyerInfo == null)
					entity.BuyerInfo = await GetOrderBuyerInfo(orderId);
				if (entity.ShippingAddress == null)
					entity.ShippingAddress = await GetOrderAddress(orderId);
			}
			return entity;
		}

		public virtual async IAsyncEnumerable<OrderItem> GetOrderItems(string orderId)
		{
			await foreach (var item in base.GetAll<OrderItem, GetOrderItemsResponse, OrderItemsList>(urlSegments: MakeUrlSegments(orderId), url: GetOrderItemsUrl, createRestrictedDataToken: CreateRestrictedTokenRequest()))
			{
				yield return item;
			}
		}

		public virtual async IAsyncEnumerable<Order> GetAll(Filter filter, CancellationToken cancellationToken)
		{
			await foreach (var item in base.GetAll<Order, GetOrdersResponse, OrdersList>(filter, cancellationToken : cancellationToken))
			{
				yield return item;
			}
		}

		public virtual async Task<Address> GetOrderAddress(string orderId)
		{
			return (await base.GetByID<OrderAddress, GetOrderAddressResponse>(urlSegments: MakeUrlSegments(orderId), url: GetOrderAddressUrl))?.ShippingAddress;
		}

		public virtual async Task<BuyerInfo> GetOrderBuyerInfo(string orderId)
		{
			return await base.GetByID<BuyerInfo, GetOrderBuyerInfoResponse>(urlSegments: MakeUrlSegments(orderId), url: GetOrderBuyerInfoUrl);
		}
	}
}
