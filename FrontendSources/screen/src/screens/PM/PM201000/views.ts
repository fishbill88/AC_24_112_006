import {
	columnConfig,
	gridConfig,
	GridColumnShowHideMode,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class AccountGroup extends PXView {
	GroupCD: PXFieldState;
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
	IsExpense: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.Multiline>;
	SortOrder: PXFieldState;
	IsActive: PXFieldState;
	RevenueAccountGroupID: PXFieldState;
}

export class AccountGroupProperties extends PXView {
	CreatesCommitment: PXFieldState;
	DefaultLineMarkupPct: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class Accounts extends PXView {
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	Type: PXFieldState<PXFieldOptions.Disabled>;
	AccountClassID: PXFieldState<PXFieldOptions.Disabled>;
	Description: PXFieldState<PXFieldOptions.Disabled>;
	CuryID: PXFieldState;
	IsDefault: PXFieldState<PXFieldOptions.CommitChanges>;
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
