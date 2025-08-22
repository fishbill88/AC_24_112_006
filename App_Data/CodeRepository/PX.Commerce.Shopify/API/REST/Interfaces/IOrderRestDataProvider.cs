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

namespace PX.Commerce.Shopify.API.REST
{
	public interface IOrderRestDataProvider
	{
		Task<OrderData> CancelOrder(string orderId);
		Task<OrderData> CloseOrder(string orderId);
		Task<OrderData> Create(OrderData entity);
		Task<bool> Delete(string id);
		IAsyncEnumerable<OrderData> GetAll(IFilter filter = null, CancellationToken cancellationToken = default);
		Task<OrderData> GetByID(string id);
		Task<OrderData> GetByID(string id, bool includedMetafields = false, bool includedTransactions = false, bool includedCustomer = true, bool includedOrderRisk = false, CancellationToken cancellationToken = default);
		Task<List<MetafieldData>> GetMetafieldsById(string id, CancellationToken cancellationToken = default);
		Task<CustomerData> GetOrderCustomer(string orderId);
		Task<List<OrderRisk>> GetOrderRisks(string orderId, CancellationToken cancellationToken = default);
		Task<OrderTransaction> GetOrderSingleTransaction(string orderId, string transactionId);
		Task<List<OrderTransaction>> GetOrderTransactions(string orderId, CancellationToken cancellationToken = default);
		Task<OrderTransaction> PostPaymentToCapture(OrderTransaction entity, string orderId);
		Task<OrderData> ReopenOrder(string orderId);
		Task<OrderData> Update(OrderData entity);
		Task<OrderData> Update(OrderData entity, string id);
	}
}
