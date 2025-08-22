import {
	PXScreen, createCollection, graphInfo, PXView, createSingle, PXFieldState, PXFieldOptions, columnConfig, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.GL.AllocationMaint', primaryView: 'AllocationHeader', showUDFIndicator: true })
export class GL204500 extends PXScreen {

	AllocationHeader = createSingle(GLAllocation);
	Allocation = createSingle(GLAllocation);

	Batches = createCollection(Batch);

	Destination = createCollection(GLAllocationDestination);
	Source = createCollection(GLAllocationSource);

}

export class GLAllocation extends PXView {

	GLAllocationID: PXFieldState;
	Descr: PXFieldState;

	@columnConfig({})
	Active: PXFieldState;

	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;

	StartFinPeriodID: PXFieldState;
	EndFinPeriodID: PXFieldState;

	Recurring: PXFieldState;

	@columnConfig({})
	DataField: PXFieldState;

	AllocCollectMethod: PXFieldState;
	AllocMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	AllocLedgerID: PXFieldState<PXFieldOptions.CommitChanges>;
	SourceLedgerID: PXFieldState<PXFieldOptions.CommitChanges>;
	BasisLederID: PXFieldState<PXFieldOptions.CommitChanges>;
	AllocateSeparately: PXFieldState<PXFieldOptions.CommitChanges>;
	SortOrder: PXFieldState;
	LastRevisionOn: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ allowInsert: false, allowDelete: false })
export class Batch extends PXView {

	Module: PXFieldState;
	BatchNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	LedgerID: PXFieldState;

	DateEntered: PXFieldState;

	@columnConfig({ hideViewLink: true })
	FinPeriodID: PXFieldState;

	Status: PXFieldState;
	CuryControlTotal: PXFieldState;

}

export class GLAllocationDestination extends PXView {

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountCD: PXFieldState;

	SubCD: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BasisBranchID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BasisAccountCD: PXFieldState;

	BasisSubCD: PXFieldState;
	Weight: PXFieldState;

}

export class GLAllocationSource extends PXView {

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountCD: PXFieldState<PXFieldOptions.CommitChanges>;

	SubCD: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ContrAccountID: PXFieldState<PXFieldOptions.CommitChanges>;

	ContrSubID: PXFieldState;
	LimitAmount: PXFieldState;
	LimitPercent: PXFieldState;

}
