import {
	createCollection,
	createSingle,
	PXScreen,
	PXView,
	graphInfo,
	PXFieldState,
	PXFieldOptions,
} from "client-controls";

import { CSAttributeGroup } from "../../interfaces/CR/AttributeGroup";

@graphInfo({
	graphType: "PX.Objects.EP.EmployeeClassMaint",
	primaryView: "EmployeeClass",
})
export class EP202000 extends PXScreen {
	EmployeeClass = createSingle(EPEmployeeClass);
	Mapping = createCollection(CSAttributeGroup);
}

export class EPEmployeeClass extends PXView {
	VendorClassID: PXFieldState;
	Descr: PXFieldState;
	TermsID: PXFieldState;
	PaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	CashAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	APAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	APSubID: PXFieldState;
	DiscTakenAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscTakenSubID: PXFieldState;
	PrepaymentAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepaymentSubID: PXFieldState;
	ExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseSubID: PXFieldState;
	SalesAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesSubID: PXFieldState;
	CuryID: PXFieldState;
	AllowOverrideCury: PXFieldState;
	CuryRateTypeID: PXFieldState;
	AllowOverrideRate: PXFieldState;
	CalendarID: PXFieldState;
	TaxZoneID: PXFieldState;
	HoursValidation: PXFieldState;
	DefaultDateInActivity: PXFieldState;
	ProbationPeriodMonths: PXFieldState<PXFieldOptions.CommitChanges>;
}
