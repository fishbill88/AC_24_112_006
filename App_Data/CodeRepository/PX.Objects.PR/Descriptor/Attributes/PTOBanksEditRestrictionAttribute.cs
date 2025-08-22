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
using System.Linq;

namespace PX.Objects.PR
{
	/// <summary>
	/// This attribute is intended to restrict users from making changes to specifc fields within (PRPTOBank / PREmployeeClassPTOBank / PRBandingRulePTOBank) DACs
	/// when they're being utilized by a user in the Employee Payroll Settings screen.
	/// </summary>
	public class PTOBanksEditRestrictionAttribute : PXEventSubscriberAttribute, IPXFieldVerifyingSubscriber
	{
		private readonly Type _EmployeeClassField = null;

		public PTOBanksEditRestrictionAttribute() { }

		public PTOBanksEditRestrictionAttribute(Type employeeClassField)
		{
			this._EmployeeClassField = employeeClassField;
		}

		public void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			object oldValue = sender.GetValue(e.Row, _FieldName);
			
			if (e.Row == null || oldValue != null && oldValue == e.NewValue ||
				sender.GetStatus(e.Row) == PXEntryStatus.Inserted || sender.GetStatus(e.Row) == PXEntryStatus.Notchanged)
			{
				return;
			}

			bool employeePayrollSettingsUsingCurrentPTOBank;

			if (_EmployeeClassField != null)
			{
				var employeeClass = sender.GetValue(e.Row, _EmployeeClassField.Name);

				if (employeeClass == null)
				{
					employeePayrollSettingsUsingCurrentPTOBank = SelectFrom<PREmployeePTOBank>
						.InnerJoin<PRPTOBank>.On<PREmployeePTOBank.FK.PTOBank>
						.InnerJoin<PREmployee>.On<PREmployeePTOBank.FK.Employee>
							.Where<PREmployeePTOBank.bankID.IsEqual<PRPTOBank.bankID.FromCurrent>
								.And<PREmployeePTOBank.employeeClassID.IsNull>
								.And<PREmployee.useCustomSettings.IsEqual<False>>>.View.Select(sender.Graph).Any();
				}
				else
				{
					//Filter on BankID and EmployeeClassID if the Employee Class is specified
					employeePayrollSettingsUsingCurrentPTOBank = SelectFrom<PREmployeePTOBank>
					.InnerJoin<PRPTOBank>.On<PREmployeePTOBank.FK.PTOBank>
					.InnerJoin<PREmployee>.On<PREmployeePTOBank.FK.Employee>
						.Where<PREmployeePTOBank.bankID.IsEqual<PRPTOBank.bankID.FromCurrent>
							.And<PREmployeePTOBank.employeeClassID.IsEqual<P.AsString>>
							.And<PREmployee.useCustomSettings.IsEqual<False>>>.View.Select(sender.Graph, employeeClass).Any();
				}
			}
			else
			{
				employeePayrollSettingsUsingCurrentPTOBank = SelectFrom<PREmployeePTOBank>
						.InnerJoin<PRPTOBank>.On<PREmployeePTOBank.FK.PTOBank>
						.InnerJoin<PREmployee>.On<PREmployeePTOBank.FK.Employee>
							.Where<PREmployeePTOBank.bankID.IsEqual<PRPTOBank.bankID.FromCurrent>
								.And<PREmployee.useCustomSettings.IsEqual<False>>>.View.Select(sender.Graph).Any();
			}

			if (employeePayrollSettingsUsingCurrentPTOBank)
			{
				if (sender.GetItemType() == typeof(PRPTOBank))
				{
					throw new PXSetPropertyException(Messages.PTOBankIsUsedByEmployees);
				}
				else if (sender.GetItemType() == typeof(PREmployeeClassPTOBank))
				{
					throw new PXSetPropertyException(Messages.CannotModifyOrDeleteEmployeeClassSettings);
				}
				else if (sender.GetItemType() == typeof(PRBandingRulePTOBank))
				{
					throw new PXSetPropertyException(Messages.CannotModifyBandingRules);
				}
				else
				{
					throw new NotImplementedException();
				}
			}
		}
	}
}
