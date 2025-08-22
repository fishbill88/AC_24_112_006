import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, PXView, PXFieldState, gridConfig,
	linkCommand, columnConfig
} from "client-controls";

@graphInfo({graphType: "PX.Objects.GL.BatchPost", primaryView: "BatchList", })
export class GL502000 extends PXScreen {

	BatchList_batchNbr_ViewDetails: PXActionState;

	BatchList = createCollection(Batch);

}

@gridConfig({
	syncPosition: true,
	mergeToolbarWith: "ScreenToolbar",
	quickFilterFields: ["BatchNbr", "Description"],
	batchUpdate: true
})
export class Batch extends PXView {

	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;

	@columnConfig({ hideViewLink: true })
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
