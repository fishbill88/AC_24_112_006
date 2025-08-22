import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXActionState,
	PXFieldOptions,
	gridConfig,
	GridPreset,
	linkCommand,
} from 'client-controls';

export class CapacityFilter extends PXView {
	MachID: PXFieldState<PXFieldOptions.CommitChanges>;
	CapacityRange: PXFieldState<PXFieldOptions.CommitChanges>;
	FromDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ToDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	mergeToolbarWith: 'ScreenToolbar',
})
export class CapacityDetail extends PXView {
	MachID: PXFieldState;
	@linkCommand("ViewSchedule") SchdDate: PXFieldState;
	TotalBlocks: PXFieldState;
	PlanBlocks: PXFieldState;
	SchdBlocks: PXFieldState;
	AvailableBlocks: PXFieldState;
	PlanUtilizationPct: PXFieldState;
	SchdUtilizationPct: PXFieldState;
	FromDate: PXFieldState;
	ToDate: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.MachineCapacityInq', primaryView: 'CapacityFilter' })
export class AM406000 extends PXScreen {
	// to remove the button from the screen toolbar
	ViewSchedule: PXActionState;

	CapacityFilter = createSingle(CapacityFilter);
	CapacityDetail = createCollection(CapacityDetail);
}
