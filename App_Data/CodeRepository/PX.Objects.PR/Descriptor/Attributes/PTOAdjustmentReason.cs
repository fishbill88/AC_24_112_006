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

namespace PX.Objects.PR
{
	public class PTOAdjustmentReason
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[]
				{
					InitialBalanceCorrection, BonusOrIncentiveAllocation, AccrualModification,
					TimecardUsageCorrection, AccrualEarnedCorrection, DataEntryCorrection,
					CarryoverCorrection, MaximumAccrualCorrection, Other
				},
				new string[]
				{
					Messages.InitialBalanceCorrection, Messages.BonusOrIncentiveAllocation, Messages.AccrualModification,
					Messages.TimecardUsageCorrection, Messages.AccrualEarnedCorrection, Messages.DataEntryCorrection,
					Messages.CarryoverCorrection, Messages.MaximumAccrualCorrection, Messages.PTOAdjustmentOtherReason
				})
			{ }
		}

		public class initialBalanceCorrection : PX.Data.BQL.BqlString.Constant<initialBalanceCorrection>
		{
			public initialBalanceCorrection() : base(InitialBalanceCorrection)
			{
			}
		}

		public class bonusOrIncentiveAllocation : PX.Data.BQL.BqlString.Constant<bonusOrIncentiveAllocation>
		{
			public bonusOrIncentiveAllocation() : base(BonusOrIncentiveAllocation)
			{
			}
		}

		public class accrualModification : PX.Data.BQL.BqlString.Constant<accrualModification>
		{
			public accrualModification() : base(AccrualModification)
			{
			}
		}

		public class timecardUsageCorrection : PX.Data.BQL.BqlString.Constant<timecardUsageCorrection>
		{
			public timecardUsageCorrection() : base(TimecardUsageCorrection)
			{
			}
		}

		public class accrualEarnedCorrection : PX.Data.BQL.BqlString.Constant<accrualEarnedCorrection>
		{
			public accrualEarnedCorrection() : base(AccrualEarnedCorrection)
			{
			}
		}

		public class dataEntryCorrection : PX.Data.BQL.BqlString.Constant<dataEntryCorrection>
		{
			public dataEntryCorrection() : base(DataEntryCorrection)
			{
			}
		}

		public class carryoverCorrection : PX.Data.BQL.BqlString.Constant<carryoverCorrection>
		{
			public carryoverCorrection() : base(CarryoverCorrection)
			{
			}
		}

		public class maximumAccrualCorrection : PX.Data.BQL.BqlString.Constant<maximumAccrualCorrection>
		{
			public maximumAccrualCorrection() : base(MaximumAccrualCorrection)
			{
			}
		}

		public class other : PX.Data.BQL.BqlString.Constant<other>
		{
			public other() : base(Other)
			{
			}
		}

		public const string InitialBalanceCorrection = "IBC";
		public const string BonusOrIncentiveAllocation = "BIA";
		public const string AccrualModification = "AMD";
		public const string TimecardUsageCorrection = "TUC";
		public const string AccrualEarnedCorrection = "AEC";
		public const string DataEntryCorrection = "DEC";
		public const string CarryoverCorrection = "COC";
		public const string MaximumAccrualCorrection = "MAC";
		public const string Other = "OTH";
	}
}
