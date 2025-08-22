import {
	columnConfig,
	gridConfig,
	headerDescription,
	linkCommand,
	GridColumnShowHideMode,
	ICurrencyInfo,
	PXActionState,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class ExpenseClaim extends PXView {
	Hold: PXFieldState<PXFieldOptions.Hidden>;
	Approved: PXFieldState<PXFieldOptions.Hidden>;
	RefNbr: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	DocDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ApproveDate: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Disabled>;
	DocDesc: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Multiline>;
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState;
	DepartmentID: PXFieldState<PXFieldOptions.Disabled>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDocBal: PXFieldState<PXFieldOptions.Disabled>;
	CuryVatTaxableTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryVatExemptTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryTaxTotal: PXFieldState<PXFieldOptions.Disabled>;
	@headerDescription
	FormCaptionDescription: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class ReceiptsForSubmit extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	ClaimDetailCD: PXFieldState;
	ExpenseDate: PXFieldState;
	ExpenseRefNbr: PXFieldState;
	EmployeeID: PXFieldState;
	BranchID: PXFieldState;
	@linkCommand("ViewUnsubmitReceipt")
	TranDesc: PXFieldState;
	CuryTranAmtWithTaxes: PXFieldState;
	CuryID: PXFieldState;
	Status: PXFieldState;
}

export class CustomerUpdateAsk extends PXView {
	CustomerUpdateAnswer: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ExpenseClaimDetailsCurrent extends PXView {
	CuryTaxRoundDiff: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true
})
export class Tax_Rows extends PXView {
	TaxID: PXFieldState;
	TaxRate: PXFieldState;
	CuryTaxableAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryTaxAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	NonDeductibleTaxRate: PXFieldState;
	CuryExpenseAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	Tax__TaxType: PXFieldState;
	Tax__PendingTax: PXFieldState;
	Tax__ReverseTax: PXFieldState;
	Tax__ExemptTax: PXFieldState;
	Tax__StatisticalTax: PXFieldState;
}

export class ExpenseClaimCurrent extends PXView {
	CuryTaxRoundDiff: PXFieldState<PXFieldOptions.Disabled>;
	TaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCalcMode: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	initNewRow: true
})
export class ExpenseClaimDetails extends PXView {
	createNew: PXActionState;
	ShowSubmitReceipt: PXActionState;

	ClaimDetailCD: PXFieldState;
	ExpenseDate: PXFieldState;
	ExpenseRefNbr: PXFieldState;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("EditDetail")
	TranDesc: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitCost: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryExtCost: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Disabled>;
	@linkCommand("ViewTaxes")
	CuryTaxTotal: PXFieldState;
	CuryEmployeePart: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryTipAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryNetAmount: PXFieldState;
	CuryTranAmtWithTaxes: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	ClaimCuryTranAmtWithTaxes: PXFieldState;
	Status: PXFieldState;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Billable: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewProject")
	ContractID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	PaidWith: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	CorpCardID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	ExpenseAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	ExpenseSubID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SalesAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	SalesSubID: PXFieldState;
	TaxCategoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxZoneID: PXFieldState;
	TaxCalcMode: PXFieldState;
	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	ARRefNbr: PXFieldState;
	@linkCommand("ViewAPInvoice")
	APRefNbr: PXFieldState;
	EmployeeID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	CustomerID_Customer_acctName: PXFieldState<PXFieldOptions.Disabled|PXFieldOptions.Hidden>;
	ProjectDescription: PXFieldState;
	ProjectTaskDescription: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	allowInsert: false,
	allowDelete: false
})
export class Taxes extends PXView {
	RefNbr: PXFieldState<PXFieldOptions.Hidden>;
	TaxID: PXFieldState;
	TaxRate: PXFieldState<PXFieldOptions.Disabled>;
	CuryTaxableAmt: PXFieldState;
	CuryTaxAmt: PXFieldState;
	NonDeductibleTaxRate: PXFieldState;
	CuryExpenseAmt: PXFieldState;
	Tax__TaxType: PXFieldState;
	Tax__PendingTax: PXFieldState;
	Tax__ReverseTax: PXFieldState;
	Tax__ExemptTax: PXFieldState;
	Tax__StatisticalTax: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	suppressNoteFiles: true,
	showBottomBar: false,
	fastFilterByAllFields: false
})
export class APDocuments extends PXView {
	DocType: PXFieldState;
	@linkCommand("ViewInvoice")
	RefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryOrigDocAmt: PXFieldState;
	TaxZoneID: PXFieldState;
	TaxCalcMode: PXFieldState;
	Status: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class Approval extends PXView {
	ApproverEmployee__AcctCD: PXFieldState;
	ApproverEmployee__AcctName: PXFieldState;
	@columnConfig({ hideViewLink: true })
	WorkgroupID: PXFieldState;
	ApprovedByEmployee__AcctCD: PXFieldState;
	ApprovedByEmployee__AcctName: PXFieldState;
	OrigOwnerID: PXFieldState<PXFieldOptions.Hidden>;
	ApproveDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Status: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Reason: PXFieldState;
	AssignmentMapID: PXFieldState<PXFieldOptions.Hidden>;
	RuleID: PXFieldState<PXFieldOptions.Hidden>;
	StepID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}

export class ReasonApproveRejectParams extends PXView {
	Reason: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Multiline>;
}

export class ReassignApprovalFilter extends PXView {
	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CurrencyInfo extends PXView implements ICurrencyInfo {
	CuryInfoID: PXFieldState;
	BaseCuryID: PXFieldState;
	BaseCalc: PXFieldState;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisplayCuryID: PXFieldState;
	CuryRateTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	BasePrecision: PXFieldState;
	CuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEffDate: PXFieldState<PXFieldOptions.CommitChanges>;
	RecipRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleCuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleRecipRate: PXFieldState<PXFieldOptions.CommitChanges>;
}
