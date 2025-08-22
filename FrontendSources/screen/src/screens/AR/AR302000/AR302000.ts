import {
	PXScreen, createSingle, createCollection, graphInfo, PXView, PXFieldState, gridConfig, columnConfig, linkCommand, PXActionState, PXFieldOptions, ICurrencyInfo, viewInfo
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AR.ARPaymentEntry', primaryView: 'Document', bpEventsIndicator: true, udfTypeField: 'DocType', showUDFIndicator: true })
export class AR302000 extends PXScreen {

	ViewDocumentToApply: PXActionState;
	ViewCurrentBatch: PXActionState;
	ViewApplicationDocument: PXActionState;
	NewCustomer: PXActionState;
	EditCustomer: PXActionState;
	ViewPPDVATAdj: PXActionState;

	@viewInfo({containerName: 'Payment Summary'})
	Document = createSingle(ARPayment);
	@viewInfo({containerName: 'Financial'})
	CurrentDocument = createSingle(ARPayment);
	CurrencyInfo = createSingle(CurrencyInfo);
	Approval = createCollection(EPApproval);

	@viewInfo({containerName: 'Documents to Apply'})
	Adjustments = createCollection(ARAdjust);
	ARPost = createCollection(ARTranPostBal);
	ccProcTran = createCollection(CCProcTran);
	PaymentCharges = createCollection(ARPaymentChargeTran);
	loadOpts = createSingle(LoadOptions);

	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);
	ReasonApproveRejectParams = createSingle(ReasonApproveRejectFilter);

}

export class ARPayment extends PXView {

	AdjustDocAmt: PXActionState;

	DocType: PXFieldState;
	RefNbr: PXFieldState;
	Status: PXFieldState;
	AdjDate: PXFieldState<PXFieldOptions.CommitChanges>;
	AdjFinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExtRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerLocationID: PXFieldState<PXFieldOptions.CommitChanges>;

	PaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	CCTransactionRefund: PXFieldState<PXFieldOptions.CommitChanges>;
	RefTranExtNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	PMInstanceID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProcessingCenterID: PXFieldState<PXFieldOptions.CommitChanges>;
	NewCard: PXFieldState<PXFieldOptions.CommitChanges>;
	NewAccount: PXFieldState<PXFieldOptions.CommitChanges>;
	SaveCard: PXFieldState<PXFieldOptions.CommitChanges>;
	SaveAccount: PXFieldState<PXFieldOptions.CommitChanges>;
	CCPaymentStateDescr: PXFieldState;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState;

	DepositAfter: PXFieldState<PXFieldOptions.CommitChanges>;
	ChkServiceManagement: PXFieldState;
	DocDesc: PXFieldState<PXFieldOptions.Multiline>;
	CuryOrigDocAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryApplAmt: PXFieldState<PXFieldOptions.Readonly>;
	CurySOApplAmt: PXFieldState<PXFieldOptions.Readonly>;
	CuryUnappliedBal: PXFieldState<PXFieldOptions.Readonly>;
	CuryInitDocBal: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Readonly>;
	CuryWOAmt: PXFieldState<PXFieldOptions.Readonly>;
	CuryChargeAmt: PXFieldState<PXFieldOptions.Readonly>;
	CuryConsolidateChargeTotal: PXFieldState<PXFieldOptions.Readonly>;

	ViewOriginalDocument: PXActionState;

	BatchNbr: PXFieldState;
	DisplayCuryInitDocBal: PXFieldState;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	ARAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ARSubID: PXFieldState;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("ViewOriginalDocument")
	OrigRefNbr: PXFieldState;

	DocDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	Cleared: PXFieldState<PXFieldOptions.CommitChanges>;
	ClearDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DepositAsBatch: PXFieldState<PXFieldOptions.CommitChanges>;
	Deposited: PXFieldState;
	DepositDate: PXFieldState;
	DepositNbr: PXFieldState;
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

@gridConfig({ syncPosition: true })
export class ARAdjust extends PXView {

	LoadInvoices: PXActionState;
	AutoApply: PXActionState;

	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AdjdBranchID: PXFieldState;
	AdjdDocType: PXFieldState;

	@linkCommand("ViewDocumentToApply")
	AdjdRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	AdjdLineNbr: PXFieldState<PXFieldOptions.CommitChanges>;

	ARTran__InventoryID: PXFieldState;
	ARTran__ProjectID: PXFieldState;
	ARTran__TaskID: PXFieldState;
	ARTran__CostCodeID: PXFieldState;
	ARTran__AccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AdjdCustomerID: PXFieldState;

	CuryAdjgAmt: PXFieldState;
	CuryAdjgPPDAmt: PXFieldState;
	CuryAdjgWOAmt: PXFieldState;

	@columnConfig({ hideViewLink: true })
	WriteOffReasonCode: PXFieldState;
	AdjdDocDate: PXFieldState;
	ARRegisterAlias__DueDate: PXFieldState;
	ARInvoice__DiscDate: PXFieldState;
	AdjdCuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDocBal: PXFieldState;
	CuryDiscBal: PXFieldState;
	ARRegisterAlias__DocDesc: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AdjdCuryID: PXFieldState;
	AdjdFinPeriodID: PXFieldState;
	ARInvoice__InvoiceNbr: PXFieldState;
	HasExpiredComplianceDocuments: PXFieldState;
}

export class ARTranPostBal extends PXView {

	ReverseApplication: PXActionState;

	ViewPPDCrMemo: PXActionState;

	@columnConfig({ hideViewLink: true })
	ARRegisterAlias__BranchID: PXFieldState;

	@linkCommand("ViewCurrentBatch")
	BatchNbr: PXFieldState;

	SourceDocType: PXFieldState;

	@linkCommand("ViewApplicationDocument")
	SourceRefNbr: PXFieldState;

	LineNbr: PXFieldState;
	ARTran__InventoryID: PXFieldState;
	ARTran__ProjectID: PXFieldState;
	ARTran__TaskID: PXFieldState;
	ARTran__CostCodeID: PXFieldState;
	ARTran__AccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ARRegisterAlias__CustomerID: PXFieldState;

	CuryAmt: PXFieldState;
	CuryPPDAmt: PXFieldState;
	CuryWOAmt: PXFieldState;
	ApplicationDate: PXFieldState;
	FinPeriodID: PXFieldState;
	ARRegisterAlias__DocDate: PXFieldState;
	ARRegisterAlias__DueDate: PXFieldState;
	ARInvoice__DiscDate: PXFieldState;
	CuryBalanceAmt: PXFieldState;
	CuryDiscBalanceAmt: PXFieldState;
	ARRegisterAlias__DocDesc: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ARRegisterAlias__CuryID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ARRegisterAlias__FinPeriodID: PXFieldState;

	ARInvoice__InvoiceNbr: PXFieldState;
	ARAdjust2__PendingPPD: PXFieldState;

	@linkCommand("ViewPPDCrMemo")
	ARAdjust2__PPDCrMemoRefNbr: PXFieldState;
	ARAdjust2__TaxInvoiceNbr: PXFieldState;
	ARAdjust2__HasExpiredComplianceDocuments: PXFieldState;
}

export class CCProcTran extends PXView {

	TranNbr: PXFieldState;
	ProcessingCenterID: PXFieldState;
	TranType: PXFieldState;
	TranStatus: PXFieldState;
	Amount: PXFieldState;
	FundHoldExpDate: PXFieldState;
	RefTranNbr: PXFieldState;
	PCTranNumber: PXFieldState;
	AuthNumber: PXFieldState;
	PCResponseReasonText: PXFieldState;
	StartTime: PXFieldState;
	ProcStatus: PXFieldState;
	CVVVerificationStatus: PXFieldState;
	ErrorSource: PXFieldState;
	ErrorText: PXFieldState;
}

@gridConfig({ syncPosition: true })
export class ARPaymentChargeTran extends PXView {

	EntryTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	TranDesc: PXFieldState;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState;
	CuryTranAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class EPApproval extends PXView {

	ApproverEmployee__AcctCD: PXFieldState;
	ApproverEmployee__AcctName: PXFieldState;
	WorkgroupID: PXFieldState;
	ApprovedByEmployee__AcctCD: PXFieldState;
	ApprovedByEmployee__AcctName: PXFieldState;
	OrigOwnerID: PXFieldState;
	ApproveDate: PXFieldState;
	Status: PXFieldState;
	AssignmentMapID: PXFieldState;
	RuleID: PXFieldState;
	StepID: PXFieldState;
	CreatedDateTime: PXFieldState;
}

//<!--#include file = "~\Pages\Includes\CRApprovalReasonPanel.inc"-- >
export class ReasonApproveRejectFilter extends PXView {
	Reason: PXFieldState<PXFieldOptions.CommitChanges>;
}

//<!--#include file = "~\Pages\Includes\EPReassignApproval.inc"-- >
export class ReassignApprovalFilter extends PXView {
	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class LoadOptions extends PXView {

	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	FromDate: PXFieldState;
	TillDate: PXFieldState;
	MaxDocs: PXFieldState;
	StartRefNbr: PXFieldState;
	EndRefNbr: PXFieldState;
	StartOrderNbr: PXFieldState;
	EndOrderNbr: PXFieldState;
	Apply: PXFieldState;
	LoadChildDocuments: PXFieldState;
	OrderBy: PXFieldState;
	SOOrderBy: PXFieldState;
}
