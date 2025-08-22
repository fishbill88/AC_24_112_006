import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	columnConfig,
	gridConfig,
	GridPreset,
} from 'client-controls';

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
})
export class ECRRecords extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	ECRID: PXFieldState;
	RevisionID: PXFieldState;
	@columnConfig({ hideViewLink: true }) BOMID: PXFieldState;
	@columnConfig({ hideViewLink: true }) BOMRevisionID: PXFieldState;
	@columnConfig({ hideViewLink: true }) InventoryID: PXFieldState;
	InventoryItem__Descr: PXFieldState;
	@columnConfig({ hideViewLink: true }) SiteID: PXFieldState;
	Priority: PXFieldState;
	RequestDate: PXFieldState;
	EffectiveDate: PXFieldState;
	@columnConfig({ hideViewLink: true }) Requestor: PXFieldState;
	Descr: PXFieldState;
}

export class Filter extends PXView {
	MergeECRs: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.CreateECOsProcess', primaryView: 'ECRRecords' })
export class AM514000 extends PXScreen {
	Filter = createSingle(Filter);
	ECRRecords = createCollection(ECRRecords);
}
