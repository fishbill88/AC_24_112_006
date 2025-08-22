import {
	createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXView, PXFieldState, gridConfig, ICurrencyInfo, PXFieldOptions, columnConfig, PXActionState
} from "client-controls";

@graphInfo({ graphType: "PX.Objects.CA.CATranEntry", primaryView: "CAAdjRecords", udfTypeField: "AdjTranType", showUDFIndicator: true })
export class CA304000 extends PXScreen {

	CAReversingTransactions: PXActionState;

	@viewInfo({ containerName: "Transaction Summary" })
	CAAdjRecords = createSingle(CAAdj);

	CurrentDocument = createSingle(CAAdj2);

	@viewInfo({ containerName: "Details" })
	CASplitRecords = createCollection(CASplit);

	@viewInfo({ containerName: "Taxes" })
	Taxes = createCollection(TaxTran);

	@viewInfo({ containerName: "Approvals" })
	Approval = createCollection(EPApproval);

	@viewInfo({ containerName: "Enter Reason" })
	ReasonApproveRejectParams = createSingle(ReasonApproveRejectFilter);

	@viewInfo({ containerName: "Reassign Approval" })
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);

	@viewInfo({ containerName: "currencyinfo" })
	currencyinfo = createSingle(CurrencyInfo);

}

export class CAAdj extends PXView {

	AdjTranType: PXFieldState<PXFieldOptions.Disabled>;
	AdjRefNbr: PXFieldState;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	Approved: PXFieldState<PXFieldOptions.Disabled>;
	TranDesc: PXFieldState<PXFieldOptions.Multiline>;
	TranDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	EntryTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	DrCr: PXFieldState<PXFieldOptions.Disabled>;
	ExtRefNbr: PXFieldState;
	EmployeeID: PXFieldState;
	OrigAdjRefNbr: PXFieldState<PXFieldOptions.Disabled>;
	ReverseCount: PXFieldState<PXFieldOptions.Disabled>;
	CuryTranAmt: PXFieldState<PXFieldOptions.Disabled>;
	CuryVatTaxableTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryVatExemptTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryTaxTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryTaxRoundDiff: PXFieldState<PXFieldOptions.Disabled>;
	CuryControlAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryTaxAmt: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class CAAdj2 extends PXView {

	TranID_CATran_batchNbr: PXFieldState<PXFieldOptions.Disabled>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	DepositAfter: PXFieldState<PXFieldOptions.CommitChanges>;
	Cleared: PXFieldState<PXFieldOptions.CommitChanges>;
	ClearDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DepositAsBatch: PXFieldState<PXFieldOptions.CommitChanges>;
	Deposited: PXFieldState;
	DepositDate: PXFieldState<PXFieldOptions.Disabled>;
	DepositNbr: PXFieldState;
	TaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	UsesManualVAT: PXFieldState<PXFieldOptions.Disabled>;
	TaxCalcMode: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ initNewRow: true, syncPosition: true })
export class CASplit extends PXView {

	@columnConfig({hideViewLink: true})
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({hideViewLink: true})
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;

	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState;
	UOM: PXFieldState;
	CuryUnitPrice: PXFieldState;
	CuryTranAmt: PXFieldState;

	@columnConfig({hideViewLink: true})
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({hideViewLink: true})
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({hideViewLink: true})
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;

	TaxCategoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	TranDesc: PXFieldState;
	LineNbr: PXFieldState<PXFieldOptions.Hidden>;

	@columnConfig({ allowUpdate: false })
	AccountID_description: PXFieldState;

	NonBillable: PXFieldState;

}

@gridConfig({ syncPosition: true })
export class TaxTran extends PXView {

	@columnConfig({ allowUpdate: false })
	TaxID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false, })
	TaxRate: PXFieldState;

	CuryTaxableAmt: PXFieldState;
	CuryTaxAmt: PXFieldState;
	CuryExemptedAmt: PXFieldState;
	NonDeductibleTaxRate: PXFieldState;
	CuryExpenseAmt: PXFieldState;
	Tax__TaxType: PXFieldState;
	Tax__PendingTax: PXFieldState;
	Tax__ReverseTax: PXFieldState;
	Tax__ExemptTax: PXFieldState;
	Tax__StatisticalTax: PXFieldState;

}

@gridConfig({ allowDelete: false, allowInsert: false })
export class EPApproval extends PXView {

	ApproverEmployee__AcctCD: PXFieldState;
	ApproverEmployee__AcctName: PXFieldState;
	WorkgroupID: PXFieldState;
	ApprovedByEmployee__AcctCD: PXFieldState;
	ApprovedByEmployee__AcctName: PXFieldState;
	OrigOwnerID: PXFieldState<PXFieldOptions.Hidden>;
	ApproveDate: PXFieldState;

	@columnConfig({ allowUpdate: false, allowNull: false })
	Status: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Reason: PXFieldState;

	AssignmentMapID: PXFieldState<PXFieldOptions.Hidden>;
	RuleID: PXFieldState<PXFieldOptions.Hidden>;
	StepID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;

}

export class ReasonApproveRejectFilter extends PXView {

	Reason: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class ReassignApprovalFilter extends PXView {

	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class CurrencyInfo extends PXView implements ICurrencyInfo {

	CuryInfoID: PXFieldState;
	BaseCuryID: PXFieldState;
	BaseCalc: PXFieldState;
	DisplayCuryID: PXFieldState;
	CuryRateTypeID: PXFieldState;
	BasePrecision: PXFieldState;
	CuryRate: PXFieldState;
	CuryEffDate: PXFieldState;
	RecipRate: PXFieldState;
	SampleCuryRate: PXFieldState;
	SampleRecipRate: PXFieldState;
	CuryID: PXFieldState;

}
