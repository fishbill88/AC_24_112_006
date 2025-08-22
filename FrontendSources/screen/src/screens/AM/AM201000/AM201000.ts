import {
	PXScreen,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	GridPreset,
	linkCommand,
} from 'client-controls';

@gridConfig({
	preset: GridPreset.Primary,
	initNewRow: true,
})
export class AMMPSRecord extends PXView {
	@columnConfig({ hideViewLink: true }) InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	PlanDate: PXFieldState;
	MPSTypeID: PXFieldState;
	@columnConfig({ hideViewLink: true }) SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	@linkCommand("ViewBOM") BOMID: PXFieldState;
	ActiveFlg: PXFieldState;
	MPSID: PXFieldState;
	InventoryID_description: PXFieldState;
	BOMID_description: PXFieldState;
	@columnConfig({ hideViewLink: true }) BranchID: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.MPSMaint', primaryView: 'AMMPSRecords' })
export class AM201000 extends PXScreen {
	AMMPSRecords = createCollection(AMMPSRecord);
}
