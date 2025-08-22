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

using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Payroll.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.PR
{
	using PTOAccrualSplits = PRCalculationEngine.PTOAccrualSplits;

	public class CanadianPTOCalculationEngineExt : PXGraphExtension<PRCalculationEngine>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.payrollCAN>();
		}

		#region Data views
		public SelectFrom<PRPTODetail>.
			Where<PRPTODetail.FK.Payment.SameAsCurrent>
			.OrderBy<PRPTODetail.bankID.Asc>.View PTODetails;
		#endregion Data views

		#region Base graph overrides
		public delegate void DeleteCalculatedEarningLinesDelegate(ref List<PRPayment> paymentList);
		[PXOverride]
		public virtual void DeleteCalculatedEarningLines(ref List<PRPayment> paymentList, DeleteCalculatedEarningLinesDelegate baseMethod)
		{
			baseMethod(ref paymentList);

			List<PRPayment> updatedPayments = new List<PRPayment>();
			foreach (PRPayment payment in paymentList)
			{
				Base.Payments.Current = payment;
				RegularAmountAttribute.EnforceEarningDetailUpdate<PRPayment.regularAmount>(Base.Payments.Cache, Base.Payments.Current, false);
				CanadianPTOPaychecksAndAdjustmentsExt.RevertPaymentSplitPTOEarnings(Base, payment, Base.PaymentEarningDetails.View);
				RegularAmountAttribute.EnforceEarningDetailUpdate<PRPayment.regularAmount>(Base.Payments.Cache, Base.Payments.Current, true);
				updatedPayments.Add(Base.Payments.Current);
			}

			Base.Actions.PressSave();
			paymentList = updatedPayments;
		}

		public delegate void PerformPTOCalculationsDelegate();
		[PXOverride]
		public virtual void PerformPTOCalculations(PerformPTOCalculationsDelegate baseMethod)
		{
			int accrualMoneyCalculationPrecision = 6;
			int accrualMoneyDisplayPrecision = 2;

			PXDBDecimalAttribute.SetPrecision(Base.PaymentPTOBanks.Cache, nameof(PRPaymentPTOBank.AccrualMoney), accrualMoneyCalculationPrecision);

			baseMethod();

			PXDBDecimalAttribute.SetPrecision(Base.PaymentPTOBanks.Cache, nameof(PRPaymentPTOBank.AccrualMoney), accrualMoneyDisplayPrecision);
		}

		public delegate void CalculatePTODelegate(PREarningDetail earningDetail);
		[PXOverride]
		public virtual void CalculatePTO(PREarningDetail earningDetail, CalculatePTODelegate baseMethod)
		{
			baseMethod(earningDetail);
		}

		public delegate void CreatePTODetailsDelegate();
		[PXOverride]
		public virtual void CreatePTODetails(CreatePTODetailsDelegate baseMethod)
		{
			PTODetails.Select().ForEach(x => PTODetails.Delete(x));

			PRPayment currentPayment = Base.Payments.Current;
			foreach (string ptoBankID in Base.PaymentPTOBanks.Select().FirstTableItems
				.Where(x => x.CreateFinancialTransaction == true && x.IsActive == true)
				.GroupBy(x => x.BankID)
				.Select(x => x.Key))
			{
				Base.PaymentsToProcess[currentPayment].PTOAccrualMoneySplitByEarning.TryGetValue(ptoBankID, out PTOAccrualSplits accrualSplits);
				CreatePTODetail(Base, currentPayment, PTODetails.Cache, ptoBankID, Base.PaymentEarningDetails.Select().FirstTableItems, accrualSplits);
			}
		}
		#endregion Base graph overrides

		#region Helpers
		public static void CreatePTODetail(
			PXGraph graph,
			PRPayment currentPayment,
			PXCache ptoDetailViewCache,
			string bankID,
			IEnumerable<PREarningDetail> earnings,
			PTOAccrualSplits applicableAmountsPerEarning = null)
		{
			PXGraph.CreateInstance<PRCalculationEngine.PRCalculationEngineUtils>()
				.GetExtension<CanadianPTOCalculationEngineUtilsExt>()
				.CreatePTODetail(graph, currentPayment, ptoDetailViewCache, bankID, earnings, applicableAmountsPerEarning);
		}

		public virtual void AdjustPTODisbursementRate(PREmployeePTOBank bank, PREarningDetail row, decimal availableHours, decimal availableMoney)
		{
			if (Base.Payments.Current.CountryID != LocationConstants.CanadaCountryCode || bank.CreateFinancialTransaction != true)
			{
				return;
			}

			PRPTOBank ptoBank = PRPTOBank.PK.Find(Base, bank.BankID);

			if (ptoBank.DisbursingType == PTODisbursingType.AverageRate)
			{
				row.Rate = availableHours <= 0 || availableMoney <= 0 ? 0 : availableMoney / availableHours;
				Base.PaymentEarningDetails.Update(row);
			}
		}

		public virtual void AdjustPTODisbursementAcctAndSub(PREmployeePTOBank bank, PREarningDetail disbursementEarning, decimal effectiveAvailableMoney, decimal disbursementAmount)
		{
			if (Base.Payments.Current.CountryID != LocationConstants.CanadaCountryCode || bank.CreateFinancialTransaction != true)
			{
				return;
			}

			PRCalculationEngine.PTODisbursementEarningDetail disbursementEarningSetup = new PRCalculationEngine.PTODisbursementEarningDetail() { PTOBankID = bank.BankID, EmployeeID = disbursementEarning.EmployeeID };
			Base.Caches[typeof(PRCalculationEngine.PTODisbursementEarningDetail)].SetDefaultExt<PRCalculationEngine.PTODisbursementEarningDetail.liabilityAccountID>(disbursementEarningSetup);
			Base.Caches[typeof(PRCalculationEngine.PTODisbursementEarningDetail)].SetDefaultExt<PRCalculationEngine.PTODisbursementEarningDetail.liabilitySubID>(disbursementEarningSetup);
			Base.Caches[typeof(PRCalculationEngine.PTODisbursementEarningDetail)].SetDefaultExt<PRCalculationEngine.PTODisbursementEarningDetail.assetAccountID>(disbursementEarningSetup);
			Base.Caches[typeof(PRCalculationEngine.PTODisbursementEarningDetail)].SetDefaultExt<PRCalculationEngine.PTODisbursementEarningDetail.assetSubID>(disbursementEarningSetup);

			RegularAmountAttribute.EnforceEarningDetailUpdate<PRPayment.regularAmount>(Base.Payments.Cache, Base.Payments.Current, false);
			if (disbursementEarning.Amount != 0)
			{
				if (effectiveAvailableMoney <= 0)
				{
					if (effectiveAvailableMoney + disbursementEarning.Amount > 0)
					{
						PREarningDetail copy = (PREarningDetail)Base.PaymentEarningDetails.Cache.CreateCopy(disbursementEarning);
						copy.RecordID = null;
						copy.Amount = null;
						copy.BasePTORecordID = disbursementEarning.RecordID;
						copy = Base.PaymentEarningDetails.Insert(copy);

						decimal? originalHours = disbursementEarning.Hours;
						decimal nonExceededAmount = effectiveAvailableMoney + disbursementEarning.Amount.GetValueOrDefault();

						Base.PaymentEarningDetails.Cache.SetValueExt<PREarningDetail.hours>(disbursementEarning, nonExceededAmount / disbursementEarning.Rate);
						copy.Hours = originalHours - disbursementEarning.Hours;
						copy.AccountID = disbursementEarningSetup.AssetAccountID;
						copy.SubID = disbursementEarningSetup.AssetSubID;
						Base.PaymentEarningDetails.Update(copy);

						disbursementEarning.AccountID = disbursementEarningSetup.LiabilityAccountID;
						disbursementEarning.SubID = disbursementEarningSetup.LiabilitySubID;

					}
					else
					{
						disbursementEarning.AccountID = disbursementEarningSetup.AssetAccountID;
						disbursementEarning.SubID = disbursementEarningSetup.AssetSubID;
					}
				}
				else
				{
					disbursementEarning.AccountID = disbursementEarningSetup.LiabilityAccountID;
					disbursementEarning.SubID = disbursementEarningSetup.LiabilitySubID;

				}
			}

			Base.PaymentEarningDetails.Update(disbursementEarning);
			RegularAmountAttribute.EnforceEarningDetailUpdate<PRPayment.regularAmount>(Base.Payments.Cache, Base.Payments.Current, true);
		}
		#endregion Helpers

		public class CanadianPTOCalculationEngineUtilsExt : PXGraphExtension<PRCalculationEngine.PRCalculationEngineUtils>
		{
			public static bool IsActive()
			{
				return PXAccess.FeatureInstalled<FeaturesSet.payrollCAN>();
			}

			public virtual void CreatePTODetail(
				PXGraph graph,
				PRPayment currentPayment,
				PXCache ptoDetailViewCache,
				string bankID,
				IEnumerable<PREarningDetail> earnings,
				PTOAccrualSplits applicableAmountsPerEarning)
			{
				var setupView = new SelectFrom<PRSetup>.View(graph);
				PRSetup preferences = setupView.SelectSingle();
				DetailSplitType splitType = Base.GetExpenseSplitSettings(
					setupView.Cache,
					preferences,
					typeof(PRSetup.ptoExpenseAcctDefault),
					typeof(PRSetup.ptoExpenseSubMask),
					PRPTOExpenseAcctSubDefault.MaskEarningType,
					PRPTOExpenseAcctSubDefault.MaskLaborItem);

				PREmployeePTOBank sourceBank = PTOHelper.GetBankSettings(graph, bankID, currentPayment.EmployeeID.Value, currentPayment.TransactionDate.Value);
				if (sourceBank?.CreateFinancialTransaction != true)
				{
					return;
				}

				PRCalculationEngine.PRCalculationEngineUtils.UnmatchedSplit unmatched = null;
				List<PREarningDetail> accruingEarningDetails = new List<PREarningDetail>();
				bool calculateApplicableAmounts = applicableAmountsPerEarning == null;
				applicableAmountsPerEarning = applicableAmountsPerEarning ?? new PTOAccrualSplits();
				foreach (PRPaymentPTOBank paymentPTOBank in SelectFrom<PRPaymentPTOBank>
					.Where<PRPaymentPTOBank.docType.IsEqual<P.AsString>
						.And<PRPaymentPTOBank.refNbr.IsEqual<P.AsString>>
						.And<PRPaymentPTOBank.bankID.IsEqual<P.AsString>>
						.And<PRPaymentPTOBank.isActive.IsEqual<True>>
						.And<PRPaymentPTOBank.accrualMoney.IsNotEqual<decimal0>>>.View.Select(graph, currentPayment.DocType, currentPayment.RefNbr, bankID))
				{
					List<PREarningDetail> currentBankAccruingEarningDetails = GetAccruingEarningDetails(graph, paymentPTOBank, sourceBank, earnings);
					accruingEarningDetails.AddRange(currentBankAccruingEarningDetails);
					if (calculateApplicableAmounts)
					{
						PTOAccrualSplits currentBankApplicableAmountsPerEarning = SplitPTOAccrualAmountsPerEarning(
						graph,
						paymentPTOBank,
						sourceBank,
						accruingEarningDetails,
						out PRCalculationEngine.PRCalculationEngineUtils.UnmatchedSplit currentBankUnmatched);

						foreach (KeyValuePair<int?, decimal?> kvp in currentBankApplicableAmountsPerEarning)
						{
							if (!applicableAmountsPerEarning.ContainsKey(kvp.Key))
							{
								applicableAmountsPerEarning[kvp.Key] = 0m;
							}
							applicableAmountsPerEarning[kvp.Key] += kvp.Value;
						}

						if (currentBankUnmatched != null)
						{
							if (unmatched == null)
							{
								unmatched = new PRCalculationEngine.PRCalculationEngineUtils.UnmatchedSplit();
							}
							unmatched.Amount += currentBankUnmatched.Amount;
						}
					}
				}

				int? paymentBranch = currentPayment.BranchID;
				if (preferences.ProjectCostAssignment != ProjectCostAssignmentType.WageLaborBurdenAssigned
					&& !splitType.SplitByProjectTask
					&& !splitType.SplitByEarningType
					&& !splitType.SplitByLaborItem)
				{
					Base.CreateDetailSplitByBranch<PRPTODetail, string>(ptoDetailViewCache, bankID, applicableAmountsPerEarning, accruingEarningDetails, unmatched, paymentBranch);
				}
				else
				{
					if (preferences.ProjectCostAssignment == ProjectCostAssignmentType.WageLaborBurdenAssigned || splitType.SplitByProjectTask)
					{
						if (splitType.SplitByEarningType)
						{
							Base.CreateDetailSplitByProjectTaskAndEarningType<PRPTODetail, string>(ptoDetailViewCache, bankID, applicableAmountsPerEarning, accruingEarningDetails, unmatched);
						}
						else if (splitType.SplitByLaborItem)
						{
							Base.CreateDetailSplitByProjectTaskAndLaborItem<PRPTODetail, string, PRSetup.ptoExpenseAlternateAcctDefault, PRSetup.ptoExpenseAlternateSubMask>(ptoDetailViewCache, bankID, applicableAmountsPerEarning, accruingEarningDetails, unmatched, paymentBranch);

						}
						else
						{
							Base.CreateDetailSplitByProjectTask<PRPTODetail, string, PRSetup.ptoExpenseAlternateAcctDefault, PRSetup.ptoExpenseAlternateSubMask>(ptoDetailViewCache, bankID, applicableAmountsPerEarning, accruingEarningDetails, unmatched, paymentBranch);
						}

					}
					else
					{
						if (splitType.SplitByEarningType)
						{
							Base.CreateDetailSplitByEarningType<PRPTODetail, string>(ptoDetailViewCache, bankID, applicableAmountsPerEarning, accruingEarningDetails, unmatched);
						}
						else if (splitType.SplitByLaborItem)
						{
							Base.CreateDetailSplitByLaborItem<PRPTODetail, string, PRSetup.ptoExpenseAlternateAcctDefault, PRSetup.ptoExpenseAlternateSubMask>(ptoDetailViewCache, bankID, applicableAmountsPerEarning, accruingEarningDetails, unmatched, paymentBranch);
						}
					}
				}
			}

			protected virtual PTOAccrualSplits SplitPTOAccrualAmountsPerEarning(
				PXGraph graph,
				PRPaymentPTOBank paymentPTOBank,
				PREmployeePTOBank sourceBank,
				List<PREarningDetail> accruingEarningDetails,
				out PRCalculationEngine.PRCalculationEngineUtils.UnmatchedSplit unmatched)
			{
				unmatched = null;
				PTOAccrualSplits splits = new PTOAccrualSplits();
				PTOAccrualSplits nominalSplits = GetPTOAccrualSplits(graph, paymentPTOBank, sourceBank, accruingEarningDetails);
				decimal totalNominalAccrualMoney = nominalSplits.Sum(x => x.Value.GetValueOrDefault());

				if (totalNominalAccrualMoney != 0 && paymentPTOBank.AccrualMoney > totalNominalAccrualMoney)
				{
					foreach (PREarningDetail earning in accruingEarningDetails)
					{
						splits[earning.RecordID] = PRUtils.Round(nominalSplits[earning.RecordID] * paymentPTOBank.AccrualMoney / totalNominalAccrualMoney);
					}
				}
				else if (paymentPTOBank.AccrualMoney != 0)
				{
					decimal totalAccuralRemaining = paymentPTOBank.AccrualMoney.Value;
					foreach (PREarningDetail earning in accruingEarningDetails)
					{
						decimal currentEarningAccrual = Math.Min(totalAccuralRemaining, nominalSplits[earning.RecordID].GetValueOrDefault());
						if (currentEarningAccrual != 0)
						{
							splits[earning.RecordID] = currentEarningAccrual;
							totalAccuralRemaining -= currentEarningAccrual; 
						}

						if (totalAccuralRemaining <= 0)
						{
							break;
						}
					}
				}

				if (splits.Any())
				{
					Base.HandleRounding(splits, paymentPTOBank.AccrualMoney);
				}
				else if (paymentPTOBank.AccrualMoney != 0)
				{
					unmatched = new PRCalculationEngine.PRCalculationEngineUtils.UnmatchedSplit()
					{
						Amount = paymentPTOBank.AccrualMoney.GetValueOrDefault()
					};
				}

				return splits;
			}

			public virtual List<PREarningDetail> GetAccruingEarningDetails(PXGraph graph, PRPaymentPTOBank paymentPTOBank, PREmployeePTOBank sourceBank, IEnumerable<PREarningDetail> allEarningDetails)
			{
				HashSet<string> accrueTimeOff = new HashSet<string>();
				foreach (EPEarningType earningType in SelectFrom<EPEarningType>.View.Select(graph).FirstTableItems)
				{
					PREarningType prEarningType = earningType?.GetExtension<PREarningType>();
					if (prEarningType == null || prEarningType.AccruePTO == true)
						accrueTimeOff.Add(earningType.TypeCD);
				}

				IEnumerable<PREarningDetail> accruingEarningDetails = allEarningDetails.Where(x => accrueTimeOff.Contains(x.TypeCD)
					&& x.IsFringeRateEarning == false
					&& x.Date != null
					&& x.Date.Value.Date >= paymentPTOBank.EffectiveStartDate.Value.Date
					&& x.Date.Value.Date <= paymentPTOBank.EffectiveEndDate.Value.Date);
				if (paymentPTOBank.IsCertifiedJob == true)
				{
					accruingEarningDetails = accruingEarningDetails.Where(x => x.CertifiedJob == true);
				}

				return accruingEarningDetails.ToList();
			}

			public virtual PTOAccrualSplits GetPTOAccrualSplits(PXGraph graph, PRPaymentPTOBank paymentBank, PREmployeePTOBank sourceBank, List<PREarningDetail> accruingEarningDetails)
			{
				PTOAccrualSplits splits = new PTOAccrualSplits();
				if (paymentBank.CreateFinancialTransaction != true)
				{
					return splits;
				}

				GetAccrualPctAndMultiplier(graph, paymentBank, out decimal accrualPct, out decimal accrualMultiplier);
				PXCache earningDetailCache = graph.Caches[typeof(PREarningDetail)];
				PRPTOBank ptoBank = PRPTOBank.PK.Find(graph, sourceBank.BankID);

				foreach (PREarningDetail earning in accruingEarningDetails)
				{
					if (ptoBank.DisbursingType != PTODisbursingType.AverageRate)
					{
						splits[earning.RecordID] = earning.Amount.GetValueOrDefault() * accrualPct * accrualMultiplier;
					}
					else
					{
						// If the earning is a PTO disbursement with Average Rate disbursing type,
						// the accrual is calculated with the default rate, not the actual rate, to avoid
						// circular logic.
						PREarningDetail copy = earningDetailCache.CreateCopy(earning) as PREarningDetail;
						copy.ManualRate = false;
						PayRateAttribute.SetRate(earningDetailCache, copy);
						splits[earning.RecordID] = copy.Amount.GetValueOrDefault() * accrualPct * accrualMultiplier;
					}
				}

				return splits;
			}

			public virtual void GetAccrualPctAndMultiplier(PXGraph graph, PRPaymentPTOBank bank, out decimal accrualPct, out decimal accrualMultiplier)
			{
				accrualPct = bank.AccrualRate.GetValueOrDefault() / 100;
				accrualMultiplier = (graph.Caches<PRPayment>().Current as PRPayment)?.DocType == PayrollType.VoidCheck ? -1 : 1;
			}
		}
	}
}
