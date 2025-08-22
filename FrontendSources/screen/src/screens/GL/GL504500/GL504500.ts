import {
	PXScreen, createSingle, createCollection, graphInfo, PXView, PXFieldState,
	PXFieldOptions, linkCommand, columnConfig, PXActionState, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.GL.AllocationProcess', primaryView: 'Filter' })
export class GL504500 extends PXScreen {

	Filter = createSingle(AllocationFilter);
	Allocations = createCollection(AllocationExt);

	// this action is from Allocations. I define it here to prevent showing it in grid toolbar. It's platform bug for the moment.
	viewBatch: PXActionState;
	EditDetail: PXActionState;

}

export class AllocationFilter extends PXView {

	DateEntered: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({mergeToolbarWith: 'ScreenToolbar'})
export class AllocationExt extends PXView {

	@columnConfig({ allowCheckAll: true, allowSort: false })
	Selected: PXFieldState;

	BranchID: PXFieldState;

	@linkCommand("EditDetail")
	GLAllocationID: PXFieldState;

	Descr: PXFieldState;
	AllocMethod: PXFieldState;
	AllocLedgerID: PXFieldState;
	SortOrder: PXFieldState;

	@linkCommand("viewBatch")
	BatchNbr: PXFieldState;

	BatchPeriod: PXFieldState;
	ControlTotal: PXFieldState;
	Status: PXFieldState;

}
