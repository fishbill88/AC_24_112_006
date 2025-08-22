import {
	createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs,
	PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings,
	PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, TextAlign
} from "client-controls";

@graphInfo({ graphType: "PX.Objects.GL.JournalEntryImport", primaryView: "Map", bpEventsIndicator: true, showActivitiesIndicator: true, showUDFIndicator: true })
export class GL303010 extends PXScreen {

	Process: PXActionState;
	ProcessAll: PXActionState;

	@viewInfo({ containerName: "Import Summary" })
	Map = createSingle(GLTrialBalanceImportMap);
	@viewInfo({ containerName: "Transaction Details" })
	MapDetails = createCollection(GLTrialBalanceImportDetails);
	@viewInfo({ containerName: "Transaction Details" })
	Operations = createSingle(OperationParam);
	@viewInfo({ containerName: "Exceptions" })
	Exceptions = createCollection(GLHistoryEnquiryResult);
}

export class GLTrialBalanceImportMap extends PXView {

	// eslint-disable-next-line id-denylist
	Number: PXFieldState;

	Status: PXFieldState<PXFieldOptions.Disabled>;
	ImportDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	LedgerID: PXFieldState<PXFieldOptions.CommitChanges>;
	BatchNbr: PXFieldState<PXFieldOptions.Disabled>;
	Description: PXFieldState<PXFieldOptions.CommitChanges>;
	DebitTotalBalance: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Disabled>;
	CreditTotalBalance: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Disabled>;
	TotalBalance: PXFieldState<PXFieldOptions.CommitChanges>;
	IsEditable: PXFieldState<PXFieldOptions.Disabled>;
}

export class GLTrialBalanceImportDetails extends PXView {

	Validate: PXActionState;
	MergeDuplicates: PXActionState;

	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;

	Status: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ImportBranchCD: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	MapBranchID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ImportAccountCD: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	MapAccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ImportSubAccountCD: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	MapSubAccountID: PXFieldState;

	YtdBalance: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	CuryYtdBalance: PXFieldState<PXFieldOptions.CommitChanges>;

	AccountType: PXFieldState;
	Description: PXFieldState;
}

export class OperationParam extends PXView {
	Action: PXFieldState;
}

export class GLHistoryEnquiryResult extends PXView {

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountCD: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState;

	Type: PXFieldState;
	Description: PXFieldState;
	LastActivityPeriod: PXFieldState;
	BegBalance: PXFieldState;
	PtdDebitTotal: PXFieldState;
	PtdCreditTotal: PXFieldState;
	EndBalance: PXFieldState;
}
