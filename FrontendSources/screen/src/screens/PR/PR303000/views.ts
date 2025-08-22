import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig
} from "client-controls";

export class Document extends PXView {
	RefNbr: PXFieldState;
	Status: PXFieldState;
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Amendment: PXFieldState<PXFieldOptions.CommitChanges>;
	AmendedRefNbr: PXFieldState;
	ReasonForROE: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodType: PXFieldState;
	Comments: PXFieldState<PXFieldOptions.CommitChanges>;
	DocDesc: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CurrentDocument extends PXView {
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	CRAPayrollAccountNumber: PXFieldState;
	FirstDayWorked: PXFieldState<PXFieldOptions.CommitChanges>;
	LastDayForWhichPaid: PXFieldState<PXFieldOptions.CommitChanges>;
	FinalPayPeriodEndingDate: PXFieldState<PXFieldOptions.CommitChanges>;
	VacationPay: PXFieldState;
	TotalInsurableHours: PXFieldState;
	TotalInsurableEarnings: PXFieldState;
}

export class Address extends PXView {
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class StatutoryHolidays extends PXView {
	Date: PXFieldState;
	Amount: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class OtherMonies extends PXView {
	TypeCD: PXFieldState;
	TypeCD_Description: PXFieldState;
	Amount: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class InsurableEarnings extends PXView {
	PayPeriodID: PXFieldState;
	InsurableEarnings: PXFieldState;
	InsurableHours: PXFieldState;
}

