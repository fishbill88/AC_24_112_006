import {
	createCollection, createSingle, PXScreen, graphInfo, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior,
	PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, PXActionState
} from "client-controls";

@graphInfo({graphType: "PX.Objects.GL.VoucherRelease", primaryView: "Documents"})
export class GL501500 extends PXScreen {

	EditDetail: PXActionState;

	Documents = createCollection(GLDocBatch);
}


@gridConfig({
	syncPosition: true,
	mergeToolbarWith: "ScreenToolbar",
	quickFilterFields: ["BatchNbr", "Description"]
})
export class GLDocBatch extends PXView {

	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;

	Module: PXFieldState;

	@linkCommand("EditDetail")
	BatchNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	LedgerID: PXFieldState;

	DateEntered: PXFieldState;
	LastModifiedByID_description: PXFieldState;

	@columnConfig({ hideViewLink: true })
	FinPeriodID: PXFieldState;

	ControlTotal: PXFieldState;
	Description: PXFieldState;
}
