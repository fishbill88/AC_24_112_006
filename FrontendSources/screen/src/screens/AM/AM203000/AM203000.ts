import {
	PXScreen,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	gridConfig,
	GridPreset,
} from 'client-controls';

@gridConfig({
	preset: GridPreset.Primary,
	initNewRow: true,
})
export class AMMPSTypeRecords extends PXView {
	MPSTypeID: PXFieldState;
	Descr: PXFieldState;
	MPSNumberingID: PXFieldState;
	Dependent: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.MPSType', primaryView: 'AMMPSTypeRecords' })
export class AM203000 extends PXScreen {
	AMMPSTypeRecords = createCollection(AMMPSTypeRecords);
}
