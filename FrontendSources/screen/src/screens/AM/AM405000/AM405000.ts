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
	WcID: PXFieldState<PXFieldOptions.CommitChanges>;
	CapacityRange: PXFieldState<PXFieldOptions.CommitChanges>;
	FromDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ToDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
})
export class CapacityDetail extends PXView {
	WcID: PXFieldState;
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

@graphInfo({ graphType: 'PX.Objects.AM.WorkCenterCapacityInq', primaryView: 'CapacityFilter' })
export class AM405000 extends PXScreen {
	// to remove the button from the screen toolbar
	ViewSchedule: PXActionState;

	CapacityFilter = createSingle(CapacityFilter);
	CapacityDetail = createCollection(CapacityDetail);
}
