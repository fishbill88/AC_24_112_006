import {
	createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs,
	PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings,
	PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, TextAlign
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.AR.ARCreateWriteOff", primaryView: "Filter",
	pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues
})
export class AR505000 extends PXScreen {

	showCustomer: PXActionState;
	editDetail: PXActionState;
	editCustomer: PXActionState;

	@viewInfo({ containerName: "Selection" })
	Filter = createSingle(ARWriteOffFilter);
	@viewInfo({ containerName: "Documents" })
	ARDocumentList = createCollection(ARRegister);
}

export class ARWriteOffFilter extends PXView {

	WOType: PXFieldState<PXFieldOptions.CommitChanges>;
	WODate: PXFieldState<PXFieldOptions.CommitChanges>;
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	WOFinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	ReasonCode: PXFieldState<PXFieldOptions.CommitChanges>;
	WOLimit: PXFieldState<PXFieldOptions.CommitChanges>;
	SelTotal: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	syncPosition: true,
	mergeToolbarWith: "ScreenToolbar"
})
export class ARRegister extends PXView {

	@columnConfig({ allowUpdate: false, allowSort: false, textAlign: TextAlign.Center, allowCheckAll: true })
	Selected: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DocType: PXFieldState;

	@linkCommand("editDetail")
	RefNbr: PXFieldState;

	@linkCommand("editCustomer")
	@columnConfig({ allowUpdate: false })
	CustomerID: PXFieldState;

	CustomerID_BAccountR_acctName: PXFieldState;

	DocDate: PXFieldState;

	@columnConfig({ hideViewLink: true })
	FinPeriodID: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right, allowShowHide: GridColumnShowHideMode.Server, hideViewLink: true })
	CuryID: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right, allowShowHide: GridColumnShowHideMode.Server })
	CuryDocBal: PXFieldState;

	@columnConfig({ allowUpdate: false, textAlign: TextAlign.Right })
	DocBal: PXFieldState;

	DocDesc: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ReasonCode: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right, allowShowHide: GridColumnShowHideMode.Server })
	CuryOrigDocAmt: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	OrigDocAmt: PXFieldState;
}
