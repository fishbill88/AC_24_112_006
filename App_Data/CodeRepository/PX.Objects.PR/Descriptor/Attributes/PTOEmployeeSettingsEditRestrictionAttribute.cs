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
	/// This attribute is intended to restrict users from making changes to specifc fields within the PREmployeePTOBank DAC if it's associated in any non-voided paychecks.
	/// </summary>
	public class PTOEmployeeSettingsEditRestrictionAttribute : PXEventSubscriberAttribute, IPXFieldVerifyingSubscriber
	{
		private readonly Type _BaccountIDField;
		private readonly Type _StartDateField;

		public PTOEmployeeSettingsEditRestrictionAttribute(Type baccountIDField, Type startDateField)
		{
			this._BaccountIDField = baccountIDField;
			this._StartDateField = startDateField;
		}

		public void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			object oldValue = sender.GetValue(e.Row, _FieldName);
			
			if (e.Row == null || oldValue != null && oldValue == e.NewValue ||
				sender.GetStatus(e.Row) == PXEntryStatus.Inserted || sender.GetStatus(e.Row) == PXEntryStatus.Notchanged)
			{
				return;
			}

			var startDate = sender.GetValue(e.Row, _StartDateField.Name);
			var baccountID = sender.GetValue(e.Row, _BaccountIDField.Name);

			bool nonVoidedPaychecksUsingCurrentPTOBank = SelectFrom<PRPaymentPTOBank>
			.InnerJoin<PRPayment>.On<PRPaymentPTOBank.FK.Payment>
			.Where<PRPaymentPTOBank.bankID.IsEqual<PREmployeePTOBank.bankID.FromCurrent>
				.And<PRPaymentPTOBank.effectiveStartDate.IsGreaterEqual<P.AsDateTime.UTC>>
				.And<PRPayment.employeeID.IsEqual<P.AsInt>>
				.And<PRPayment.docType.IsNotEqual<PayrollType.voidCheck>>
				.And<PRPayment.status.IsNotEqual<PaymentStatus.voided>>>.View.Select(sender.Graph, startDate, baccountID).Any();

			if (nonVoidedPaychecksUsingCurrentPTOBank)
			{
				throw new PXSetPropertyException(Messages.PTOBankCannotBeModified);
			}
		}
	}
}
