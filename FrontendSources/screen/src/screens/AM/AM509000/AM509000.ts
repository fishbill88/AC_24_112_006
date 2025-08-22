import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	GridPreset,
	linkCommand,
} from 'client-controls';

export class UpdateBomMatlRecs extends PXView {
	CurrInvID: PXFieldState<PXFieldOptions.CommitChanges>;
	CurrSubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	EffStartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	NewInvID: PXFieldState<PXFieldOptions.CommitChanges>;
	NewSubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
})
export class SelectedBoms extends PXView {
	Selected: PXFieldState;
	@linkCommand("ViewBOM") BOMID: PXFieldState;
	RevisionID: PXFieldState;
	AMBomItem__Status: PXFieldState;
	@columnConfig({ hideViewLink: true }) OperationID: PXFieldState;
	@columnConfig({ hideViewLink: true }) AMBomItem__InventoryID: PXFieldState;
	AMBomItem__InventoryID_description: PXFieldState;
	@columnConfig({ hideViewLink: true }) AMBomItem__SiteID: PXFieldState;
	QtyReq: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	Descr: PXFieldState;
	BatchSize: PXFieldState;
	MaterialType: PXFieldState;
	PhantomRouting: PXFieldState;
	BFlush: PXFieldState;
	CompBOMID: PXFieldState;
	CompBOMRevisionID: PXFieldState;
	@columnConfig({ hideViewLink: true }) SiteID: PXFieldState;
	LocationID: PXFieldState;
	ScrapFactor: PXFieldState;
	BubbleNbr: PXFieldState;
	EffDate: PXFieldState;
	ExpDate: PXFieldState;
	AMBomItem__EffStartDate: PXFieldState;
	AMBomItem__EffEndDate: PXFieldState;
	LineID: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.BOMMassChange', primaryView: 'UpdateBomMatlRecs' })
export class AM509000 extends PXScreen {
	UpdateBomMatlRecs = createSingle(UpdateBomMatlRecs);
	SelectedBoms = createCollection(SelectedBoms);
}
