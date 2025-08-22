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
using PX.Data.BQL.Fluent;
using PX.Objects.CR.Extensions.CRCaseCommitments;
using System;
using System.Collections;
using PX.Common;
using PX.Data.BQL;
using PX.Objects.CR.Extensions;
using PX.Objects.CR.Workflows;
using PX.Objects.EP;
using PX.Objects.CS;
using PX.Objects.CS.Services.WorkTimeCalculation;
using System.Linq;

namespace PX.Objects.CR.CRCaseMaint_Extensions
{
	public class CRCaseClassMaint_CommitmentsExt : PXGraphExtension<CRCaseClassMaint>
	{
		public static bool IsActive() => PXAccess.FeatureInstalled<FeaturesSet.caseCommitmentsTracking>();

		#region Selects

		[PXViewName(Messages.CaseClassReaction)]
		public PXSelect<CRClassSeverityTime,
			Where<CRClassSeverityTime.caseClassID, Equal<Current<CRCaseClass.caseClassID>>>>
				CaseClassesReaction;

		protected virtual bool NeedTrackingSolutionsInActivities => CaseClassesReaction.SelectMain().Any(c => c.TrackResolutionTime is true);

		#endregion

		#region Events

		protected virtual void _(Events.RowPersisting<CRCaseClass> e)
		{
			var row = e.Row as CRCaseClass;

			if (row == null || e.Operation == PXDBOperation.Delete)
				return;

			if (Base.IsImport
				&& row.TrackSolutionsInActivities is false
				&& NeedTrackingSolutionsInActivities)
			{
				row.TrackSolutionsInActivities = true;
			}
		}

		protected virtual void _(Events.FieldUpdated<CRCaseClass, CRCaseClass.trackSolutionsInActivities> e)
		{
			if (e.Row == null)
				return;

			if (e.NewValue is false && e.OldValue is true && e.Row.StopTimeCounterType == CRCaseClass.stopTimeCounterType.CaseSolutionProvidedInActivity)
			{
				if (NeedTrackingSolutionsInActivities)
				{
					e.Cache.RaiseExceptionHandling<CRCaseClass.trackSolutionsInActivities>(
						e.Row, e.NewValue,
						new PXSetPropertyException<CRCaseClass.trackSolutionsInActivities>(MessagesNoPrefix.ConsiderTurnOnTrackSolutionsInActivities, PXErrorLevel.Warning));
				}
			}
		}

		protected virtual void _(Events.FieldUpdated<CRCaseClass, CRCaseClass.stopTimeCounterType> e)
		{
			if (e.Row == null)
				return;

			if (e.NewValue is CRCaseClass.stopTimeCounterType.CaseSolutionProvidedInActivity && e.Row.TrackSolutionsInActivities is false)
			{
				if (NeedTrackingSolutionsInActivities)
				{
					e.Cache.RaiseExceptionHandling<CRCaseClass.trackSolutionsInActivities>(
						e.Row, e.NewValue,
						new PXSetPropertyException<CRCaseClass.trackSolutionsInActivities>(MessagesNoPrefix.ConsiderTurnOnTrackSolutionsInActivities, PXErrorLevel.Warning));
				}
			}
		}

		protected virtual void _(Events.FieldUpdated<CRClassSeverityTime, CRClassSeverityTime.trackResolutionTime> e)
		{
			if (e.Row == null)
				return;

			if (NeedTrackingSolutionsInActivities && Base.CaseClasses.Current is { } current)
			{
				current.TrackSolutionsInActivities = true;

				Base.CaseClasses.Update(current);
			}
		}

		protected virtual void _(Events.FieldUpdated<CRCaseClass, CRCaseClass.calendarID> e)
		{
			if (e.Row == null
			 || e.NewValue is not string newCalendarId
			 || e.OldValue is not string oldCalendarId
			 || newCalendarId == oldCalendarId)
				return;

			var oldCalculator = WorkTimeCalculatorProvider.GetWorkTimeCalculator(oldCalendarId);
			var newCalculator = WorkTimeCalculatorProvider.GetWorkTimeCalculator(newCalendarId);

			foreach (var commitments in CaseClassesReaction.SelectMain())
			{
				Recalculate<CRClassSeverityTime.resolutionGracePeriod>();
				Recalculate<CRClassSeverityTime.resolutionTimeTarget>();
				Recalculate<CRClassSeverityTime.responseGracePeriod>();
				Recalculate<CRClassSeverityTime.responseTimeTarget>();
				Recalculate<CRClassSeverityTime.initialResponseGracePeriod>();
				Recalculate<CRClassSeverityTime.initialResponseTimeTarget>();

				CaseClassesReaction.Update(commitments);

				void Recalculate<TField>() where TField : IBqlField
				{
					if (CaseClassesReaction.Cache.GetValue<TField>(commitments) is not int oldValue)
						return;

					var oldTimeSpan = TimeSpan.FromMinutes(oldValue);
					var oldWorkTime = oldCalculator.ToWorkTimeSpan(oldTimeSpan);
					var newWorkTimeInfo = new WorkTimeInfo(oldWorkTime.RoundWorkdays, oldWorkTime.RoundHours, oldWorkTime.RoundMinutes);
					var newWorkTime = newCalculator.ToWorkTimeSpan(newWorkTimeInfo);

					CaseClassesReaction.Cache.SetValue<TField>(commitments, (int)newWorkTime.TotalMinutes);
				}
			}
		}

		#endregion

	}
}
