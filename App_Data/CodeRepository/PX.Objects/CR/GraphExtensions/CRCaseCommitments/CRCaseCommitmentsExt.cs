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

#nullable enable
using PX.Common;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CS.Services.WorkTimeCalculation;
using System;
using PX.Data.BQL;
using PX.Data.PushNotifications;
using PX.Objects.CS;

// case, oldcase, ...
using CaseDto = PX.Data.PXResult<PX.Objects.CR.CRCase, PX.Objects.CR.CRCase?, PX.Objects.CR.CRCaseClass, PX.Objects.CR.CRActivityStatistics?, PX.Objects.CR.CRClassSeverityTime?, PX.Objects.CR.CRPMTimeActivity?>;

namespace PX.Objects.CR.Extensions.CRCaseCommitments
{
	public abstract class CRCaseCommitmentsExt<TGraph, TActivity> : CRCaseCommitmentsExt<TGraph>
		where TGraph : PXGraph
		where TActivity : CRActivity, new()
	{
		[PXOverride]
		public virtual void Persist(Action persist)
		{
			using (var scope = new PXTransactionScope())
			{
				persist();

				TryCalculateCommitmentsForRelatedCase(TryGetActivity());

				scope.Complete(Base);
			}
		}

		#region Methods

		public virtual void TryCalculateCommitmentsForRelatedCase(TActivity? activity)
		{
			if (activity == null)
				return;

			if (activity.RefNoteIDType == typeof(CRCase).FullName
				&& activity.RefNoteID is { } refNoteID
				&& new EntityHelper(Base).GetEntityRow(typeof(CRCase), refNoteID) is CRCase caseEntity)
			{
				var cache = Base.Caches[typeof(CRCase)];

				var originalCaseEntity = cache.CreateCopy(caseEntity);

				SetSolutionProvidedForRelatedCaseIfNeeded(activity, caseEntity);

				CalculateCaseCommitments(caseEntity);

				if (cache.ObjectsEqual<CRCase.initialResponseDueDateTime, CRCase.responseDueDateTime, CRCase.resolutionDueDateTime, CRCase.solutionActivityNoteID>(caseEntity, originalCaseEntity))
					return;

				cache.Update(caseEntity);

				using (new SuppressPushNotificationsScope())
				{
					cache.PersistUpdated(caseEntity);
				}

				cache.Clear();
			}
		}

		public virtual TActivity? TryGetActivity() => (TActivity)Base.Caches[typeof(TActivity)].Current;

		public virtual void SetSolutionProvidedForRelatedCaseIfNeeded(TActivity? activity, CRCase caseEntity)
		{
			if (activity is { ProvidesCaseSolution: true, UIStatus: ActivityStatusListAttribute.Completed }
				&& caseEntity is { SolutionActivityNoteID: null })
			{
				caseEntity.SolutionActivityNoteID = activity.NoteID;
			}
		}

		#endregion

		#region Events

		// optimization to remove extra calls to db, because we don't change status here
		[PXDBDate]
		[PXMergeAttributes(Method = MergeMethod.Replace)]
		protected virtual void _(Events.CacheAttached<CRCase.statusDate> e) { }

		// optimization to remove extra calls to db, because we don't change status here
		[PXDBInt]
		[PXMergeAttributes(Method = MergeMethod.Replace)]
		protected virtual void _(Events.CacheAttached<CRCase.statusRevision> e) { }

		#endregion
	}

	public abstract class CRCaseCommitmentsExt<TGraph> : PXGraphExtension<TGraph>
		where TGraph : PXGraph
	{
		#region Methods

		protected static bool IsExtensionActive() => PXAccess.FeatureInstalled<FeaturesSet.caseCommitmentsTracking>();

		public virtual void CalculateCaseCommitments(CRCase caseEntity, CRCase? oldCaseEntity = null)
		{
			if (caseEntity?.NoteID is null)
				return;

			var dto = GetData(caseEntity, oldCaseEntity);

			CalculateInitialResponseTime(dto);
			CalculateResponseTime(dto);
			CalculateResolutionTime(dto);
		}

		public virtual CaseDto GetData(CRCase caseEntity, CRCase? oldCaseEntity)
		{
			var caseClass = CRCaseClass.PK.Find(Base, caseEntity.CaseClassID, PKFindOptions.IncludeDirty);
			var stats = CRActivityStatistics.PK.Find(Base, caseEntity.NoteID, PKFindOptions.IncludeDirty);
			var severityTime = CRClassSeverityTime.PK.Find(Base, caseEntity.CaseClassID, caseEntity.Severity, PKFindOptions.IncludeDirty);

			CRPMTimeActivity firstUnansweredIncomingActivity =
				SelectFrom<CRPMTimeActivity>
					.Where<CRPMTimeActivity.refNoteID.IsEqual<@P.AsGuid>
						.And<CRPMTimeActivity.completedDate.IsGreaterEqual<@P.AsDateTime>>
						.And<CRPMTimeActivity.incoming.IsEqual<True>>
						.And<CRPMTimeActivity.isPrivate.IsNotEqual<True>>>
					.OrderBy<CRPMTimeActivity.completedDate.Asc>
					.View
					.SelectSingleBound(Base, null, caseEntity.NoteID, stats?.LastOutgoingActivityDate);

			return new CaseDto(caseEntity, oldCaseEntity, caseClass, stats, severityTime, firstUnansweredIncomingActivity);
		}

		public virtual void CalculateInitialResponseTime(CaseDto dto)
		{
			var (caseEntity, _, caseClass, stats, tcr) = dto;

			if (caseEntity.IsActive is true
				&& tcr is {TrackInitialResponseTime: true}
				&& stats?.InitialOutgoingActivityCompletedAtDate is null)
			{
				caseEntity.InitialResponseDueDateTime = AddTargetTime(caseClass, caseEntity.ReportedOnDateTime, tcr.InitialResponseTimeTarget);

				CalculateGracePeriodIfNeeded<CRCase.initialResponseDueDateTime, CRClassSeverityTime.initialResponseGracePeriod>(dto);
			}
			else if (caseEntity.IsActive is not true
				|| tcr is null or { TrackInitialResponseTime: not true }
				|| stats is { InitialOutgoingActivityCompletedAtDate: not null })
			{
				caseEntity.InitialResponseDueDateTime = null;
			}
		}

		public virtual void CalculateResponseTime(CaseDto dto)
		{
			var (caseEntity, _, caseClass, stats, tcr, firstUnansweredIncomingActivity) = dto;

			if (caseEntity.IsActive is true
				&& tcr is {TrackResponseTime: true})
			{
				if (stats?.InitialOutgoingActivityCompletedAtDate is null)
				{
					caseEntity.ResponseDueDateTime = AddTargetTime(caseClass, caseEntity.ReportedOnDateTime, tcr.ResponseTimeTarget);
					CalculateGracePeriodIfNeeded<CRCase.responseDueDateTime, CRClassSeverityTime.responseGracePeriod>(dto);
				}
				else if (stats?.LastOutgoingActivityDate is null)
				{
					caseEntity.ResponseDueDateTime = AddTargetTime(caseClass, caseEntity.ReportedOnDateTime, tcr.ResponseTimeTarget);
				}
				else if (stats?.LastIncomingActivityDate > stats?.LastOutgoingActivityDate || stats?.LastOutgoingActivityDate is null)
				{
					caseEntity.ResponseDueDateTime = AddTargetTime(caseClass, firstUnansweredIncomingActivity?.CompletedDate ?? stats?.LastIncomingActivityDate, tcr.ResponseTimeTarget);
					CalculateGracePeriodIfNeeded<CRCase.responseDueDateTime, CRClassSeverityTime.responseGracePeriod>(dto);
				}
				else if (stats?.LastIncomingActivityDate is null || stats?.LastOutgoingActivityDate > stats?.LastIncomingActivityDate)
				{
					caseEntity.ResponseDueDateTime = null;
				}
			}
			else if (caseEntity.IsActive is not true
				|| tcr is null or { TrackInitialResponseTime: false, TrackResponseTime: false }
				|| stats?.LastIncomingActivityDate is null
				|| stats?.LastOutgoingActivityDate > stats?.LastIncomingActivityDate)
			{
				caseEntity.ResponseDueDateTime = null;
			}
		}

		public virtual void CalculateResolutionTime(CaseDto dto)
		{
			var (caseEntity, _, caseClass, _, tcr) = dto;

			if (tcr is {TrackResolutionTime: true})
			{
				bool trackByActivity = caseClass.StopTimeCounterType == CRCaseClass.stopTimeCounterType.CaseSolutionProvidedInActivity
					&& caseEntity.SolutionActivityNoteID is null;

				bool trackByCase = caseClass.StopTimeCounterType == CRCaseClass.stopTimeCounterType.CaseDeactivated
					&& caseEntity.IsActive is true;

				if (trackByActivity || trackByCase)
				{
					DateTime? newResolutionDue = AddTargetTime(caseClass, caseEntity.ReportedOnDateTime, tcr.ResolutionTimeTarget);
					if (newResolutionDue is not null)
						caseEntity.ResolutionDueDateTime = newResolutionDue.Value.AddMilliseconds(- newResolutionDue.Value.Millisecond);
					CalculateGracePeriodIfNeeded<CRCase.resolutionDueDateTime, CRClassSeverityTime.resolutionGracePeriod>(dto);
					return;
				}
			}

			bool trackByActivityProvided = caseClass.StopTimeCounterType == CRCaseClass.stopTimeCounterType.CaseSolutionProvidedInActivity
				&& caseEntity.SolutionActivityNoteID is not null;

			bool trackByCaseProvided = caseClass.StopTimeCounterType == CRCaseClass.stopTimeCounterType.CaseDeactivated
				&& caseEntity.IsActive is false;

			if (trackByActivityProvided || trackByCaseProvided)
			{
				caseEntity.ResolutionDueDateTime = null;
			}
		}

		public virtual DateTime? AddTargetTime(CRCaseClass caseClass, DateTime? dateTime, int? targetTimeInMinutes)
		{
			if (dateTime == null || targetTimeInMinutes == null)
				return null;

			var calculator = GetWorkTimeCalculator(caseClass);
			var timeSpan = TimeSpan.FromMinutes(targetTimeInMinutes.Value);
			var workTimeSpan = calculator.ToWorkTimeSpan(timeSpan);
			var result = calculator.AddWorkTime(DateTimeInfo.FromLocalTimeZone(dateTime.Value), workTimeSpan);

			return result.DateTime;
		}

		public virtual IWorkTimeCalculator GetWorkTimeCalculator(CRCaseClass caseClass)
		{
			var key = nameof(IWorkTimeCalculator) + "_" + caseClass.CalendarID;
			var calculator = PXContext.GetSlot<IWorkTimeCalculator>(key);

			if (calculator is not null)
				return calculator;

			return PXContext.SetSlot(key, WorkTimeCalculatorProvider.GetWorkTimeCalculator(caseClass.CalendarID));
		}

		public virtual void CalculateGracePeriodIfNeeded<TDueDate, TGracePeriod>(CaseDto dto)
			where TDueDate : IBqlField
			where TGracePeriod : IBqlField
		{
			var (caseEntity, oldCaseEntity, caseClass, _, tcr) = dto;
			if (oldCaseEntity == null)
				return;

			if (caseEntity.Severity == oldCaseEntity.Severity && caseEntity.CaseClassID == oldCaseEntity.CaseClassID)
				return;

			var cache = Base.Caches[typeof(CRCase)];
			var oldDue = cache.GetValue<TDueDate>(oldCaseEntity) as DateTime?;

			var now = PXTimeZoneInfo.Now;
			if (oldDue <= now)
				return;

			var newDue = cache.GetValue<TDueDate>(caseEntity) as DateTime?;
			var severityCache = Base.Caches[typeof(CRClassSeverityTime)];
			var gracePeriod = severityCache.GetValue<TGracePeriod>(tcr) as int?;
			var graceTime = AddTargetTime(caseClass, now, gracePeriod);
			if (newDue >= graceTime)
				return;

			cache.SetValue<TDueDate>(caseEntity, graceTime);
		}

		#endregion
	}

	public abstract class CRCaseDefaultReportedOnDateTimeExt<TGraph> : PXGraphExtension<TGraph>
		where TGraph : PXGraph
	{
		#region Methods

		public virtual void DefaultReportedOnDateTimeIfNeeded(CRCase row)
		{
			row.ReportedOnDateTime ??= PXTimeZoneInfo.Now;
		}

		#endregion

		#region Events

		protected virtual void _(Events.RowPersisting<CRCase> e)
		{

			CRCase row = e.Row;

			if (row == null || e.Operation == PXDBOperation.Delete)
				return;

			DefaultReportedOnDateTimeIfNeeded(row);

			e.Cache.Update(row);
		}

		#endregion
	}
}
