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
export class LaborCodeRecords extends PXView {
	LaborType: PXFieldState;
	LaborCodeID: PXFieldState;
	Descr: PXFieldState;
	@columnConfig({ hideViewLink: true }) LaborAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) LaborSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) OverheadAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) OverheadSubID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@graphInfo({ graphType: 'PX.Objects.AM.LaborCodeMaint', primaryView: 'LaborCodeRecords' })
export class AM206500 extends PXScreen {
	LaborCodeRecords = createCollection(LaborCodeRecords);
}
