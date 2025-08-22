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

export class Filter extends PXView {
	EstimateID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisionID: PXFieldState<PXFieldOptions.CommitChanges>;
	UpdateAllRevisions: PXFieldState<PXFieldOptions.CommitChanges>;
	ReuseExistingInventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState;
	RevisionDate: PXFieldState;
	CopyDetailedDescription: PXFieldState<PXFieldOptions.CommitChanges>;
	CopyFiles: PXFieldState<PXFieldOptions.CommitChanges>;
	CopyNotes: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
})
export class NonInventoryItems extends PXView {
	Selected: PXFieldState;
	Level: PXFieldState;
	@columnConfig({ hideViewLink: true }) InventoryCD: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	@columnConfig({ hideViewLink: true }) ItemClassID: PXFieldState;
	@columnConfig({ hideViewLink: true }) SiteID: PXFieldState;
	@columnConfig({ hideViewLink: true }) BaseUnit: PXFieldState;
	@columnConfig({ hideViewLink: true }) TaxCategoryID: PXFieldState;
	@columnConfig({ hideViewLink: true }) PostClassID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LotSerClassID: PXFieldState;
	StockItem: PXFieldState;
	OriginalInventoryCD: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.CreateInventoryItemProcess', primaryView: 'Filter' })
export class AM507000 extends PXScreen {
	Filter = createSingle(Filter);
	NonInventoryItems = createCollection(NonInventoryItems);
}
