import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	GridColumnShowHideMode,
	linkCommand,
	columnConfig,
	gridConfig
} from "client-controls";

export class Document extends PXView {
	BatchNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState;
	Hold: PXFieldState<PXFieldOptions.CommitChanges>;
	PayrollType: PXFieldState<PXFieldOptions.CommitChanges>;
	PayGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	PayPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState;
	EndDate: PXFieldState;
	TransactionDate: PXFieldState;
	DocDesc: PXFieldState;
	NumberOfEmployees: PXFieldState;
}

export class PRBatchTotalsFilter extends PXView {
	TotalHourQty: PXFieldState;
	TotalEarnings: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true,
	allowInsert: false
})
export class Transactions extends PXView {
	AddEmployees: PXActionState;
	ViewEarningDetails: PXActionState;

	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False
	})
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Hidden>;
	AcctCD: PXFieldState;
	AcctName: PXFieldState;
	HourQty: PXFieldState<PXFieldOptions.CommitChanges>;
	Rate: PXFieldState<PXFieldOptions.CommitChanges>;
	Amount: PXFieldState;
	@linkCommand("ViewPayCheck")
	PaymentDocAndRef: PXFieldState;
	@linkCommand("ViewVoidPayCheck")
	VoidPaymentDocAndRef: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true,
	initNewRow: true
})
export class EarningDetails extends PXView {
	CopySelectedEmployeeEarningDetailLine: PXActionState;
	ImportTimeActivities: PXActionState;

	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False
	})
	AllowCopy: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({
		hideViewLink: true
	})
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeID_Description: PXFieldState;
	@columnConfig({
		hideViewLink: true
	})
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	Date: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true
	})
	TypeCD: PXFieldState<PXFieldOptions.CommitChanges>;
	TypeCD_EPEarningType_Description: PXFieldState;
	@columnConfig({
		hideViewLink: true
	})
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Hours: PXFieldState<PXFieldOptions.CommitChanges>;
	Units: PXFieldState<PXFieldOptions.CommitChanges>;
	UnitType: PXFieldState;
	Rate: PXFieldState<PXFieldOptions.CommitChanges>;
	ManualRate: PXFieldState<PXFieldOptions.CommitChanges>;
	Amount: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true
	})
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true
	})
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true
	})
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true
	})
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CertifiedJob: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true
	})
	UnionID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true
	})
	LabourItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true
	})
	WorkCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShiftID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewTimeActivity")
	SourceNoteID: PXFieldState;
	ExcelRecordID: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true
})
export class Deductions extends PXView {
	@columnConfig({
		allowCheckAll: true
	})
	IsEnabled: PXFieldState;
	CodeID: PXFieldState;
	PRDeductCode__Description: PXFieldState;
	PRDeductCode__ContribType: PXFieldState;
	PRDeductCode__DedCalcType: PXFieldState;
	PRDeductCode__DedAmount: PXFieldState;
	PRDeductCode__DedPercent: PXFieldState;
	PRDeductCode__CntCalcType: PXFieldState;
	PRDeductCode__CntAmount: PXFieldState;
	PRDeductCode__CntPercent: PXFieldState;
	PRDeductCode__IsGarnishment: PXFieldState;
}

export class CurrentDocument extends PXView {
	ApplyOvertimeRules: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true
})
export class BatchOvertimeRules extends PXView {
	@columnConfig({
		allowCheckAll: true
	})
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	OvertimeRuleID: PXFieldState;
	PROvertimeRule__Description: PXFieldState;
	@columnConfig({
		hideViewLink: true
	})
	PROvertimeRule__DisbursingTypeCD: PXFieldState;
	PROvertimeRule__OvertimeMultiplier: PXFieldState;
	PROvertimeRule__RuleType: PXFieldState;
	PROvertimeRule__OvertimeThreshold: PXFieldState;
	PROvertimeRule__WeekDay: PXFieldState;
	PROvertimeRule__NumberOfConsecutiveDays: PXFieldState;
	PROvertimeRule__State: PXFieldState;
	PROvertimeRule__UnionID: PXFieldState;
	PROvertimeRule__ProjectID: PXFieldState;
}

export class AddEmployeeFilter extends PXView {
	EmployeeClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeType: PXFieldState<PXFieldOptions.CommitChanges>;
	UseQuickPay: PXFieldState<PXFieldOptions.CommitChanges>;
	UseTimeSheets: PXFieldState<PXFieldOptions.CommitChanges>;
	UseSalesComm: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true
})
export class Employees extends PXView {
	ToggleSelected: PXActionState;

	Selected: PXFieldState;
	AcctCD: PXFieldState;
	AcctName: PXFieldState;
	EmployeeClassID: PXFieldState;
	EmpType: PXFieldState;
	ActivePositionID: PXFieldState;
	DepartmentID: PXFieldState;
}

export class CurrentTransaction extends PXView {
	AcctCD: PXFieldState<PXFieldOptions.Disabled>;
	EmpType: PXFieldState<PXFieldOptions.CommitChanges>;
	RegularAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	ManualRegularAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	HourQty: PXFieldState<PXFieldOptions.Disabled>;
	Amount: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true,
	initNewRow: true
})
export class EmployeeEarningDetails extends PXView {
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False
	})
	AllowCopy: PXFieldState<PXFieldOptions.Hidden>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	Date: PXFieldState<PXFieldOptions.CommitChanges>;
	TypeCD: PXFieldState<PXFieldOptions.CommitChanges>;
	TypeCD_EPEarningType_Description: PXFieldState;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Hours: PXFieldState<PXFieldOptions.CommitChanges>;
	Units: PXFieldState<PXFieldOptions.CommitChanges>;
	UnitType: PXFieldState;
	Rate: PXFieldState<PXFieldOptions.CommitChanges>;
	ManualRate: PXFieldState<PXFieldOptions.CommitChanges>;
	Amount: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CertifiedJob: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	UnionID: PXFieldState<PXFieldOptions.CommitChanges>;
	LabourItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShiftID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ImportTimeActivitiesFilter extends PXView {
	ShowImportedActivities: PXFieldState<PXFieldOptions.CommitChanges>;
	AddSelectedTimeActivities: PXActionState;
	AddSelectedTimeActivitiesAndClose: PXActionState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true
})
export class TimeActivities extends PXView {
	ToggleSelectedTimeActivities: PXActionState;

	Selected: PXFieldState;
	OwnerID: PXFieldState;
	OwnerID_Description: PXFieldState;
	Branch__BranchCD: PXFieldState;
	Date: PXFieldState;
	TimeSpent: PXFieldState;
	EarningTypeID: PXFieldState;
	ProjectID: PXFieldState;
	ProjectTaskID: PXFieldState;
	CertifiedJob: PXFieldState;
	UnionID: PXFieldState;
	LabourItemID: PXFieldState;
	CostCodeID: PXFieldState;
	WorkCodeID: PXFieldState;
}

export class UpdateTaxesPopupView extends PXView {
	RedirectTaxMaintenance: PXActionState;

	Message: PXFieldState<PXFieldOptions.Disabled>;
}

