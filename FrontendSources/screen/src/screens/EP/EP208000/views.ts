import {
	columnConfig,
	gridConfig,
	GridColumnShowHideMode,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class Equipment extends PXView {
	EquipmentCD: PXFieldState;
	Description: PXFieldState<PXFieldOptions.Multiline>;
	Status: PXFieldState;
}

export class EquipmentProperties extends PXView {
	FixedAssetID: PXFieldState;
	RunRateItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	SetupRateItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	SuspendRateItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	RunRate: PXFieldState;
	SetupRate: PXFieldState;
	SuspendRate: PXFieldState;
	DefaultAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultSubID: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true
})
export class Rates extends PXView {
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	PMProject__Description: PXFieldState;
	RunRate: PXFieldState;
	SetupRate: PXFieldState;
	SuspendRate: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	EquipmentID: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	allowInsert: false,
	allowDelete: false
})
export class Answers extends PXView {
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	AttributeID: PXFieldState;
	isRequired: PXFieldState;
	Value: PXFieldState;
}
