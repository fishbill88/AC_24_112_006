import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
    GridPagerMode
} from "client-controls";

export class EmployeeClass extends PXView {
	EmployeeClassID: PXFieldState;
	Descr: PXFieldState;
}

export class CurEmployeeClassRecord extends PXView {
	EmpType: PXFieldState<PXFieldOptions.CommitChanges>;
	PayGroupID: PXFieldState;
	CalendarID: PXFieldState<PXFieldOptions.CommitChanges>;
	HoursPerWeek: PXFieldState;
	StdWeeksPerYear: PXFieldState<PXFieldOptions.CommitChanges>;
	HoursPerYear: PXFieldState;
	OverrideHoursPerYearForCertified: PXFieldState<PXFieldOptions.CommitChanges>;
	HoursPerYearForCertified: PXFieldState;
	ExemptFromOvertimeRules: PXFieldState;
	NetPayMin: PXFieldState;
	GrnMaxPctNet: PXFieldState;
	WorkCodeID: PXFieldState;
	UnionID: PXFieldState;
	ExemptFromCertifiedReporting: PXFieldState;
	UsePayrollProjectWorkLocation: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	pagerMode: GridPagerMode.InfiniteScroll,
	initNewRow: true
})
export class WorkLocations extends PXView {
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	PRLocation__Description: PXFieldState;
	IsDefault: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class EmployeeClassPTOBanks extends PXView {
	IsActive: PXFieldState;
	BankID: PXFieldState<PXFieldOptions.CommitChanges>;
	BankID_Description: PXFieldState;
	StartDate: PXFieldState;
	AccrualMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	AccrualRate: PXFieldState;
	HoursPerYear: PXFieldState;
	AccrualLimit: PXFieldState;
	CarryoverType: PXFieldState<PXFieldOptions.CommitChanges>;
	CarryoverAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	FrontLoadingAmount: PXFieldState;
	DisbursingType: PXFieldState;
}

