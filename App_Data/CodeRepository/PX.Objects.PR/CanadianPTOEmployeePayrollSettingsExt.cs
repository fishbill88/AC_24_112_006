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
using PX.Objects.CS;
using PX.Objects.EP;
using System;
using System.Collections;

namespace PX.Objects.PR
{
	public class CanadianPTOEmployeePayrollSettingsExt : PXGraphExtension<PREmployeePayrollSettingsMaint>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.payrollCAN>();
		}

		#region Data views
		public PXFilter<PTOPaidHoursFilter> PTOPaidHoursPopupFilter;
		public IEnumerable ptoPaidHoursPopupFilter()
		{
			PTOHelper.PTOHistoricalAmounts history = PTOHelper.GetPTOHistory(
				Base,
				PTOPaidHoursPopupFilter.Current.EffectiveDate.Value,
				Base.PayrollEmployee.Current.BAccountID.Value,
				Base.EmployeePTOBanks.Current);

			if (PTOPaidHoursPopupFilter.Current.DisbursingType == PTODisbursingType.AverageRate)
			{
				PTOPaidHoursPopupFilter.Current.AvailablePaidHours = Math.Max(history.AvailableHours, 0m);
				if (PTOPaidHoursPopupFilter.Current.AvailablePaidHours != 0)
				{
					PTOPaidHoursPopupFilter.Current.PayRate = Math.Max(history.AvailableMoney / PTOPaidHoursPopupFilter.Current.AvailablePaidHours.Value, 0m);
				}
				else
				{
					PTOPaidHoursPopupFilter.Current.PayRate = 0m;
				}
			}
			else
			{
				if (PTOPaidHoursPopupFilter.Current.PayRate != 0)
				{
					PTOPaidHoursPopupFilter.Current.AvailablePaidHours = Math.Max(history.AvailableMoney / PTOPaidHoursPopupFilter.Current.PayRate.Value, 0m);
				}
				else
				{
					PTOPaidHoursPopupFilter.Current.AvailablePaidHours = 0m;
				}
			}

			PTOPaidHoursPopupFilter.Cache.IsDirty = false;
			yield return PTOPaidHoursPopupFilter.Current;
		}
		#endregion Data views

		#region Actions
		public PXAction<PREmployee> ViewAvailablePTOPaidHours;
		[PXButton]
		[PXUIField(DisplayName = "View Payable Hours", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		public virtual void viewAvailablePTOPaidHours()
		{
			PTOHelper.PTOHistoricalAmounts history = PTOHelper.GetPTOHistory(
				Base,
				Base.Accessinfo.BusinessDate.Value,
				Base.EmployeePTOBanks.Current.BAccountID.Value,
				Base.EmployeePTOBanks.Current);

			if (history.AvailableMoney < 0)
			{
				throw new PXException(Messages.NegativePTOAmount);
			}

			PTOPaidHoursPopupFilter.Current.BankID = Base.EmployeePTOBanks.Current.BankID;
			PTOPaidHoursPopupFilter.Current.EffectiveDate = Base.Accessinfo.BusinessDate;

			PRPTOBank ptoBank = PRPTOBank.PK.Find(Base, Base.EmployeePTOBanks.Current.BankID);
			PTOPaidHoursPopupFilter.Current.DisbursingType = ptoBank.DisbursingType;

			if (PTOPaidHoursPopupFilter.Current.DisbursingType == PTODisbursingType.CurrentRate)
			{
				PTOPaidHoursPopupFilter.Current.PayRate = GetEarningRateForBank();
			}
			PTOPaidHoursPopupFilter.AskExt();
		}
		#endregion

		#region Events
		protected virtual void _(Events.FieldUpdated<PTOPaidHoursFilter, PTOPaidHoursFilter.disbursingType> e)
		{
			if (!Equals(e.OldValue, PTODisbursingType.CurrentRate)
				&& Equals(e.NewValue, PTODisbursingType.CurrentRate))
			{
				PTOPaidHoursPopupFilter.Current.PayRate = GetEarningRateForBank();
			}
		}
		#endregion

		#region Helpers
		protected virtual decimal? GetEarningRateForBank()
		{
			EPEarningType ptoEarningType = new SelectFrom<EPEarningType>
				.InnerJoin<PRPTOBank>.On<PRPTOBank.bankID.IsEqual<PTOPaidHoursFilter.bankID.FromCurrent>>
				.Where<EPEarningType.typeCD.IsEqual<PRPTOBank.earningTypeCD>>.View(Base).SelectSingle();
			PREarningType ptoEarningTypeExt = PXCache<EPEarningType>.GetExtension<PREarningType>(ptoEarningType);
			return PayRateAttribute.GetEmployeeEarningRate(Base, ptoEarningTypeExt.RegularTypeCD, Base.PayrollEmployee.Current.BAccountID, Base.Accessinfo.BusinessDate);
		}
		#endregion

		[PXHidden]
		public class PTOPaidHoursFilter : PXBqlTable, IBqlTable
		{
			#region BankID
			[PXString(15, IsUnicode = true)]
			[PXUIField(DisplayName = "PTO Bank", Enabled = false)]
			[PXSelector(typeof(SearchFor<PRPTOBank.bankID>), DescriptionField = typeof(PRPTOBank.description))]
			public virtual string BankID { get; set; }
			public abstract class bankID : BqlString.Field<bankID> { }
			#endregion
			#region EffectiveDate
			[PXDate]
			[PXUIField(DisplayName = "Effective Date")]
			public virtual DateTime? EffectiveDate { get; set; }
			public abstract class effectiveDate : BqlDateTime.Field<effectiveDate> { }
			#endregion
			#region DisbursingType
			[PXString(1, IsFixed = true)]
			[PXUIField(DisplayName = "Disbursing Type", Enabled = false)]
			[PTODisbursingType.List]
			public virtual string DisbursingType { get; set; }
			public abstract class disbursingType : BqlString.Field<disbursingType> { }
			#endregion
			#region PayRate
			[PXDecimal(MinValue = 0)]
			[PXUIField(DisplayName = "Pay Rate", Enabled = false)]
			[PayRatePrecision]
			public virtual decimal? PayRate { get; set; }
			public abstract class payRate : BqlDecimal.Field<payRate> { }
			#endregion
			#region AvailablePaidHours
			[PXDecimal]
			[PXUIField(DisplayName = "Available Payable Hours", Enabled = false)]
			public virtual decimal? AvailablePaidHours { get; set; }
			public abstract class availablePaidHours : BqlDecimal.Field<availablePaidHours> { }
			#endregion
		}
	}
}
