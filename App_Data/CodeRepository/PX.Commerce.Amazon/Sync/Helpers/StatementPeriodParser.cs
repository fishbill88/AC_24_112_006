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

using PX.Commerce.Amazon.Sync.Interfaces;
using PX.Data;
using System;
using System.Linq;

namespace PX.Commerce.Amazon.Sync.Helpers
{
	public class StatementPeriodParser : IStatementPeriodParser
	{
		private const string FinancialGroupPeriodSeparator = " - ";

		public string PrepareStatementPeriod(DateTime? financialEventGroupStartDate, DateTime? financialEventGroupEndDate)
		{
			if (!financialEventGroupStartDate.HasValue)
				throw new PXArgumentException(nameof(financialEventGroupStartDate), ErrorMessages.ArgumentNullException);

			if (!financialEventGroupEndDate.HasValue)
				throw new PXArgumentException(nameof(financialEventGroupEndDate), ErrorMessages.ArgumentNullException);

			return $"{financialEventGroupStartDate.Value.ToShortDateString()}{FinancialGroupPeriodSeparator}{financialEventGroupEndDate.Value.ToShortDateString()}";
		}

		public (DateTime StartsFrom, DateTime EndsWith) ParseDatePeriod(string statementPeriod)
		{
			if (string.IsNullOrWhiteSpace(statementPeriod))
				throw new PXArgumentException(nameof(statementPeriod), ErrorMessages.ArgumentNullException);

			var dates = statementPeriod.Split(new string[] { FinancialGroupPeriodSeparator }, StringSplitOptions.None);

			if (!dates.Any() || dates.Length != 2)
				throw new PXException(AmazonMessages.NonOrderFeeDescriptionIsIncorrect);

			if (!DateTime.TryParse(dates[0], out DateTime startsFrom))
				throw new PXException(AmazonMessages.FinancialGroupStartDateCantBeParsed);

			if (!DateTime.TryParse(dates[1], out DateTime endsWith))
				throw new PXException(AmazonMessages.FinancialGroupEndDateCantBeParsed);

			return (startsFrom, endsWith);
		}
	}
}
