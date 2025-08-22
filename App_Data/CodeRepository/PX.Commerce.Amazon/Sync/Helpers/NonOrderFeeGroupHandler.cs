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

using PX.Commerce.Amazon.API.Rest;
using PX.Commerce.Amazon.API;
using PX.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using PX.Commerce.Amazon.API.Rest.Client.Interface;
using System.Linq;
using PX.Commerce.Amazon.Sync.Interfaces;

namespace PX.Commerce.Amazon.Sync.Helpers
{
	public class NonOrderFeeGroupHandler : INonOrderFeeGroupHandler
	{
		private readonly INonOrderFinanceEventsDataProvider _financeEventsDataProvider;

		private readonly IStatementPeriodParser _statementPeriodParser;

		public NonOrderFeeGroupHandler(INonOrderFinanceEventsDataProvider financeEventsDataProvider, IStatementPeriodParser statementPeriodParser)
		{
			_financeEventsDataProvider = financeEventsDataProvider;
			_statementPeriodParser = statementPeriodParser;
		}

		private async Task<FinancialEventGroup> GetFinancialGroupByPeriodAsync(string statementPeriod, string storeCurrency, FinancialEventGroupsFilter filter, CancellationToken cancellationToken)
		{
			await Task.Yield();

			await foreach (FinancialEventGroup financialGroup in _financeEventsDataProvider.GetFinancialGroupsAsync(filter, storeCurrency, cancellationToken))
			{
				if (_statementPeriodParser.PrepareStatementPeriod(financialGroup.FinancialEventGroupStart, financialGroup.FinancialEventGroupEnd) == statementPeriod)
					return financialGroup;
			}

			throw new PXException(AmazonMessages.FinancialGroupStetementPeriodIsMissing);
		}

		public async Task<NonOrderFeeGroup> PrepareNonOrderFeeGroupAsync(string statementPeriod, string storeCurrency, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(statementPeriod))
				throw new PXArgumentException(nameof(statementPeriod), ErrorMessages.ArgumentNullException);

			if (string.IsNullOrWhiteSpace(storeCurrency))
				throw new PXArgumentException(nameof(storeCurrency), ErrorMessages.ArgumentNullException);

			(DateTime startsFrom, DateTime endsWith) = _statementPeriodParser.ParseDatePeriod(statementPeriod);
			var filter = new FinancialEventGroupsFilter { FinancialEventGroupStartedAfter = startsFrom, FinancialEventGroupStartedBefore = endsWith };
			FinancialEventGroup financialEventGroup = await this.GetFinancialGroupByPeriodAsync(statementPeriod, storeCurrency, filter, cancellationToken);

			if (financialEventGroup.ProcessingStatus == ProcessingStatus.Closed
				&& (!financialEventGroup.FinancialEventGroupEnd.HasValue || !financialEventGroup.FinancialEventGroupStart.HasValue))
			{
				throw new PXException(AmazonMessages.NonOrderFeeDescriptionIsIncorrect);
			}

			NonOrderFeeGroup nonOrderFeeGroup = new NonOrderFeeGroup { FinancialEventGroup = financialEventGroup };

			await Task.Yield();

			await foreach (var eventsByDay in _financeEventsDataProvider.GetFinancialEventsAsync(financialEventGroup, cancellationToken))
				nonOrderFeeGroup.FinancialEventsByDate.Add(eventsByDay);

			return nonOrderFeeGroup;
		}
	}
}
