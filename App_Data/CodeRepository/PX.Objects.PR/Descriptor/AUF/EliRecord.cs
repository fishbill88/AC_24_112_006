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

namespace PX.Objects.PR.AUF
{
	public class EliRecord : CalculationItem
	{
		public EliRecord(DateTime checkDate, int pimID, string state, DateTime? periodStart, DateTime? periodEnd) :
			base(AufRecordType.Eli, checkDate, pimID, state, periodStart, periodEnd) { }

		public EliRecord(DateTime checkDate, int pimID, string state, DateTime? periodStart, DateTime? periodEnd, bool? paymentPeriodAlreadyExist) :
			base(AufRecordType.Eli, checkDate, pimID, state, periodStart, periodEnd, paymentPeriodAlreadyExist)
		{ }

		public override string ToString()
		{
			object[] lineData =
			{
				State,
				CheckDate,
				PimID,
				TotalWagesAndTips,
				TaxableWagesAndTips,
				TaxableTips,
				WithholdingAmount,
				Hours,
				Days,
				Weeks,
				PeriodStart,
				PeriodEnd
			};

			return FormatLine(lineData);
		}

		public bool IsResidentTax;
	}
}
