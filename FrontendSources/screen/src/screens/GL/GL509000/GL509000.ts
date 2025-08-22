import {
	createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent,
	CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig,
	headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig,
	GridColumnShowHideMode, GridColumnType
} from "client-controls";

@graphInfo({graphType: "PX.Objects.GL.GLConsolReadMaint", primaryView: "ConsolSetupRecords" })
export class GL509000 extends PXScreen {

	ConsolSetupRecords = createCollection(GLConsolSetup);
}


@gridConfig({ syncPosition: true, mergeToolbarWith: "ScreenToolbar" })
export class GLConsolSetup extends PXView {

	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	LedgerId: PXFieldState;

	Description: PXFieldState;
	Url: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SourceLedgerCD: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SourceBranchCD: PXFieldState;

	LastPostPeriod: PXFieldState;
	LastConsDate: PXFieldState;
}
