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
using PX.Commerce.Amazon.API.Rest.Domain.Entities.Finances;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Threading;

namespace PX.Commerce.Amazon.API.Rest
{
	public class NonOrderFinanceEventsDataProvider : RestDataProviderBase, INonOrderFinanceEventsDataProvider
	{
		public NonOrderFinanceEventsDataProvider(IAmazonRestClient restClient)
			: base(restClient)
		{
		}

		protected override string GetListUrl => "/finances/v0/financialEvents";

		protected override string GetSingleUrl => throw new NotImplementedException();

		private string FinancialEventsByGroupIdUrl = "/finances/v0/financialEventGroups/{id}/financialEvents";

		private string FinancialEventGroupsUrl = "/finances/v0/financialEventGroups";

		public async IAsyncEnumerable<NonOrderFinancialEvents> GetFinancialEventsByGroupAsync(FinancialEventsFilterByGroup filter, CancellationToken cancellationToken = default)
		{
			if (filter is null)
				throw new PXArgumentException(nameof(filter), ErrorMessages.ArgumentNullException);

			if (string.IsNullOrWhiteSpace(filter.FinancialEventGroupId))
				throw new PXArgumentException(nameof(filter.FinancialEventGroupId), ErrorMessages.FieldIsEmpty, nameof(filter.FinancialEventGroupId));

			var urlSegments = MakeUrlSegments(filter.FinancialEventGroupId);

			await foreach (var item in base.GetAll<NonOrderFinancialEvents, GetNonOrderFinancialEventsResponse, NonOrderFinancialEventsPage>(filter, urlSegments, this.FinancialEventsByGroupIdUrl))
				yield return item;
		}

		public async IAsyncEnumerable<FinancialEventGroup> GetFinancialGroupsAsync(FinancialEventGroupsFilter financialEventGroupsFilter, string storeCurrency, CancellationToken cancellationToken = default)
		{
			if (financialEventGroupsFilter is null)
				throw new PXArgumentException(nameof(financialEventGroupsFilter), ErrorMessages.ArgumentNullException);

			await foreach (FinancialEventGroup financialGroup in base.GetAll<FinancialEventGroup, GetFinancialEventGroupsResponse, FinancialEventGroupList>(financialEventGroupsFilter, url: this.FinancialEventGroupsUrl))
			{
				if (financialGroup.BeginningBalance != null && financialGroup.BeginningBalance.CurrencyCode != storeCurrency)
					continue;

				if (financialGroup.OriginalTotal != null && financialGroup.OriginalTotal.CurrencyCode != storeCurrency)
					continue;

				if (!financialGroup.FinancialEventGroupStart.HasValue)
					continue;

				if (!financialGroup.FinancialEventGroupEnd.HasValue)
					continue;

				yield return financialGroup;
			}
		}

		public async IAsyncEnumerable<(NonOrderFinancialEvents Events, DateTime Date)> GetFinancialEventsAsync(FinancialEventGroup financialEventGroup, CancellationToken cancellationToken = default)
		{
			if (financialEventGroup is null)
				throw new PXArgumentException(nameof(financialEventGroup), ErrorMessages.ArgumentNullException);

			DateTime startFrom = financialEventGroup.FinancialEventGroupStart.Value;
			DateTime endWith = financialEventGroup.FinancialEventGroupEnd.Value;
			int totalDays = (int)Math.Ceiling((endWith - startFrom).TotalDays);

			for (int dayCounter = 1; dayCounter <= totalDays; dayCounter++)
			{
				if (dayCounter == 1)
				{
					//
					// Handle the first period:
					// StartFrom 01.01.2000 17:15:01
					// 01.01.2000 17:15:01 - 01.02.2000 00:00:00
					//
					endWith = startFrom.Date.AddDays(1);
				}
				else if (dayCounter == totalDays)
				{
					//
					// Handle the last period:
					// EndsWith 10.01.2000 11:24:58
					// 10.01.2000 00:00:00 - 10.01.2000 11:24:58
					//
					endWith = financialEventGroup.FinancialEventGroupEnd.Value;
				}
				else
				{
					//
					// Handle regular period
					// 01.01.2000 00:00:00 - 02.01.2000 00:00:00
					//
					startFrom = startFrom.Date;
					endWith = endWith.AddDays(1);
				}

				var filter = new FinancialEventsFilterByGroup
				{
					FinancialEventGroupId = financialEventGroup.FinancialEventGroupId,
					PostedAfter = startFrom,
					PostedBefore = endWith
				};

				await foreach (NonOrderFinancialEvents financialEvents in this.GetFinancialEventsByGroupAsync(filter, cancellationToken))
				{
					if (!financialEvents.IsValid)
						continue;

					yield return (financialEvents, startFrom.Date.ToLocalTime());
				}

				startFrom = startFrom.AddDays(1);
			}
		}
	}
}
