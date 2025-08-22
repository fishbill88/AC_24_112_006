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
using PX.Data.WorkflowAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.PR
{
	public class PRPTOAdjustmentMaint : PXGraph<PRPTOAdjustmentMaint, PRPTOAdjustment>
	{
		#region Views

		public SelectFrom<PRPTOAdjustment>.View Document;
		[PXImport(typeof(PRPTOAdjustment))]
		public SelectFrom<PRPTOAdjustmentDetail>
			.Where<PRPTOAdjustmentDetail.type.IsEqual<PRPTOAdjustmentDetail.type.AsOptional>
				.And<PRPTOAdjustmentDetail.refNbr.IsEqual<PRPTOAdjustmentDetail.refNbr.AsOptional>>>.View PTOAdjustmentDetails;

		public PXSetup<PRSetup> Preferences;

		public SelectFrom<PREmployeePTOBank>
			.Where<PREmployeePTOBank.bAccountID.IsEqual<P.AsInt>
				.And<PREmployeePTOBank.bankID.IsEqual<P.AsString>>>.View EmployeePTOBanks;

		#endregion Views

		#region Entity Event Handlers

		public PXWorkflowEventHandler<PRPTOAdjustment> OnVoidingAdjustmentReleased;

		#endregion

		#region Actions

		public PXAction<PRPTOAdjustment> Release;
		[PXUIField(DisplayName = "Release", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXButton]
		public virtual IEnumerable release(PXAdapter adapter)
		{
			Actions.PressSave();

			if (OpenPaychecksExist())
			{
				throw new PXException(Messages.PTOAdjustmentCannotBeReleased);
			}

			PRPTOAdjustment adjustment = Document.Current;

			if (adjustment.Type == PTOAdjustmentType.VoidingAdjustment)
			{
				PRPTOAdjustment.Events
					.Select(e => e.VoidingAdjustmentReleased)
					.FireOn(this, adjustment);
			}

			return adapter.Get();
		}

		public PXAction<PRPTOAdjustment> VoidPTOAdjustment;
		[PXUIField(DisplayName = "Void", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXButton]
		public virtual IEnumerable voidPTOAdjustment(PXAdapter adapter)
		{
			PRPTOAdjustment existingVoidingAdjustment = PRPTOAdjustment.PK.Find(this, PTOAdjustmentType.VoidingAdjustment, Document.Current?.RefNbr);

			if (existingVoidingAdjustment != null)
			{
				return new List<PRPTOAdjustment> { existingVoidingAdjustment };
			}

			if (LinkedPaychecksExist())
			{
				throw new PXException(Messages.PTOAdjustmentAppliedToPaychecksAndCannotBeVoided);
			}

			if (OpenPaychecksExist())
			{
				throw new PXException(Messages.PTOAdjustmentCannotBeVoided);
			}

			PRPTOAdjustment voidingAdjustment = PXCache<PRPTOAdjustment>.CreateCopy(Document.Current);

			voidingAdjustment.Type = PTOAdjustmentType.VoidingAdjustment;
			voidingAdjustment.Status = PTOAdjustmentStatus.New;
			voidingAdjustment.Date = DateTime.Now.Date;
			voidingAdjustment.Description = string.Format(Messages.VoidPTOAdjustmentDescription, Document.Current.RefNbr);

			Document.Cache.SetStatus(voidingAdjustment, PXEntryStatus.Inserted);

			foreach (PRPTOAdjustmentDetail originalAdjustmentDetail in PTOAdjustmentDetails.Select(Document.Current.Type, Document.Current.RefNbr))
			{
				PRPTOAdjustmentDetail voidingAdjustmentDetail = PXCache<PRPTOAdjustmentDetail>.CreateCopy(originalAdjustmentDetail);

				voidingAdjustmentDetail.Type = PTOAdjustmentType.VoidingAdjustment;
				PopulatePTOAdjustmentDetail(voidingAdjustmentDetail, voidingAdjustmentDetail.BAccountID.Value, voidingAdjustmentDetail.BankID, voidingAdjustment.Date.Value);
				PTOAdjustmentDetails.SetValueExt<PRPTOAdjustmentDetail.adjustmentHours>(voidingAdjustmentDetail, -originalAdjustmentDetail.AdjustmentHours);

				PTOAdjustmentDetails.Insert(voidingAdjustmentDetail);
			}

			Actions.PressSave();

			return new List<PRPTOAdjustment> { PRPTOAdjustment.PK.Find(this, voidingAdjustment.Type, voidingAdjustment.RefNbr) };
		}

		#endregion Actions

		#region Event Handlers

		protected void _(Events.RowSelected<PRPTOAdjustment> e)
		{
			if (e.Row == null)
			{
				return;
			}

			PXEntryStatus entryStatus = e.Cache.GetStatus(e.Row);
			Release.SetEnabled(entryStatus != PXEntryStatus.Inserted);

			bool nonReleasedAdjustment = e.Row.Status == PTOAdjustmentStatus.New;
			bool nonVoidingAdjustment = e.Row.Type == PTOAdjustmentType.Adjustment;

			Document.AllowInsert = nonVoidingAdjustment;
			Document.AllowUpdate = nonReleasedAdjustment;
			Document.AllowDelete = nonReleasedAdjustment;
			
			Delete.SetEnabled(nonReleasedAdjustment);

			PTOAdjustmentDetails.AllowInsert = nonReleasedAdjustment && nonVoidingAdjustment;
			PTOAdjustmentDetails.AllowUpdate = nonReleasedAdjustment && nonVoidingAdjustment;
			PTOAdjustmentDetails.AllowDelete = nonReleasedAdjustment && nonVoidingAdjustment;
		}

		protected void _(Events.FieldDefaulting<PRPTOAdjustment, PRPTOAdjustment.date> e)
		{
			e.NewValue = DateTime.Now.Date;
		}

		protected void _(Events.FieldVerifying<PRPTOAdjustmentDetail, PRPTOAdjustmentDetail.bankID> e)
		{
			string bankID = e.NewValue as string;

			if (e.Row == null || e.Row.BAccountID == null || string.IsNullOrWhiteSpace(bankID) || Document.Current?.Date == null)
			{
				return;
			}

			PREmployeePTOBank employeePTOBank = GetEmployeePTOBank(e.Row.BAccountID.Value, bankID, Document.Current.Date.Value);

			if (employeePTOBank == null)
			{
				e.NewValue = e.OldValue;
				e.Cancel = true;
				throw new PXSetPropertyException(ErrorMessages.ValueDoesntExist, nameof(PREmployeePTOBank.bankID), bankID);
			}
		}

		protected void _(Events.FieldVerifying<PRPTOAdjustmentDetail, PRPTOAdjustmentDetail.adjustmentHours> e)
		{
			decimal? adjustmentHours = e.NewValue as decimal?;

			if (e.Row == null || adjustmentHours == null || e.Row.BAccountID == null || string.IsNullOrWhiteSpace(e.Row.BankID) || Document.Current?.Date == null)
			{
				return;
			}

			PREmployeePTOBank employeePTOBank = GetEmployeePTOBank(e.Row.BAccountID.Value, e.Row.BankID, Document.Current.Date.Value);
			decimal availableHours = PTOHelper.GetPTOHistory(this, Document.Current.Date.Value, e.Row.BAccountID.Value, employeePTOBank).AvailableHours;

			if (availableHours + adjustmentHours > employeePTOBank.AccrualLimit)
			{
				e.NewValue = employeePTOBank.AccrualLimit - availableHours;
				PXUIFieldAttribute.SetWarning<PRPTOAdjustmentDetail.adjustmentHours>(e.Cache, e.Row, Messages.PTOBankAccrualLimitCannotBeExceeded);
			}
			else if (availableHours + adjustmentHours < 0 && employeePTOBank.AllowNegativeBalance == false)
			{
				e.NewValue = -availableHours;
				PXUIFieldAttribute.SetWarning<PRPTOAdjustmentDetail.adjustmentHours>(e.Cache, e.Row, Messages.PTOBankCannotHaveNegativeBalance);
			}
		}

		protected void _(Events.FieldUpdated<PRPTOAdjustmentDetail, PRPTOAdjustmentDetail.bAccountID> e)
		{
			if (e.Row == null || e.Row.BankID == null || e.OldValue == e.NewValue)
			{
				return;
			}

			e.Row.BankID = null;
		}

		protected void _(Events.FieldUpdated<PRPTOAdjustmentDetail, PRPTOAdjustmentDetail.bankID> e)
		{
			if (e.Row == null)
			{
				return;
			}

			string bankID = e.NewValue as string;

			if (e.OldValue as string != bankID)
			{
				e.Row.AdjustmentHours = null;
			}
			
			if (string.IsNullOrWhiteSpace(bankID) || e.Row.BAccountID == null || Document.Current?.Date == null)
			{
				return;
			}

			PopulatePTOAdjustmentDetail(e.Row, e.Row.BAccountID.Value, bankID, Document.Current.Date.Value);
		}

		protected void _(Events.FieldUpdated<PRPTOAdjustmentDetail, PRPTOAdjustmentDetail.adjustmentReason> e)
		{
			if (e.Row != null && e.OldValue != e.NewValue)
			{
				e.Row.ReasonDetails = null;
			}
		}

		protected void _(Events.RowPersisting<PRPTOAdjustmentDetail> e)
		{
			if ((e.Operation & PXDBOperation.Command) != PXDBOperation.Delete && e.Row.BAccountID != null && e.Row.BankID != null && Document.Current?.Date != null)
			{
				PREmployeePTOBank employeePTOBank = GetEmployeePTOBank(e.Row.BAccountID.Value, e.Row.BankID, Document.Current.Date.Value);

				if (employeePTOBank == null)
				{
					e.Cache.RaiseExceptionHandling<PRPTOAdjustmentDetail.bankID>(
						e.Row,
						e.Row.BankID,
						new PXSetPropertyException(
							ErrorMessages.ValueDoesntExist,
							PXUIFieldAttribute.GetDisplayName<PRPTOAdjustmentDetail.bankID>(e.Cache),
							e.Row.BankID));
				}

				if (e.Row.EffectiveStartDate == null)
				{
					e.Cache.RaiseExceptionHandling<PRPTOAdjustmentDetail.effectiveStartDate>(
						e.Row,
						e.Row.EffectiveStartDate,
						new PXSetPropertyException(
							Messages.CantBeEmpty,
							PXUIFieldAttribute.GetDisplayName<PRPTOAdjustmentDetail.effectiveStartDate>(e.Cache)));
				}
			}
		}

		#endregion Event Handlers

		#region Helper Methods

		protected virtual PREmployeePTOBank GetEmployeePTOBank(int bAccountID, string bankID, DateTime effectiveDate)
		{
			return PTOHelper.GetBankSettings(this, bankID, bAccountID, effectiveDate);
		}

		public virtual void PopulatePTOAdjustmentDetail(PRPTOAdjustmentDetail ptoAdjustmentDetail, int bAccountID, string bankID, DateTime effectiveDate)
		{
			PREmployeePTOBank employeePTOBank = GetEmployeePTOBank(bAccountID, bankID, effectiveDate);

			if (employeePTOBank != null)
			{
				PTOHelper.GetPTOBankYear(effectiveDate, employeePTOBank.PTOYearStartDate.Value, out DateTime ptoYearStart, out DateTime ptoYearEnd);
				int day = PTOHelper.GetPTOEffectiveDay(employeePTOBank.StartDate, employeePTOBank.PTOYearStartDate, ptoYearStart, ptoYearEnd);
				DateTime effectiveStartDate = PTOHelper.GetPTOEffectiveStartDate(employeePTOBank.StartDate, ptoYearStart, day);

				ptoAdjustmentDetail.EffectiveStartDate = effectiveStartDate;
				ptoAdjustmentDetail.BalanceLimit = employeePTOBank.AccrualLimit;
				PTOAdjustmentDetails.SetValueExt<PRPTOAdjustmentDetail.initialBalance>(ptoAdjustmentDetail, PTOHelper.GetPTOHistory(this, effectiveDate, bAccountID, employeePTOBank).AvailableHours);
			}
		}

		protected virtual bool OpenPaychecksExist()
		{
			PRPTOAdjustment adjustment = Document.Current;

			int[] employeeIDs = PTOAdjustmentDetails.Select<PRPTOAdjustmentDetail>().Select(item => item.BAccountID.GetValueOrDefault()).Distinct().ToArray();

			PRPayment[] openPaychecks = SelectFrom<PRPayment>
				.Where<PRPayment.employeeID.IsIn<P.AsInt>
					.And<PRPayment.released.IsEqual<False>>>
				.View.Select(this, employeeIDs).FirstTableItems.ToArray();

			if (openPaychecks.Length > 0)
			{
				foreach (PRPayment paycheck in openPaychecks)
				{
					PXTrace.WriteError(Messages.PaycheckIsNotReleased, paycheck.PaymentDocAndRef);
				}

				return true;
			}

			return false;
		}

		protected virtual bool LinkedPaychecksExist()
		{
			PRPayment[] linkedPaychecks = SelectFrom<PRPayment>
				.InnerJoin<PRPTOAdjustmentDetail>.On<PRPTOAdjustmentDetail.FK.Payment>
				.Where<PRPTOAdjustmentDetail.type.IsEqual<PRPTOAdjustment.type.FromCurrent>
					.And<PRPTOAdjustmentDetail.refNbr.IsEqual<PRPTOAdjustment.refNbr.FromCurrent>>
					.And<PRPayment.docType.IsNotEqual<PayrollType.voidCheck>>
					.And<PRPayment.voided.IsEqual<False>>>
				.View.Select(this).FirstTableItems.ToArray();

			if (linkedPaychecks.Length > 0)
			{
				foreach (PRPayment paycheck in linkedPaychecks)
				{
					PXTrace.WriteError(Messages.PTOAdjustmentAppliedToPaycheck, paycheck.PaymentDocAndRef);
				}

				return true;
			}

			return false;
		}

		#endregion Helper Methods
	}
}
