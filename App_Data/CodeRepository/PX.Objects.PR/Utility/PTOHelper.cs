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

using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.PR
{
	public class PTOHelper
	{
		private const int _NonLeapYear = 1900;
		private const int _LeapMonth = 2;
		private const int _LeapDay = 29;

		public static void GetPTOBankYear(DateTime targetDate, DateTime bankStartDate, out DateTime startDate, out DateTime endDate)
		{
			startDate = GetDateConsideringLeapYear(targetDate.Year, bankStartDate.Month, bankStartDate.Day);

			if (startDate > targetDate)
			{
				startDate = startDate.AddYears(-1);
			}

			endDate = startDate.AddYears(1).AddSeconds(-1);
		}

		public static DateTime GetDateConsideringLeapYear(int year, int month, int day)
		{
			if (!DateTime.IsLeapYear(year) && month == _LeapMonth && day == _LeapDay)
			{
				return new DateTime(year, month, day - 1);
			}
			
			return new DateTime(year, month, day);
		}

		public static PREmployeePTOBank GetBankSettings(PXGraph graph, string bankID, int employeeID, DateTime targetDate)
		{
			return PTOHelper.PTOBankSelect.View.Select(graph, employeeID, bankID).FirstTableItems
				.Where(bank => bank.StartDate.Value.Date <= targetDate)
				.OrderBy(bank => bank.StartDate).LastOrDefault();
		}

		public static PRPaymentPTOBank GetEffectivePaymentBank(IEnumerable<PRPaymentPTOBank> paymentBanks, DateTime targetDate, string bankID)
		{
			return paymentBanks.SingleOrDefault(x => x.BankID == bankID && x.EffectiveStartDate <= targetDate && targetDate < x.EffectiveEndDate);
		}

		/// <summary>
		/// Returns the hour amount that should be carried over from last year's PTO.
		/// </summary>
		public static decimal CalculateHoursToCarryover(PXGraph graph, PRPayment currentPayment, PREmployeePTOBank sourceBank, DateTime ptoYearStartDate)
		{
			IEnumerable<PRPaymentPTOBank> pastYearHistory;
			decimal? carryoverAmount = null;

			switch (sourceBank.CarryoverType)
			{
				case CarryoverType.Total:
				case CarryoverType.Partial:
					pastYearHistory = EmployeePTOHistory.Select(graph, ptoYearStartDate.AddYears(-1), currentPayment.EmployeeID.Value, sourceBank).FirstTableItems;
					carryoverAmount = pastYearHistory.Sum(x => x.TotalAccrual.GetValueOrDefault() - x.TotalDisbursement.GetValueOrDefault() + x.AdjustmentHours.GetValueOrDefault());
					break;
				case CarryoverType.None:
				default:
					return 0;
			}

			if (currentPayment != null)
			{
				// Add amount that could be accrued on same paycheck but previous PTO year.
				foreach (PRPaymentPTOBank bank in PaymentPTOBanks.View.Select(graph, currentPayment.DocType, currentPayment.RefNbr, sourceBank.BankID))
				{
					if (bank.EffectiveStartDate < ptoYearStartDate)
					{
						carryoverAmount += bank.TotalAccrual.Value - bank.TotalDisbursement.Value + bank.AdjustmentHours.Value;
					}
					else if (bank.EffectiveStartDate == ptoYearStartDate)
					{
						// Add amounts that were accrued as PTO Adjustment in the previous PTO year and should be carried over to the current year.
						carryoverAmount += bank.AdjustmentCarryoverHours;
					}
				}
			}

			if (sourceBank.CarryoverType == CarryoverType.Partial)
			{
				carryoverAmount = Math.Min(carryoverAmount.GetValueOrDefault(), sourceBank.CarryoverAmount.GetValueOrDefault());
			}
			return carryoverAmount ?? 0;
		}

		/// <summary>
		/// Returns the dollar amount that should be carried over from last year's PTO.
		/// </summary>
		public static decimal CalculateMoneyToCarryover(PXGraph graph, PRPayment currentPayment, PREmployeePTOBank sourceBank, DateTime ptoYearStartDate)
		{
			if (sourceBank?.CreateFinancialTransaction != true)
			{
				return 0m;
			}

			decimal carryoverMoney = EmployeePTOHistory.Select(graph, ptoYearStartDate.AddYears(-1), currentPayment.EmployeeID.Value, sourceBank).FirstTableItems
				.Sum(x => x.TotalAccrualMoney.GetValueOrDefault() - x.DisbursementMoney.GetValueOrDefault());

			if (currentPayment != null)
			{
				foreach (PRPaymentPTOBank bank in PaymentPTOBanks.View.Select(graph, currentPayment.DocType, currentPayment.RefNbr, sourceBank.BankID))
				{
					if (bank.EffectiveStartDate < ptoYearStartDate)
					{
						carryoverMoney += bank.TotalAccrualMoney.Value - bank.TotalDisbursementMoney.Value;
					}
				}
			}

			return carryoverMoney;
		}

		/// <summary>
		/// Calculate accumulated, used and available PTO amounts at specified date, for an employee and a bank.
		/// PTO Adjustments amounts from the previous PTO cycle will be carried over to thhe current PTO cycle.
		/// Make sure you pass the right pto bank for your needs, either the PTO bank itself, the Class bank or the Employee Bank.
		/// </summary>
		public static PTOHistoricalAmounts GetPTOHistoryForPaymentPTOBank(
			PXGraph graph,
			DateTime targetDate,
			int employeeID,
			PREmployeePTOBank bank,
			DateTime? transactionDate,
			bool includePTOAdjustments = true)
		{
			return GetPTOHistory(graph, targetDate, employeeID, bank, transactionDate, carryoverPreviousCycleAdjustments: true, includePTOAdjustments: includePTOAdjustments);
		}

		/// <summary>
		/// Calculate accumulated, used and available PTO amounts at specified date, for an employee and a bank.
		/// Make sure you pass the right pto bank for your needs, either the PTO bank itself, the Class bank or the Employee Bank.
		/// </summary>
		public static PTOHistoricalAmounts GetPTOHistory(
			PXGraph graph,
			DateTime targetDate,
			int employeeID,
			PREmployeePTOBank bank,
			DateTime? transactionDate = null,
			bool carryoverPreviousCycleAdjustments = false,
			bool includePTOAdjustments = true)
		{
			VerifyBankIsValid(bank);

			GetPTOBankYear(targetDate, bank.PTOYearStartDate.Value, out DateTime startDate, out DateTime endDate);
			PRPaymentPTOBank[] historyRecords = EmployeePTOHistory.Select(graph, startDate, employeeID, bank).FirstTableItems.ToArray();

			PTOHistoricalAmounts history = new PTOHistoricalAmounts();
			history.AccumulatedHours = historyRecords.Sum(x => x.TotalAccrual.GetValueOrDefault());
			// PTO Adjustments applied to previous paychecks
			history.AccumulatedHours += historyRecords.Sum(x => x.AdjustmentHours.GetValueOrDefault());

			if (includePTOAdjustments)
			{
				DateTime ptoAdjustmentDate = transactionDate ?? targetDate;

				history.PTOAdjustmentAmounts =
					CalculatePTOAdjustments(graph, ptoAdjustmentDate, employeeID, bank.BankID, startDate, carryoverPreviousCycleAdjustments);

				history.AccumulatedHours += history.PTOAdjustmentAmounts.AdjustmentHours;
			}

			history.AccumulatedMoney = historyRecords.Sum(x => x.TotalAccrualMoney.GetValueOrDefault());
			history.UsedHours = historyRecords.Sum(x => x.TotalDisbursement.GetValueOrDefault());
			history.UsedMoney = historyRecords.Sum(x => x.DisbursementMoney.GetValueOrDefault());

			if (bank.DisburseFromCarryover == true)
			{
				history.AvailableHours = historyRecords.Sum(x => x.CarryoverAmount.GetValueOrDefault());
				history.AvailableMoney = historyRecords.Sum(x => x.CarryoverMoney.GetValueOrDefault());
			}
			else
			{
				history.AvailableHours = history.AccumulatedHours;
				history.AvailableMoney = history.AccumulatedMoney;
			}

			history.AvailableHours -= history.UsedHours;
			history.AvailableMoney -= history.UsedMoney;

			return history;
		}

		public static PTOAdjustmentAmounts CalculatePTOAdjustments(
			PXGraph graph,
			DateTime ptoAdjustmentDate,
			int employeeID,
			string bankID,
			DateTime ptoBankYearStartDate,
			bool carryoverPreviousCycleAdjustments)
		{
			PTOAdjustmentAmounts ptoAdjustmentAmounts = new PTOAdjustmentAmounts();

			var notAppliedPTOAdjustmentDetailsQuery = SelectFrom<PRPTOAdjustmentDetail>
				.InnerJoin<PRPTOAdjustment>.On<PRPTOAdjustmentDetail.FK.PTOAdjustment>
				.Where<PRPTOAdjustment.type.IsEqual<PTOAdjustmentType.adjustment>
					.And<PRPTOAdjustment.status.IsEqual<PTOAdjustmentStatus.released>>
					.And<PRPTOAdjustmentDetail.bAccountID.IsEqual<P.AsInt>>
					.And<PRPTOAdjustmentDetail.bankID.IsEqual<P.AsString>>
					.And<PRPTOAdjustment.date.IsLessEqual<P.AsDateTime.UTC>>
					.And<PRPTOAdjustmentDetail.paymentDocType.IsNull
						.And<PRPTOAdjustmentDetail.paymentRefNbr.IsNull>
						.Or<PRPTOAdjustmentDetail.paymentDocType.IsEqual<PRPayment.docType.FromCurrent>
							.And<PRPTOAdjustmentDetail.paymentRefNbr.IsEqual<PRPayment.refNbr.FromCurrent>>>>>
				.View.Select(graph, employeeID, bankID, ptoAdjustmentDate);

			List<PRPTOAdjustmentDetail> notAppliedPTOAdjustmentDetails = new List<PRPTOAdjustmentDetail>();

			foreach (PXResult<PRPTOAdjustmentDetail, PRPTOAdjustment> record in notAppliedPTOAdjustmentDetailsQuery)
			{
				PRPTOAdjustment ptoAdjustment = record;
				PRPTOAdjustmentDetail ptoAdjustmentDetail = record;

				if (carryoverPreviousCycleAdjustments && ptoAdjustment.Date < ptoBankYearStartDate)
				{
					ptoAdjustmentAmounts.AdjustmentCarryoverHours += ptoAdjustmentDetail.AdjustmentHours.GetValueOrDefault();
				}
				else
				{
					ptoAdjustmentAmounts.AdjustmentHours += ptoAdjustmentDetail.AdjustmentHours.GetValueOrDefault();
				}

				notAppliedPTOAdjustmentDetails.Add(ptoAdjustmentDetail);
			}

			ptoAdjustmentAmounts.PTOAdjustmentDetails = notAppliedPTOAdjustmentDetails.ToArray();

			return ptoAdjustmentAmounts;
		}

		public static PTOYearSummary GetPTOYearSummary(PXGraph graph, DateTime targetDate, int employeeID, PREmployeePTOBank bank)
		{
			GetPTOBankYear(targetDate, bank.PTOYearStartDate.Value, out DateTime startDate, out DateTime endDate);
			var results = EmployeePTOHistory.Select(graph, targetDate, employeeID, bank.PTOYearStartDate.Value, bank.BankID);
			var history = results.Select(x => (PXResult<PRPaymentPTOBank, PRPayment>)x).ToList();

			PTOYearSummary summary = new PTOYearSummary();
			summary.StartDate = startDate;
			summary.EndDate = endDate;
			summary.AccrualAmount = history.Sum(x => ((PRPaymentPTOBank)x).AccrualAmount.GetValueOrDefault());
			summary.AccrualMoney = history.Where(x => ((PRPaymentPTOBank)x).CreateFinancialTransaction == true).Sum(x => ((PRPaymentPTOBank)x).AccrualMoney.GetValueOrDefault());
			summary.DisbursementAmount = history.Sum(x => ((PRPaymentPTOBank)x).DisbursementAmount.GetValueOrDefault());
			summary.DisbursementMoney = history.Where(x => ((PRPaymentPTOBank)x).CreateFinancialTransaction == true).Sum(x => ((PRPaymentPTOBank)x).DisbursementMoney.GetValueOrDefault());
			summary.FrontLoadingAmount = history.Sum(x => ((PRPaymentPTOBank)x).FrontLoadingAmount.GetValueOrDefault());
			summary.CarryoverAmount = history.Sum(x => ((PRPaymentPTOBank)x).CarryoverAmount.GetValueOrDefault());
			summary.CarryoverMoney = history.Where(x => ((PRPaymentPTOBank)x).CreateFinancialTransaction == true).Sum(x => ((PRPaymentPTOBank)x).CarryoverMoney.GetValueOrDefault());
			summary.SettlementDiscardAmount = history.Sum(x => ((PRPaymentPTOBank)x).SettlementDiscardAmount.GetValueOrDefault());

			summary.ProcessedFrontLoading = history.Any(x => ((PRPaymentPTOBank)x).ProcessedFrontLoading == true && ((PRPayment)x).Voided == false && ((PRPayment)x).DocType != PayrollType.VoidCheck);
			summary.ProcessedCarryover = history.Any(x => ((PRPaymentPTOBank)x).ProcessedCarryover == true && ((PRPayment)x).Voided == false && ((PRPayment)x).DocType != PayrollType.VoidCheck);

			return summary;
		}

		protected static void VerifyBankIsValid(PREmployeePTOBank bank)
		{
			if (bank.PTOYearStartDate == null)
			{
				throw new PXException(Messages.InvalidBankStartDate);
			}
		}

		public static IEnumerable<PREmployeePTOBank> GetEmployeeBanks(PXGraph graph, PRPayment payment)
		{
			return SelectFrom<PREmployeePTOBank>
				.Where<PREmployeePTOBank.bAccountID.IsEqual<P.AsInt>.
					And<PREmployeePTOBank.startDate.IsLessEqual<P.AsDateTime.UTC>>>.View
				.Select(graph, payment.EmployeeID, payment.EndDate).FirstTableItems
				.OrderBy(bank => bank.StartDate);
		}

		public static PREmployeePTOBank GetNextEffectiveBankInTheSameYear(PXGraph graph,
			PRPayment payment,
			string bankId,
			DateTime ptoYearStart,
			DateTime effectiveStartDate)
		{
			return SelectFrom<PREmployeePTOBank>
				.Where<PREmployeePTOBank.bAccountID.IsEqual<P.AsInt>
					.And<PREmployeePTOBank.bankID.IsEqual<P.AsString>>
					.And<Where<DatePart<DatePart.year, PREmployeePTOBank.startDate>, Equal<P.AsInt>>>
					.And<PREmployeePTOBank.startDate.IsGreater<P.AsDateTime.UTC>>>.View
				.Select(graph, payment.EmployeeID, bankId, ptoYearStart.Year, effectiveStartDate).FirstTableItems
				.OrderBy(x => x.StartDate).FirstOrDefault();
		}

		public static IEnumerable<PRPaymentPTOBank> GetLastEffectiveBanks(IEnumerable<PRPaymentPTOBank> banks)
		{
			return banks.OrderBy(x => x.EffectiveStartDate).GroupBy(x => x.BankID).Select(x => x.Last());
		}

		public static PRPaymentPTOBank GetLastEffectiveBank(IEnumerable<PRPaymentPTOBank> banks, string bankID)
		{
			return GetLastEffectiveBanks(banks).Single(x => x.BankID == bankID);
		}

		public static IEnumerable<PREmployeePTOBank> GetFirstEffectiveBanks(IEnumerable<PREmployeePTOBank> banks)
		{
			return banks.OrderBy(x => x.StartDate).GroupBy(x => x.BankID).Select(x => x.First());
		}

		public static IEnumerable<PREmployeePTOBank> GetLastEffectiveBanks(IEnumerable<PREmployeePTOBank> banks)
		{
			return banks.OrderBy(x => x.StartDate).GroupBy(x => x.BankID).Select(x => x.Last());
		}

		public static bool SpansTwoPTOYears(DateTime bankEffectiveDate, DateTime periodStartDate, DateTime periodEndDate)
		{
			GetPTOBankYear(periodStartDate, bankEffectiveDate, out DateTime paymentStartPTOYearStart, out DateTime paymentStartPTOYearEnd);
			GetPTOBankYear(periodEndDate, bankEffectiveDate, out DateTime paymentEndPTOYearStart, out DateTime paymentEndPTOYearEnd);
			return paymentStartPTOYearStart != paymentEndPTOYearStart && paymentStartPTOYearEnd != paymentEndPTOYearEnd;
		}

		public static int GetPTOEffectiveDay(DateTime? bankStartDate, DateTime? bankPTOYearStartDate, DateTime ptoYearStart, DateTime ptoYearEnd)
		{
			return ptoYearStart <= bankStartDate && bankStartDate <= ptoYearEnd ? bankStartDate.Value.Day : bankPTOYearStartDate.Value.Day;
		}

		public static DateTime GetPTOEffectiveStartDate(DateTime? bankStartDate, DateTime ptoYearStart, int day)
		{
			if (bankStartDate.Value.Year != ptoYearStart.Year)
			{
				if (bankStartDate.Value.Year > ptoYearStart.Year)
				{
					return bankStartDate.GetValueOrDefault();
				}
				else
				{
					return new DateTime(ptoYearStart.Year, ptoYearStart.Month, day);
				}
			}

			if (ptoYearStart.DayOfYear > bankStartDate.Value.DayOfYear)
			{
				return new DateTime(ptoYearStart.Year, ptoYearStart.Month, day);
			}

			return new DateTime(ptoYearStart.Year, bankStartDate.Value.Month, bankStartDate.Value.Day);
		}

		public static DateTime CalculatePTOTransferDate(PRPTOBank bank, DateTime? hireDate)
		{
			if (bank.TransferDateType == TransferDateType.SpecificDate || hireDate == null)
			{
				return new DateTime(_NonLeapYear, bank.StartDate.Value.Month, bank.StartDate.Value.Day);
			}
			else
			{
				return new DateTime(_NonLeapYear, hireDate.Value.Month, hireDate.Value.Day);
			}
		}

		public static DateTime GetPTOEffectiveDate(DateTime hireDate, DateTime transferDate, int yearsOfService, string roundingMethod)
		{
			DateTime dateAfterYearsOfService = hireDate.AddYears(yearsOfService);
			DateTime effectiveDate = new DateTime(hireDate.Year + yearsOfService, transferDate.Month, transferDate.Day);

			if (dateAfterYearsOfService == effectiveDate)
			{
				return effectiveDate;
			}
			else if (dateAfterYearsOfService > effectiveDate)
			{
				return roundingMethod == BandingRuleRoundingMethod.RoundUp ? effectiveDate : effectiveDate.AddYears(1);
			}
			else
			{
				return roundingMethod == BandingRuleRoundingMethod.RoundUp ? effectiveDate.AddYears(-1) : effectiveDate;
			}
		}

		public static class EmployeePTOHistory
		{
			protected class EmployeePTOHistorySelect : SelectFrom<PRPaymentPTOBank>
			.InnerJoin<PRPayment>.On<PRPaymentPTOBank.FK.Payment>
			.LeftJoin<PREarningDetail>.On<PREarningDetail.FK.Payment>
			.Where<PRPayment.employeeID.IsEqual<P.AsInt>
				.And<PRPaymentPTOBank.effectiveStartDate.IsBetween<P.AsDateTime.UTC, P.AsDateTime.UTC>>
				.And<PRPaymentPTOBank.bankID.IsEqual<P.AsString>>
				.And<PRPayment.released.IsEqual<True>>>
			.AggregateTo<
				GroupBy<PRPaymentPTOBank.docType>,
				GroupBy<PRPaymentPTOBank.refNbr>,
				GroupBy<PRPaymentPTOBank.bankID>,
				GroupBy<PRPaymentPTOBank.effectiveStartDate>>
			{ }

			public static PXResultset<PRPaymentPTOBank> Select(PXGraph graph, DateTime targetDate, int employeeID, DateTime bankStartDate, string bankID)
			{
				var results = new PXResultset<PRPaymentPTOBank>();
				GetPTOBankYear(targetDate, bankStartDate, out DateTime ptoYearStartDate, out DateTime ptoYearEndDate);
				foreach (PXResult<PRPaymentPTOBank, PRPayment, PREarningDetail> result in EmployeePTOHistorySelect.View.Select(graph, employeeID, ptoYearStartDate, ptoYearEndDate, bankID))
				{
					PRPaymentPTOBank paymentBank = result;
					// In previous versions that didn't have overlaping PTO year in the same payment, bank start date are in year 1900 instead of paycheck's year.
					if (paymentBank.EffectiveStartDate?.Year == 1900 || ptoYearStartDate <= paymentBank.EffectiveStartDate && paymentBank.EffectiveStartDate <= ptoYearEndDate)
					{
						results.Add(result);
					}
				}

				return results;
			}

			public static PXResultset<PRPaymentPTOBank> Select(PXGraph graph, DateTime targetDate, int employeeID, PREmployeePTOBank bank)
			{
				return Select(graph, targetDate, employeeID, bank.PTOYearStartDate.Value, bank.BankID);
			}
		}

		public class PTOBankSelect : SelectFrom<PREmployeePTOBank>
			.Where<PREmployeePTOBank.bAccountID.IsEqual<P.AsInt>
				.And<PREmployeePTOBank.bankID.IsEqual<P.AsString>>>
		{ }

		public class PaymentPTOBanks : SelectFrom<PRPaymentPTOBank>
			.Where<PRPaymentPTOBank.docType.IsEqual<P.AsString>
				.And<PRPaymentPTOBank.refNbr.IsEqual<P.AsString>>
				.And<PRPaymentPTOBank.bankID.IsEqual<P.AsString>>>
		{ }

		public class PTOYearSummary : IPTOHistory
		{
			public DateTime StartDate { get; set; }

			public DateTime EndDate { get; set; }

			public decimal? AccrualAmount { get; set; }

			public decimal? AccrualMoney { get; set; }

			public decimal? DisbursementAmount { get; set; }

			public decimal? DisbursementMoney { get; set; }

			public bool? ProcessedFrontLoading { get; set; }

			public decimal? FrontLoadingAmount { get; set; }

			public bool? ProcessedCarryover { get; set; }

			public decimal? CarryoverAmount { get; set; }

			public decimal? CarryoverMoney { get; set; }

			public decimal? SettlementDiscardAmount { get; set; }

			public decimal TotalIncreasedHours => AccrualAmount.GetValueOrDefault() + FrontLoadingAmount.GetValueOrDefault() + CarryoverAmount.GetValueOrDefault();

			public decimal TotalDecreasedHours => DisbursementAmount.GetValueOrDefault() + SettlementDiscardAmount.GetValueOrDefault();

			public decimal BalanceHours => TotalIncreasedHours - TotalDecreasedHours;

			public decimal TotalIncreasedMoney => AccrualMoney.GetValueOrDefault() + CarryoverMoney.GetValueOrDefault();

			public decimal TotalDecreasedMoney => DisbursementMoney.GetValueOrDefault();

			public decimal BalanceMoney => TotalIncreasedMoney - TotalDecreasedMoney;
		}

		public interface IPTOHistory
		{
			decimal? AccrualAmount { get; set; }

			decimal? AccrualMoney { get; set; }

			decimal? DisbursementAmount { get; set; }

			decimal? DisbursementMoney { get; set; }

			bool? ProcessedFrontLoading { get; set; }

			decimal? FrontLoadingAmount { get; set; }

			bool? ProcessedCarryover { get; set; }

			decimal? CarryoverAmount { get; set; }

			decimal? CarryoverMoney { get; set; }

			decimal? SettlementDiscardAmount { get; set; }

		}

		public class PTOHistoricalAmounts
		{
			public decimal AccumulatedHours = 0;
			public decimal AccumulatedMoney = 0;
			public decimal UsedHours = 0;
			public decimal UsedMoney = 0;
			public decimal AvailableHours = 0;
			public decimal AvailableMoney = 0;
			public PTOAdjustmentAmounts PTOAdjustmentAmounts = new PTOAdjustmentAmounts();
		}

		public class PTOAdjustmentAmounts
		{
			public decimal AdjustmentHours = 0;
			public decimal AdjustmentCarryoverHours = 0;
			public PRPTOAdjustmentDetail[] PTOAdjustmentDetails = new PRPTOAdjustmentDetail[0];
		}
	}
}
