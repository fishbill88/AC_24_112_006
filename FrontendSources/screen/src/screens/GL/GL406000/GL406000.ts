import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, PXPageLoadBehavior, PXView,
	PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode
} from "client-controls";

@graphInfo({
	graphType: "PX.GLAnomalyDetection.AnomalyTransactionEnq", primaryView: "Filter",
	pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues
})
export class GL406000 extends PXScreen {

	ViewSubspace: PXActionState;
	ViewBatch: PXActionState;
	ViewDocument: PXActionState;
	ViewReclassBatch: PXActionState;
	ViewPMTran: PXActionState;

	Filter = createSingle(AnomalyTransactionFilter);
	Transactions = createCollection(GLTran);
	Subspaces = createCollection(MLSubspaceScore);
}

export class AnomalyTransactionFilter extends PXView {

	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	LedgerID: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;
	ScoreTreshold: PXFieldState<PXFieldOptions.CommitChanges>;
	AmountTreshold: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ syncPosition: true, allowDelete: false, allowInsert: false, mergeToolbarWith: "ScreenToolbar" })
export class GLTran extends PXView {

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server, allowCheckAll: true })
	Selected: PXFieldState;

	MLStatusUI: PXFieldState;

	@linkCommand("ViewSubspace")
	MLScore: PXFieldState;

	Module: PXFieldState;

	@linkCommand("ViewBatch")
	BatchNbr: PXFieldState;

	TranDate: PXFieldState;
	TranDesc: PXFieldState;

	@linkCommand("ViewDocument")
	RefNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState;

	DebitAmt: PXFieldState;
	CreditAmt: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ReferenceID: PXFieldState;

	LineNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	FinPeriodID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	TranPeriodID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	InventoryID: PXFieldState;

	@linkCommand("ViewReclassBatch")
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	ReclassBatchNbr: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	IncludedInReclassHistory: PXFieldState<PXFieldOptions.Hidden>;

	@linkCommand("ViewPMTran")
	@columnConfig({ hideViewLink: true })
	PMTranID: PXFieldState;
}

@gridConfig({ statusField: "TextForSubspacesGrid" })
export class MLSubspaceScore extends PXView {

	MLScore: PXFieldState;
	SubspaceDescr: PXFieldState;
	AnomalyType: PXFieldState;
	Reason: PXFieldState;
}
