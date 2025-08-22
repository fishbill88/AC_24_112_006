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

using System;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.CS.Services.WorkTimeCalculation;

namespace PX.Objects.CR
{
	public class CRCaseClassMaint : PXGraph<CRCaseClassMaint, CRCaseClass>
	{
		#region Selects

		[PXViewName(Messages.CaseClass)]
		public PXSelect<CRCaseClass>
			CaseClasses;

		[PXHidden]
		public PXSelect<CRCaseClass,
			Where<CRCaseClass.caseClassID, Equal<Current<CRCaseClass.caseClassID>>>>
			CaseClassesCurrent;

		[PXViewName(Messages.Attributes)]
		public CSAttributeGroupList<CRCaseClass, CRCase> Mapping;

		[PXHidden]
		public PXSelect<CRSetup>
			Setup;

		public PXSelect<CRCaseClassLaborMatrix, Where<CRCaseClassLaborMatrix.caseClassID, Equal<Current<CRCaseClass.caseClassID>>>>
			LaborMatrix;

		public SelectFrom<CSCalendar>
			.Where<CSCalendar.calendarID.IsEqual<CRCaseClass.calendarID.FromCurrent>>
			.View
			WorkCalendar;

		#endregion

		#region Events

		#region CacheAttached

		[PXDBInt(MinValue = 0, MaxValue = 1440)]
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		protected virtual void _(Events.CacheAttached<CRCaseClass.reopenCaseTimeInDays> e) { }

		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDBDefault(typeof(CRCaseClass.caseClassID))]
		[PXMergeAttributes(Method = MergeMethod.Replace)]
		protected virtual void _(Events.CacheAttached<CRClassSeverityTime.caseClassID> e) { }

		[PXDefault(false)]
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		protected virtual void _(Events.CacheAttached<IN.InventoryItem.stkItem> e) { }

		#endregion

		protected virtual void _(Events.FieldVerifying<CRCaseClass, CRCaseClass.perItemBilling> e)
		{
			CRCaseClass row = e.Row as CRCaseClass;
			if (row == null) return;

			CRCase crCase = PXSelect<CRCase, Where<CRCase.caseClassID, Equal<Required<CRCaseClass.caseClassID>>,
								And<CRCase.isBillable, Equal<True>,
								And<CRCase.released, Equal<False>>>>>.SelectWindowed(this, 0, 1, row.CaseClassID);

			if (crCase != null)
			{
				throw new PXSetPropertyException(Messages.CurrentClassHasUnreleasedRelatedCases, PXErrorLevel.Error);
			}
		}

		protected virtual void _(Events.RowInserted<CRCaseClass> e)
		{
			var row = e.Row as CRCaseClass;
			if (row == null) return;

			if (row.IsBillable == true)
			{
				row.RequireCustomer = true;
			}
		}

		protected virtual void _(Events.RowSelected<CRCaseClass> e)
		{
			var row = e.Row as CRCaseClass;
			if (row == null) return;

			Delete.SetEnabled(NoCaseExistsForClass(row));
		}

		protected virtual void _(Events.RowDeleted<CRCaseClass> e)
		{
			var row = e.Row as CRCaseClass;
			if (row == null) return;

			CRSetup s = Setup.Select();

			if (s != null && s.DefaultCaseClassID == row.CaseClassID)
			{
				s.DefaultCaseClassID = null;
				Setup.Update(s);
			}
		}

		protected virtual void _(Events.RowDeleting<CRCaseClass> e)
		{
			var row = e.Row as CRCaseClass;
			if (row == null) return;

			if (!NoCaseExistsForClass(row))
			{
				throw new PXException(Messages.RecordIsReferenced);
			}
		}

		protected virtual void _(Events.RowPersisting<CRCaseClass> e)
		{
			var row = e.Row as CRCaseClass;

			if (row == null || e.Operation == PXDBOperation.Delete)
				return;

			var currentAllowValue = e.Cache.GetValue<CRCaseClass.allowEmployeeAsContact>(row);
			var oldAllowValue = e.Cache.GetValueOriginal<CRCaseClass.allowEmployeeAsContact>(row);

			if (object.Equals(currentAllowValue, oldAllowValue) || row.AllowEmployeeAsContact == true)
				return;

			if (!NoEmployeeCaseExistsForClass(row))
			{
				throw new PXException(Messages.CaseClassOfEmployeeCase);
			}
		}

		protected virtual void _(Events.FieldVerifying<CRCaseClass, CRCaseClass.calendarID> e)
		{
			if (e.Row == null)
				return;

			var calendar = CSCalendar.PK.Find(this, e.NewValue as string);

			if (calendar == null)
				return;

			bool OverlapsTheMidnight(bool? dayIsActive, DateTime? startDateTime, DateTime? endDateTime)
			{
				if (startDateTime == null || endDateTime == null || dayIsActive is false)
					return false;

				if (startDateTime.Value > endDateTime.Value)
					return true;

				return false;
			}

			if (OverlapsTheMidnight(calendar.MonWorkDay, calendar.MonStartTime, calendar.MonEndTime)
				|| OverlapsTheMidnight(calendar.TueWorkDay, calendar.TueStartTime, calendar.TueEndTime)
				|| OverlapsTheMidnight(calendar.WedWorkDay, calendar.WedStartTime, calendar.WedEndTime)
				|| OverlapsTheMidnight(calendar.ThuWorkDay, calendar.ThuStartTime, calendar.ThuEndTime)
				|| OverlapsTheMidnight(calendar.FriWorkDay, calendar.FriStartTime, calendar.FriEndTime)
				|| OverlapsTheMidnight(calendar.SatWorkDay, calendar.SatStartTime, calendar.SatEndTime)
				|| OverlapsTheMidnight(calendar.SunWorkDay, calendar.SunStartTime, calendar.SunEndTime)
				)
			{
				throw new PXSetPropertyException<CRCaseClass.calendarID>(MessagesNoPrefix.MidnightCalendarOverlapping, PXErrorLevel.Error);
			}
		}

		#endregion

		#region Methods

		protected virtual bool NoCaseExistsForClass(CRCaseClass row)
		{
			if (row == null)
				return true;

			CRCase c = PXSelect<
					CRCase,
				Where<
					CRCase.caseClassID, Equal<Required<CRCase.caseClassID>>>>
				.SelectWindowed(this, 0, 1, row.CaseClassID);

			return c == null;
		}

		protected virtual bool NoEmployeeCaseExistsForClass(CRCaseClass row)
		{
			if (row == null)
				return true;

			CRCase c = PXSelectJoin<
					CRCase,
				InnerJoin<Contact,
					On<Contact.contactID, Equal<CRCase.contactID>,
					And<Contact.contactType, Equal<ContactTypesAttribute.employee>>>>,
				Where<
					CRCase.caseClassID, Equal<Required<CRCase.caseClassID>>>>
				.SelectWindowed(this, 0, 1, row.CaseClassID);

			return c == null;
		}

		#endregion
	}
}
