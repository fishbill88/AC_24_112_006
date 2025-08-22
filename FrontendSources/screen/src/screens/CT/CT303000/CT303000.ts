import { createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, gridConfig, PXFieldOptions } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CT.UsageMaint', primaryView: 'Filter' })
export class CT303000 extends PXScreen {

	Filter = createSingle(UsageFilter);

	// we split DAC into 2 classes to show in different grids
	UnBilled = createCollection(PMTran);
	Billed = createCollection(PMTran2);

}

export class UsageFilter extends PXView {
	ContractID: PXFieldState<PXFieldOptions.CommitChanges>;
	InvFinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ initNewRow: true, syncPosition: true })
export class PMTran extends PXView {
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	BAccountID: PXFieldState;
	LocationID: PXFieldState;
	InventoryID: PXFieldState;
	Description: PXFieldState;
	UOM: PXFieldState;
	BillableQty: PXFieldState;
	CRCase__CaseCD: PXFieldState;
	Date: PXFieldState;
}

@gridConfig({ allowDelete: false, allowInsert: false })
export class PMTran2 extends PXView {
	BAccountID: PXFieldState;
	InventoryID: PXFieldState;
	BilledDate: PXFieldState;
	BranchID: PXFieldState;
	LocationID: PXFieldState;
	ARRefNbr: PXFieldState;
	Description: PXFieldState;
	UOM: PXFieldState;
	BillableQty: PXFieldState;
	StartDate: PXFieldState;
	EndDate: PXFieldState;
	ARTranType: PXFieldState;
	CRCase__CaseCD: PXFieldState;
	Date: PXFieldState;
}
