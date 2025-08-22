import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, PXViewCollection, PXView, PXFieldState,
	gridConfig, PXFieldOptions, columnConfig, linkCommand } from "client-controls";

@graphInfo({graphType: "PX.Objects.GL.BatchRelease", primaryView: "BatchList", })
export class GL501000 extends PXScreen {

	BatchList_batchNbr_ViewDetails: PXActionState;

	BatchList = createCollection(Batch);
}

@gridConfig({
	syncPosition: true, allowInsert: false,
	mergeToolbarWith: "ScreenToolbar",
	quickFilterFields: ["BatchNbr", "Description"]
})
export class Batch extends PXView {

	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;

	BranchID: PXFieldState;
	Module: PXFieldState;
	@linkCommand("BatchList_batchNbr_ViewDetails")
	BatchNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	LedgerID: PXFieldState;

	DateEntered: PXFieldState;
	LastModifiedByID_Modifier_Username: PXFieldState;

	@columnConfig({ hideViewLink: true })
	FinPeriodID: PXFieldState;

	ControlTotal: PXFieldState;
	Description: PXFieldState;
}
