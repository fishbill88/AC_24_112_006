import {
	columnConfig,
	gridConfig,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class Filter extends PXView {
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	EffectiveDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	UnionID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	initNewRow: true
})
export class Items extends PXView {
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
	UnionID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeID_description: PXFieldState;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	EmploymentType: PXFieldState<PXFieldOptions.CommitChanges>;
	RegularHours: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualSalary: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	WageRate: PXFieldState<PXFieldOptions.CommitChanges>;
	Rate: PXFieldState<PXFieldOptions.CommitChanges>;
	BurdenRate: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
	ExtRefNbr: PXFieldState;
	EffectiveDate: PXFieldState<PXFieldOptions.CommitChanges>;
}
