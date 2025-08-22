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
using System;

namespace PX.Objects.PR.Descriptor
{
	public class HidePTOSettings : PXEventSubscriberAttribute, IPXFieldSelectingSubscriber
	{
		private Type _PTOSettingsField;

		public HidePTOSettings(Type ptoSettingField)
		{
			_PTOSettingsField = ptoSettingField;
		}

		public void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			if (e.Row != null)
			{
				PRPTOBank ptoBank = sender.Graph.Caches[typeof(PRPTOBank)].Current as PRPTOBank;
				if (ptoBank != null)
				{
					bool hideValue;
					if (sender.GetItemType() == typeof(PREmployeeClassPTOBank))
					{
						hideValue = HideEmployeeClassSettingsFieldValue(ptoBank);
					}
					else if (sender.GetItemType() == typeof(PRBandingRulePTOBank))
					{
						hideValue = HideBandingRulesFieldValue(ptoBank);
					}
					else
					{
						throw new NotImplementedException();
					}

					if (hideValue)
					{
						e.ReturnValue = null;
					}
				}
			}
		}

		public bool HideBandingRulesFieldValue(PRPTOBank ptoBank)
		{
			switch (_PTOSettingsField.Name)
			{
				case nameof(PRBandingRulePTOBank.accrualRate):
					if (ptoBank.AccrualMethod != PTOAccrualMethod.Percentage &&
						ptoBank.AccrualMethod != PTOAccrualMethod.FrontLoadingAndPercentage)
					{
						return true;
					}

					break;
				case nameof(PRBandingRulePTOBank.hoursPerYear):
					if (ptoBank.AccrualMethod != PTOAccrualMethod.TotalHoursPerYear &&
						ptoBank.AccrualMethod != PTOAccrualMethod.FrontLoadingAndHoursPerYear)
					{
						return true;
					}

					break;
				case nameof(PRBandingRulePTOBank.carryoverAmount):
					if (ptoBank.CarryoverType != CarryoverType.Partial)
					{
						return true;
					}

					break;
				case nameof(PRBandingRulePTOBank.frontLoadingAmount):
					if (ptoBank.AccrualMethod != PTOAccrualMethod.FrontLoading &&
						ptoBank.AccrualMethod != PTOAccrualMethod.FrontLoadingAndHoursPerYear &&
						ptoBank.AccrualMethod != PTOAccrualMethod.FrontLoadingAndPercentage)
					{
						return true;
					}

					break;
			}

			return false;
		}

		public bool HideEmployeeClassSettingsFieldValue(PRPTOBank ptoBank)
		{
			switch (_PTOSettingsField.Name)
			{
				case nameof(PREmployeeClassPTOBank.accrualRate):
					if (ptoBank.AccrualMethod != PTOAccrualMethod.Percentage &&
						ptoBank.AccrualMethod != PTOAccrualMethod.FrontLoadingAndPercentage)
					{
						return true;
					}

					break;
				case nameof(PREmployeeClassPTOBank.hoursPerYear):
					if (ptoBank.AccrualMethod != PTOAccrualMethod.TotalHoursPerYear &&
						ptoBank.AccrualMethod != PTOAccrualMethod.FrontLoadingAndHoursPerYear)
					{
						return true;
					}

					break;
				case nameof(PREmployeeClassPTOBank.carryoverAmount):
					if (ptoBank.CarryoverType != CarryoverType.Partial)
					{
						return true;
					}

					break;
				case nameof(PREmployeeClassPTOBank.frontLoadingAmount):
					if (ptoBank.AccrualMethod != PTOAccrualMethod.FrontLoading &&
						ptoBank.AccrualMethod != PTOAccrualMethod.FrontLoadingAndHoursPerYear &&
						ptoBank.AccrualMethod != PTOAccrualMethod.FrontLoadingAndPercentage)
					{
						return true;
					}

					break;
			}

			return false;
		}
	}
}
