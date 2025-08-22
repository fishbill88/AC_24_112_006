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

using PX.Objects.FA.DepreciationMethods.Parameters;
using PX.Objects.GL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.FA.DepreciationMethods
{
	public class DutchMethod2 : DepreciationMethodBase
	{
		protected override string CalculationMethod => FADepreciationMethod.depreciationMethod.Dutch2;
		protected override string[] ApplicableAveragingConventions { get; } = { FAAveragingConvention.FullPeriod };

		protected override void ApplyDispose(SortedDictionary<string, SortedDictionary<string, FADepreciationScheduleItem>> schedules) { }

		protected override SortedDictionary<string, SortedDictionary<string, FADepreciationScheduleItem>> Calculate()
		{
			SortedDictionary<string, SortedDictionary<string, FADepreciationScheduleItem>> depreciationSchedule = new SortedDictionary<string, SortedDictionary<string, FADepreciationScheduleItem>>();

			foreach (FAAddition addition in CalculationParameters.Additions)
			{
				SortedDictionary<string, FADepreciationScheduleItem> depreciationAdditionSchedule = new SortedDictionary<string, FADepreciationScheduleItem>();
				DutchMethod2AdditionParameters additionParameters = CalculateAdditionParameters(CalculationParameters, addition);
				string lastDeprPeriodWithSuspend = IncomingParameters.PeriodDepreciationUtils
													.AddPeriodsCountToPeriod(additionParameters.DepreciateToPeriodID, IncomingParameters.SuspendedPeriodsIDs.Count);
				List<string> periods = GetAdditionPeriods(additionParameters.DepreciateFromPeriodID, lastDeprPeriodWithSuspend);

				decimal accumulatedDepretiation = 0;
				decimal accumulatedDepretiationOfFinYear = 0;
				decimal depriciationBaseAmount = additionParameters.DepreciationBasis;
				decimal deprericiationRate = 0;
				FinYearInfo info = null;

				IYearSetup yearSetup = IncomingParameters.PeriodDepreciationUtils.YearSetup;

				Dictionary<string, FinYearInfo> map = GetFinYearInfo(periods, yearSetup);

				foreach (string period in periods)
				{
					if (string.CompareOrdinal(period, CalculationParameters.MaxDepreciateToPeriodID) > 0) break;

					FABookPeriod currentPeriod = IncomingParameters.PeriodDepreciationUtils.BookPeriods[period];

					if ((info == null || info.FinYear != currentPeriod.FinYear) && map.TryGetValue(currentPeriod.FinYear, out info))
					{
						deprericiationRate = (decimal)(1 - Math.Pow(1 - (double)(IncomingParameters.Method.PercentPerYear / 100), (double)(1 / info.NumberOfPeriodsInFinYear)));
						depriciationBaseAmount = additionParameters.DepreciationBasis - accumulatedDepretiation;
						accumulatedDepretiationOfFinYear = 0;
					}

					decimal currentPeriodDepreciationAmount = 0;

					if (IncomingParameters.Method.YearlyAccountancy == true)
					{
						currentPeriodDepreciationAmount = depriciationBaseAmount * (IncomingParameters.Method.PercentPerYear.Value / 100) / info.NumberOfPeriodsInFinYear;
					}
					else
					{
						currentPeriodDepreciationAmount = (depriciationBaseAmount - accumulatedDepretiationOfFinYear) * deprericiationRate;
					}

					if (info.FinYear == additionParameters.DepreciateFromPeriodID && addition.IsOriginal)
					{
						currentPeriodDepreciationAmount = currentPeriodDepreciationAmount + CalculationParameters.BookBalance.Tax179Amount.Value
							+ CalculationParameters.BookBalance.BonusAmount.Value;
					}

					if (IncomingParameters.SuspendedPeriodsIDs.Contains(period))
					{
						currentPeriodDepreciationAmount = 0;
					}

					accumulatedDepretiationOfFinYear+= currentPeriodDepreciationAmount;
					accumulatedDepretiation += currentPeriodDepreciationAmount;

					if (depreciationAdditionSchedule.TryGetValue(period, out FADepreciationScheduleItem scheduleItem))
					{
						scheduleItem.DepreciationAmount += currentPeriodDepreciationAmount;
					}
					else
					{
						depreciationAdditionSchedule[period] = new FADepreciationScheduleItem
						{
							FinPeriodID = period,
							DepreciationAmount = currentPeriodDepreciationAmount
						};
					}
				}

				depreciationSchedule[addition.PeriodID] = depreciationAdditionSchedule;
			}

			return depreciationSchedule;
		}

		private Dictionary<string, FinYearInfo> GetFinYearInfo(List<string> periods, IYearSetup yearSetup)
		{
			Dictionary<string, FinYearInfo> map = new Dictionary<string, FinYearInfo>();

			foreach (string period in periods)
			{
				FABookPeriod currentPeriod = IncomingParameters.PeriodDepreciationUtils.BookPeriods[period];
				if (!map.ContainsKey(currentPeriod.FinYear))
				{
					map[currentPeriod.FinYear] = new FinYearInfo
					{
						FinYear = currentPeriod.FinYear,
						NumberOfPeriodsInFinYear = GetNumberOfPeriods(yearSetup),
					};
				}
			}

			return map;
		}

		protected virtual int GetNumberOfPeriods(IYearSetup yearSetup)
		{
			return yearSetup.HasAdjustmentPeriod == true ? (int)(yearSetup.FinPeriods - 1) : (int)yearSetup.FinPeriods;
		}

		protected override void ApplySuspend(SortedDictionary<string, SortedDictionary<string, FADepreciationScheduleItem>> schedules)
		{
		}

		protected List<string> GetAdditionPeriods(string fromPeriodID, string toPeriodID)
		{
			return IncomingParameters.PeriodDepreciationUtils.GetPeriods(fromPeriodID, toPeriodID).ToList();
		}

		private DutchMethod2AdditionParameters CalculateAdditionParameters(
			CalculationParameters calculationData,
			FAAddition addition)
		{
			FABookBalance bookBalance = calculationData.BookBalance;

			CheckParametersContracts();
			int assetID = calculationData.AssetID;
			int bookID = calculationData.BookID;

			DutchMethod2AdditionParameters parameters = new DutchMethod2AdditionParameters
			{
				DepreciationBasis = (addition.Amount * addition.BusinessUse) - addition.Section179Amount - addition.BonusAmount - addition.SalvageAmount
			};

			addition.CalculatedAdditionParameters = parameters;

			FABookPeriod additionPeriod = IncomingParameters.RepositoryHelper.FindFABookPeriodOfDate(addition.Date, bookID, assetID);
			FABookPeriod originalDepriciateFromPeriod = IncomingParameters.RepositoryHelper.FindFABookPeriodOfDate(bookBalance.DeprFromDate.Value, bookID, assetID);

			parameters.DepreciateFromDate = additionPeriod.StartDate.Value;
			parameters.DepreciateFromPeriodID = additionPeriod.FinPeriodID;

			parameters.DepreciateToDate = DeprCalcParameters.GetDatePlusYears(originalDepriciateFromPeriod.StartDate.Value, bookBalance.UsefulLife.Value);
			parameters.DepreciateToPeriodID = IncomingParameters.RepositoryHelper.GetFABookPeriodIDOfDate(parameters.DepreciateToDate, bookID, assetID);

			return parameters;
		}

		#region Parameters Contracts
		protected void CheckParametersContracts()
		{
			FABookBalance bookBalance = CalculationParameters.BookBalance;

			if (bookBalance == null)
			{
				throw new ArgumentNullException(nameof(CalculationParameters.BookBalance));
			}
			if (bookBalance.DeprFromDate == null)
			{
				throw new ArgumentNullException(nameof(bookBalance.DeprFromDate));
			}
			if (bookBalance.BusinessUse == null)
			{
				throw new ArgumentNullException(nameof(bookBalance.BusinessUse));
			}
			if (bookBalance.SalvageAmount == null)
			{
				throw new ArgumentNullException(nameof(bookBalance.SalvageAmount));
			}
			if (bookBalance.Tax179Amount == null)
			{
				throw new ArgumentNullException(nameof(bookBalance.Tax179Amount));
			}
			if (bookBalance.BonusAmount == null)
			{
				throw new ArgumentNullException(nameof(bookBalance.BonusAmount));
			}
		}
		#endregion

		public class FinYearInfo
		{
			public string FinYear { get; set; }
			public decimal NumberOfPeriodsInFinYear { get; set; }
		}
	}
}
