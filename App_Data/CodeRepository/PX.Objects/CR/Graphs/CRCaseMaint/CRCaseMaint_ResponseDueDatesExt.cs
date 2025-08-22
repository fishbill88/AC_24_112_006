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
using PX.Objects.CN.Common.Extensions;
using PX.Objects.CS;

namespace PX.Objects.CR.CRCaseMaint_Extensions
{
	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	public class CRCaseMaint_ResponseDueDatesExt : PXGraphExtension<CRCaseMaint>
	{
		#region Selects

		[PXHidden]
		public SelectFrom<
				CRClassSeverityTime>
			.Where<
				CRClassSeverityTime.caseClassID.IsEqual<CRCase.caseClassID.FromCurrent>
				.And<CRClassSeverityTime.severity.IsEqual<CRCase.severity.FromCurrent>>>
			.View
			TargetConfigRecord;

		#endregion

		#region ctor

		private const string OriginalCaseFieldStatesSlotName = nameof(CRCase) + nameof(OriginalCaseFieldStatesSlotName);

		public override void Initialize()
		{
			base.Initialize();

			AddHeaderFields();
		}

		#endregion

		#region Methods

		public const string HeaderInitialResponseDueDateTimeFieldName = "Header" + nameof(CRCase.InitialResponseDueDateTime);
		public const string HeaderResponseDueDateTimeFieldName = "Header" + nameof(CRCase.ResponseDueDateTime);
		public const string HeaderResolutionDueDateTimeFieldName = "Header" + nameof(CRCase.ResolutionDueDateTime);

		public virtual void AddHeaderFields()
		{
			Base.Caches[typeof(CRCase)].Fields.Add(HeaderInitialResponseDueDateTimeFieldName);
			Base.Caches[typeof(CRCase)].Fields.Add(HeaderResponseDueDateTimeFieldName);
			Base.Caches[typeof(CRCase)].Fields.Add(HeaderResolutionDueDateTimeFieldName);

			Base.FieldSelecting.AddHandler(typeof(CRCase), HeaderInitialResponseDueDateTimeFieldName, (s, e) =>
			{
				if (s.GetStateExt<CRCase.initialResponseDueDateTime>(e.Row) is not PXDateState sourceState)
					return;

				e.ReturnState = CreateCopyFieldStateWithVisibilityCondition(
					sourceState: sourceState,
					fieldName: HeaderInitialResponseDueDateTimeFieldName,
					predicate: e.Row is CRCase { InitialResponseDueDateTime: { } initialResponse, ResponseDueDateTime: var response }
								&& (response is null || initialResponse <= response));
			});

			Base.FieldSelecting.AddHandler(typeof(CRCase), HeaderResponseDueDateTimeFieldName, (s, e) =>
			{
				if (s.GetStateExt<CRCase.responseDueDateTime>(e.Row) is not PXDateState sourceState)
					return;

				e.ReturnState = CreateCopyFieldStateWithVisibilityCondition(
					sourceState: sourceState,
					fieldName: HeaderResponseDueDateTimeFieldName,
					predicate: e.Row is CRCase { InitialResponseDueDateTime: var initialResponse, ResponseDueDateTime: { } response }
								&& (initialResponse is null || response < initialResponse));
			});

			Base.FieldSelecting.AddHandler(typeof(CRCase), HeaderResolutionDueDateTimeFieldName, (s, e) =>
			{
				if (s.GetStateExt<CRCase.resolutionDueDateTime>(e.Row) is not PXDateState sourceState)
					return;

				e.ReturnState = CreateCopyFieldStateWithVisibilityCondition(
					sourceState: sourceState,
					fieldName: HeaderResolutionDueDateTimeFieldName,
					predicate: e.Row is CRCase { IsActive: true });
			});

			PXFieldState CreateCopyFieldStateWithVisibilityCondition(PXDateState sourceState, string fieldName, bool predicate)
			{
				return PXDateState.CreateInstance(PXFieldState.CreateInstance(
						value: sourceState.Value,
						dataType: sourceState.DataType,
						isKey: sourceState.PrimaryKey,
						nullable: sourceState.Nullable,
						required: 0,
						precision: sourceState.Precision,
						length: sourceState.Length,
						defaultValue: sourceState.DefaultValue,
						fieldName: fieldName,
						descriptionName: sourceState.DescriptionName,
						displayName: sourceState.DisplayName,
						error: sourceState.Error,
						errorLevel: sourceState.ErrorLevel,
						enabled: sourceState.Enabled,
						visible: sourceState.Visible && predicate,
						readOnly: sourceState.IsReadOnly,
						visibility: sourceState.Visibility,
						viewName: sourceState.ViewName,
						fieldList: sourceState.FieldList,
						headerList: sourceState.HeaderList
					),
					fieldName: fieldName,
					isKey: sourceState.PrimaryKey,
					required: 0,
					inputMask: sourceState.InputMask,
					displayMask: sourceState.DisplayMask,
					minValue: sourceState.MinValue,
					maxValue: sourceState.MaxValue);
			}
		}

		public virtual void SetStateForOriginalFields(CRCase @case)
		{
			// optimization because original goest to db in WF when changing currents
			var slot = PXContext.EnsureSlot(OriginalCaseFieldStatesSlotName, () =>
			{
				object[] originalCurrents = { Base.Case.Cache.GetOriginal(@case) };
				var originalTcr = @case == null || !PXAccess.FeatureInstalled<CS.FeaturesSet.caseCommitmentsTracking>()
					? null
					: TargetConfigRecord.View.SelectSingleBound(originalCurrents) as CRClassSeverityTime;

				var originalClass = @case == null
					? null
					: Base.Class.View.SelectSingleBound(originalCurrents) as CRCaseClass;

				return new
				{
					originalTcr?.TrackInitialResponseTime,
					originalTcr?.TrackResponseTime,
					originalTcr?.TrackResolutionTime,
					originalClass?.TrackSolutionsInActivities,
				};
			});


			Base.Case.Cache
				.AdjustUI(@case)
				.For<CRCase.initialResponseDueDateTime>(ui => ui.Visible = slot.TrackInitialResponseTime is true)
				.For<CRCase.responseDueDateTime>(ui => ui.Visible = slot.TrackResponseTime is true)
				.For<CRCase.resolutionDueDateTime>(ui => ui.Visible = slot.TrackResolutionTime is true)
				.For<CRCase.solutionActivityNoteID>(ui => ui.Visible = ui.Enabled = slot.TrackSolutionsInActivities is true);

		}

		[PXOverride]
		public virtual void PostPersist()
		{
			// hack to properly fill state of ref fields of case to show header fields
			// because after refresh header fields states collected before row selected
			PXContext.ClearSlot(OriginalCaseFieldStatesSlotName);
			SetStateForOriginalFields(Base.Case.Current);
		}

		#endregion

		#region Events

		[PXDefault]
		[PXMergeAttributes(Method = MergeMethod.Append)]
		protected virtual void _(Events.CacheAttached<CRCase.reportedOnDateTime> e) { }

		[PXFormula(typeof(Concat<TypeArrayOf<IBqlOperand>.FilledWith<Selector<CRActivity.type, EPActivityType.description>, Space, PX.Data.RTrim<CRActivity.startDate>, Space, CRActivity.subject>>))]
		[PXMergeAttributes(Method = MergeMethod.Append)]
		protected virtual void _(Events.CacheAttached<CRActivity.selectorDescription> e) { }

		protected virtual void _(Events.RowSelected<CRCase> e)
		{
			SetStateForOriginalFields(e.Row);
		}

		#endregion
	}
}
