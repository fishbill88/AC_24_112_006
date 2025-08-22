import {
	createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, PXFieldOptions,
	linkCommand, columnConfig, PXActionState
} from "client-controls";

@graphInfo({ graphType: "PX.Objects.CA.CAReconEnq", primaryView: "Filter", pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues })
export class CA302010 extends PXScreen {

	ViewDoc: PXActionState;

	@viewInfo({ containerName: "Create Reconciliation" })
	cashAccountFilter = createSingle(CashAccountFilter);

	@viewInfo({ containerName: "Selection" })
	Filter = createSingle(CAEnqFilter);

	@viewInfo({ containerName: "Reconciliation Statements" })
	CAReconRecords = createCollection(CARecon);

}

export class CashAccountFilter extends PXView {

	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class CAEnqFilter extends PXView {

	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ initNewRow: true, syncPosition: true, mergeToolbarWith: "ScreenToolbar" })
export class CARecon extends PXView {

	Status: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CashAccountID: PXFieldState;

	CashAccountID_CashAccount_descr: PXFieldState;

	@linkCommand("ViewDoc")
	ReconNbr: PXFieldState;

	ReconDate: PXFieldState;
	LastReconDate: PXFieldState;
	CuryBegBalance: PXFieldState;
	CuryReconciledDebits: PXFieldState;
	CuryReconciledCredits: PXFieldState;
	CuryBalance: PXFieldState;
	CountDebit: PXFieldState;
	CountCredit: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	CuryID: PXFieldState;

}
