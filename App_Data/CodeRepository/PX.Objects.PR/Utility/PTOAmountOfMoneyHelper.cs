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
using PX.Objects.CS;

namespace PX.Objects.PR
{
	public class PTOAmountOfMoneyHelper
	{
		#region CarryOverMoney
		internal static void UpdateCarryOverMoney(PRPaymentPTOBank effectiveBank, decimal carryOverMoney)
		{
			if (IsPayrollCanadaEnabled())
			{
				effectiveBank.CarryoverMoney = carryOverMoney;
			}
		}

		internal static void AddCarryOverMoney(PRPaymentPTOBank effectiveBank, decimal carryOverMoney)
		{
			if (IsPayrollCanadaEnabled())
			{
				effectiveBank.CarryoverMoney += carryOverMoney;
			}
		}
		#endregion

		#region AccumulatedMoney
		internal static void UpdateAccumulatedMoney(PRPaymentPTOBank effectiveBank, decimal accumulatedMoney)
		{
			if (IsPayrollCanadaEnabled())
			{
				effectiveBank.AccumulatedMoney = accumulatedMoney;
			}
		}

		internal static void AddAccumulatedMoney(PRPaymentPTOBank effectiveBank, decimal accumulatedMoney)
		{
			if (IsPayrollCanadaEnabled())
			{
				effectiveBank.AccumulatedMoney += accumulatedMoney;
			}
		}
		#endregion

		#region AvailableMoney
		internal static void UpdateAvailableMoney(PRPaymentPTOBank effectiveBank, decimal availableMoney)
		{
			if (IsPayrollCanadaEnabled())
			{
				effectiveBank.AvailableMoney = availableMoney;
			}
		}

		internal static void AddAvailableMoney(PRPaymentPTOBank effectiveBank, decimal availableMoney)
		{
			if (IsPayrollCanadaEnabled())
			{
				effectiveBank.AvailableMoney += availableMoney;
			}
		}
		#endregion

		#region AccrualMoney
		internal static void UpdateAccrualMoney(PRPaymentPTOBank effectiveBank, decimal accrualMoney)
		{
			if (IsPayrollCanadaEnabled())
			{
				effectiveBank.AccrualMoney = accrualMoney;
			}
		}

		internal static void AddAccrualMoney(PRPaymentPTOBank effectiveBank, decimal accrualMoney)
		{
			if (IsPayrollCanadaEnabled())
			{
				effectiveBank.AccrualMoney += accrualMoney;
			}
		}
		#endregion

		#region DisbursementMoney
		internal static void UpdateDisbursementMoney(PRPaymentPTOBank effectiveBank, decimal disbursementMoney)
		{
			if (IsPayrollCanadaEnabled())
			{
				effectiveBank.DisbursementMoney = disbursementMoney;
			}
		}

		internal static void AddDisbursementMoney(PRPaymentPTOBank effectiveBank, decimal disbursementMoney)
		{
			if (IsPayrollCanadaEnabled())
			{
				effectiveBank.DisbursementMoney += disbursementMoney;
			}
		}
		#endregion

		#region UsedMoney
		internal static void UpdateUsedMoney(PRPaymentPTOBank effectiveBank, decimal usedMoney)
		{
			if (IsPayrollCanadaEnabled())
			{
				effectiveBank.UsedMoney = usedMoney;
			}
		}

		internal static void AddUsedMoney(PRPaymentPTOBank effectiveBank, decimal usedMoney)
		{
			if (IsPayrollCanadaEnabled())
			{
				effectiveBank.UsedMoney += usedMoney;
			}
		}
		#endregion

		private static bool IsPayrollCanadaEnabled()
		{
			return (PXAccess.FeatureInstalled<FeaturesSet.payrollCAN>());
		}
	}
}
