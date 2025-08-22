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
} from 'client-controls';

export class Documents extends PXView {
	BOMID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisionID: PXFieldState<PXFieldOptions.CommitChanges>;
	Hold: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	EffStartDate: PXFieldState;
	EffEndDate: PXFieldState;
	Descr: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
})
export class BomAttribute extends PXView {
	LineNbr: PXFieldState;
	Level: PXFieldState;
	@columnConfig({ hideViewLink: true }) AttributeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) OperationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Label: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	Enabled: PXFieldState;
	TransactionRequired: PXFieldState;
	Value: PXFieldState;
	OrderFunction: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.BOMAttributeMaint', primaryView: 'Documents' })
export class AM208500 extends PXScreen {
	Documents = createSingle(Documents);
	BomAttributes = createCollection(BomAttribute);
}
