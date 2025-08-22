import {
	PXScreen,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	GridPreset,
} from 'client-controls';

@gridConfig({
	preset: GridPreset.Primary,
})
export class ShiftRecords extends PXView {
	ShiftCD: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	DiffType: PXFieldState;
	ShftDiff: PXFieldState;
	AMCrewSize: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.ShiftMaint', primaryView: 'ShiftRecords' })
export class AM205000 extends PXScreen {
	ShiftRecords = createCollection(ShiftRecords);
}
