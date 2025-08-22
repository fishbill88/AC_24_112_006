import {
	PXScreen, createSingle, createCollection, graphInfo, PXView, PXFieldState, PXFieldOptions, columnConfig, gridConfig, TextAlign, ICurrencyInfo, PXActionState, linkCommand, GridColumnShowHideMode
} from 'client-controls';

@graphInfo({
	graphType: 'PX.Objects.GL.JournalEntry',
	primaryView: 'BatchModule',
	bpEventsIndicator: true,
	udfTypeField: "Module", showUDFIndicator: true
})
export class GL301000 extends PXScreen {

	BatchModule = createSingle(BatchModule);
	GLTranModuleBatNbr = createCollection(GLTranModuleBatNbr);
	Approval = createCollection(Approval);

	_Batch_CurrencyInfo_ = createSingle(CurrencyInfo);

	ReasonApproveRejectParams = createSingle(ReasonApproveRejectParams);
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);
}

export class BatchModule extends PXView {

	GLReversingBatches: PXActionState;

	Module: PXFieldState;
	BatchNbr: PXFieldState;
	Status: PXFieldState;
	DateEntered: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;

	Description: PXFieldState;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	LedgerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState;
	CuryInfoID: PXFieldState;
	AutoReverse: PXFieldState<PXFieldOptions.CommitChanges>;

	AutoReverseCopy: PXFieldState<PXFieldOptions.CommitChanges>;
	CreateTaxTrans: PXFieldState<PXFieldOptions.CommitChanges>;
	SkipTaxValidation: PXFieldState<PXFieldOptions.CommitChanges>;
	BatchType: PXFieldState;
	OrigBatchNbr: PXFieldState;

	@linkCommand("GLReversingBatches")
	ReverseCount: PXFieldState;

	CuryDebitTotal: PXFieldState;
	CuryCreditTotal: PXFieldState;
	CuryControlTotal: PXFieldState<PXFieldOptions.CommitChanges>;
	DataField: PXFieldState;

}

@gridConfig({ initNewRow: true, adjustPageSize: true, allowImport: true })
export class GLTranModuleBatNbr extends PXView {

	ViewDocument: PXActionState;
	ReclassificationHistory: PXActionState;
	ViewPMTran: PXActionState;
	ViewReclassBatch: PXActionState;
	viewOrigBatch: PXActionState;

	LineNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;

	AccountID_Account_description: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;

	FinPeriodID: PXFieldState;
	TranPeriodID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;

	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("ViewPMTran")
	PMTranID: PXFieldState;
	RefNbr: PXFieldState;
	TranDate: PXFieldState;
	Qty: PXFieldState;
	UOM: PXFieldState;
	CuryDebitAmt: PXFieldState;
	CuryCreditAmt: PXFieldState;
	TranDesc: PXFieldState;
	InventoryID: PXFieldState;

	@columnConfig({ hideViewLink: true, allowShowHide: GridColumnShowHideMode.True })
	LedgerID: PXFieldState;
	ReferenceID: PXFieldState;

	@columnConfig({ hideViewLink: true, allowShowHide: GridColumnShowHideMode.Server })
	TaxID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true, allowShowHide: GridColumnShowHideMode.Server })
	TaxCategoryID: PXFieldState<PXFieldOptions.CommitChanges>;

	NonBillable: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	CuryReclassRemainingAmt: PXFieldState;

	@linkCommand("ViewReclassBatch")
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	ReclassBatchNbr: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	OrigModule: PXFieldState;

	@linkCommand("viewOrigBatch")
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	OrigBatchNbr: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	OrigLineNbr: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	IncludedInReclassHistory: PXFieldState;
}


@gridConfig({ allowDelete: false, allowInsert: false, allowUpdate: false, adjustPageSize: true })
export class Approval extends PXView {

	@columnConfig({ hideViewLink: true })
	ApproverEmployee__AcctCD: PXFieldState;

	ApproverEmployee__AcctName: PXFieldState;

	@columnConfig({ hideViewLink: true })
	WorkgroupID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ApprovedByEmployee__AcctCD: PXFieldState;

	ApprovedByEmployee__AcctName: PXFieldState;

	OrigOwnerID: PXFieldState;
	ApproveDate: PXFieldState;
	Status: PXFieldState;
	Reason: PXFieldState;
	AssignmentMapID: PXFieldState;
	RuleID: PXFieldState;
	StepID: PXFieldState;
	CreatedDateTime: PXFieldState;
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
	Reason: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ReassignApprovalFilter extends PXView {
	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;
}
