import { PXView,PXFieldState,PXFieldOptions } from 'client-controls';

// Views

export class UnitOfMeasure extends PXView  {
	Unit: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
}

export class INUnit extends PXView  {
	ToUnit: PXFieldState<PXFieldOptions.CommitChanges>;
	UnitMultDiv: PXFieldState;
	UnitRate: PXFieldState;
}
