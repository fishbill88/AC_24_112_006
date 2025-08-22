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

using PX.Commerce.Amazon.API.Rest.Client.Interface;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Threading;

namespace PX.Commerce.Amazon.API.Rest
{
	public class OrderFinanceEventsDataProvider : RestDataProviderBase, IOrderFinanceEventsDataProvider
	{
		public OrderFinanceEventsDataProvider(IAmazonRestClient restClient)
			: base(restClient)
		{
		}

		protected override string GetListUrl => "/finances/v0/financialEvents";

		protected override string GetSingleUrl => throw new NotImplementedException();

		private string FinancialEventsByOrderIdUrl = "/finances/v0/orders/{id}/financialEvents";

		public async IAsyncEnumerable<OrderFinancialEvents> GetFinancialEventsByOrderAsync(FinancialEventsFilterByOrder filter, CancellationToken cancellationToken = default)
		{
			if (filter is null)
				throw new PXArgumentException(nameof(filter), ErrorMessages.ArgumentNullException);

			if (string.IsNullOrWhiteSpace(filter.OrderId))
				throw new PXArgumentException(nameof(filter.OrderId), ErrorMessages.FieldIsEmpty, nameof(filter.OrderId));

			var urlSegments = MakeUrlSegments(filter.OrderId);

			await foreach (var item in base.GetAll<OrderFinancialEvents, GetOrderFinancialEventsResponse, OrderFinancialEventsPage>(urlSegments: urlSegments, url: this.FinancialEventsByOrderIdUrl, cancellationToken: cancellationToken))
				yield return item;
		}

		public async IAsyncEnumerable<OrderFinancialEvents> GetFinancialEventsAsync(FinancialEventsFilter filter, CancellationToken cancellationToken = default)
		{
			await foreach (var item in base.GetAll<OrderFinancialEvents, GetOrderFinancialEventsResponse, OrderFinancialEventsPage>(filter,url: this.GetListUrl, cancellationToken: cancellationToken))
				yield return item;
		}
	}
}
