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
} from 'client-controls';

@gridConfig({
	preset: GridPreset.Primary,
})
export class OvhdRecords extends PXView {
	OvhdID: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	OvhdType: PXFieldState;
	CostRate: PXFieldState;
	@columnConfig({ hideViewLink: true }) AcctID: PXFieldState;
	@columnConfig({ hideViewLink: true }) SubID: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.OvhdMaint', primaryView: 'OvhdRecords' })
export class AM202500 extends PXScreen {
	OvhdRecords = createCollection(OvhdRecords);
}
