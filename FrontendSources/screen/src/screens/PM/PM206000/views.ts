import {
	gridConfig,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class RateSequence extends PXView {
	RateTableID: PXFieldState<PXFieldOptions.CommitChanges>;
	RateTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Sequence: PXFieldState<PXFieldOptions.CommitChanges>;
	RateCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.Multiline>;
}

export class RateDefinition extends PXView {
	Project: PXFieldState;
	Employee: PXFieldState;
	AccountGroup: PXFieldState;
	Task: PXFieldState;
	RateItem: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true
})
export class Items extends PXView {
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryItem__Descr: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true,
	fastFilterByAllFields: false
})
export class Rates extends PXView {
	StartDate: PXFieldState;
	EndDate: PXFieldState;
	Rate: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true
})
export class Projects extends PXView {
	ProjectCD: PXFieldState;
	PMProject__Description: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true
})
export class Tasks extends PXView {
	TaskCD: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true
})
export class AccountGroups extends PXView {
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	PMAccountGroup__Description: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true
})
export class Employees extends PXView {
	EmployeeID: PXFieldState;
	BAccount__AcctName: PXFieldState;
}
