import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	ICurrencyInfo,
	gridConfig,
	GridPreset
} from "client-controls";

export class ClaimDetails extends PXView {
	ClaimDetailCD: PXFieldState;
	ExpenseDate: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	CuryTranAmtWithTaxes: PXFieldState<PXFieldOptions.Disabled>;
	CuryTaxTotal: PXFieldState<PXFieldOptions.Disabled>;
	ClaimCuryTranAmtWithTaxes: PXFieldState<PXFieldOptions.Disabled>;
	CardCuryID: PXFieldState<PXFieldOptions.Disabled|PXFieldOptions.NoLabel>;
	BankTranStatus: PXFieldState<PXFieldOptions.Hidden>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CurrentClaimDetails extends PXView {
	TranDesc: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Multiline>;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitCost: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEmployeePart: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryExtCost: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryTipAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState;
	ExpenseRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	ContractID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	RefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	StatusClaim: PXFieldState<PXFieldOptions.Disabled>;
	PaidWith: PXFieldState<PXFieldOptions.CommitChanges>;
	CorpCardID: PXFieldState<PXFieldOptions.CommitChanges>;
	Hold: PXFieldState<PXFieldOptions.Disabled>;
	Approved: PXFieldState<PXFieldOptions.Disabled|PXFieldOptions.Hidden>;
	Rejected: PXFieldState<PXFieldOptions.Disabled|PXFieldOptions.Hidden>;
	Released: PXFieldState<PXFieldOptions.Disabled>;
	HoldClaim: PXFieldState<PXFieldOptions.Disabled>;
	TaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCalcMode: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCategoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	FSEntityTypeUI: PXFieldState<PXFieldOptions.CommitChanges>;
	FSEntityNoteID: PXFieldState<PXFieldOptions.CommitChanges>;
	FSBillable: PXFieldState<PXFieldOptions.CommitChanges>;
	Billable: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseSubID: PXFieldState;
	SalesAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesSubID: PXFieldState;
	CuryTaxRoundDiff: PXFieldState<PXFieldOptions.Disabled>;
	BankTranStatus: PXFieldState<PXFieldOptions.Disabled>;
	Category: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowDelete: false
})
export class Taxes extends PXView {
	TaxID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxRate: PXFieldState;
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
	preset: GridPreset.Details,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class Approval extends PXView {
	ApproverEmployee__AcctCD: PXFieldState;
	ApproverEmployee__AcctName: PXFieldState;
	WorkgroupID: PXFieldState;
	ApprovedByEmployee__AcctCD: PXFieldState;
	ApprovedByEmployee__AcctName: PXFieldState;
	OrigOwnerID: PXFieldState<PXFieldOptions.Hidden>;
	ApproveDate: PXFieldState;
	@columnConfig({
		allowUpdate: false
	})
	Status: PXFieldState;
	@columnConfig({
		allowUpdate: false
	})
	Reason: PXFieldState;
	AssignmentMapID: PXFieldState<PXFieldOptions.Hidden>;
	RuleID: PXFieldState<PXFieldOptions.Hidden>;
	StepID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
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

export class ReasonApproveRejectParams extends PXView {
	Reason: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Multiline>;
}

export class ReassignApprovalFilter extends PXView {
	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;
}