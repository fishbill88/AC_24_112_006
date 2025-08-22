import {
	columnConfig,
	gridConfig,
	headerDescription,
	GridColumnShowHideMode,
	PXActionState,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class Revisions extends PXView {
	ProjectID: PXFieldState;
	RevisionID: PXFieldState;
	Description: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Multiline>;
	@headerDescription
	FormCaptionDescription: PXFieldState;
}

export class Filter extends PXView {
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountGroupType: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class Project extends PXView {
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	initNewRow: false,
	allowInsert: false,
	allowDelete: true
})
export class Items extends PXView {
	AddPeriods: PXActionState;
	SettleBalances: PXActionState;
	AddMissingLines: PXActionState;

	@columnConfig({
		allowFilter: false,
		hideViewLink: true
	})
	ProjectTask: PXFieldState;
	@columnConfig({
		allowFilter: false,
		hideViewLink: true
	})
	AccountGroup: PXFieldState;
	@columnConfig({
		allowFilter: false,
		hideViewLink: true
	})
	Inventory: PXFieldState;
	@columnConfig({
		allowFilter: false,
		hideViewLink: true
	})
	CostCode: PXFieldState;
	@columnConfig({ allowFilter: false })
	Description: PXFieldState;
	@columnConfig({ allowFilter: false })
	PlannedStartDate: PXFieldState;
	@columnConfig({ allowFilter: false })
	PlannedEndDate: PXFieldState;
	@columnConfig({
		allowFilter: false,
		allowSort: false
	})
	Period: PXFieldState;
	@columnConfig({
		allowFilter: false,
		allowSort: false
	})
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		allowFilter: false,
		allowSort: false
	})
	CuryAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		allowFilter: false,
		allowSort: false
	})
	RevisedQty: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		allowFilter: false,
		allowSort: false
	})
	CuryRevisedAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		allowFilter: false,
		allowSort: false
	})
	DraftChangeOrderQty: PXFieldState;
	@columnConfig({
		allowFilter: false,
		allowSort: false
	})
	CuryDraftChangeOrderAmount: PXFieldState;
	@columnConfig({
		allowFilter: false,
		allowSort: false
	})
	ChangeOrderQty: PXFieldState;
	@columnConfig({
		allowFilter: false,
		allowSort: false
	})
	CuryChangeOrderAmount: PXFieldState;
	@columnConfig({
		allowFilter: false,
		allowSort: false
	})
	ActualQty: PXFieldState;
	@columnConfig({
		allowFilter: false,
		allowSort: false
	})
	CuryActualAmount: PXFieldState;
	@columnConfig({
		allowFilter: false,
		allowSort: false
	})
	VarianceQuantity: PXFieldState;
	@columnConfig({
		allowFilter: false,
		allowSort: false
	})
	CuryVarianceAmount: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	FinPeriodID: PXFieldState<PXFieldOptions.Hidden>;
}

export class AddPeriodDialog extends PXView {
	StartPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	EndPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CopyDialog extends PXView {
	RevisionID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class DistributeDialog extends PXView {
	ValueOption: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	Amount: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisedQty: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisedAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	ApplyOption: PXFieldState<PXFieldOptions.CommitChanges>;
}
