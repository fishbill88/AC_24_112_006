import {
	PO301000
} from '../PO301000';

import {
	PXView,
	createCollection,
	PXFieldState,
	PXFieldOptions,
	featureInstalled,
	createSingle,
	gridConfig,
	columnConfig
} from 'client-controls';

export interface PO301000_AddProjectItems extends PO301000 { }
export class PO301000_AddProjectItems {
	ProjectItemFilter = createSingle(ProjectItemFilter);
	AvailableProjectItems = createCollection(AvailableProjectItems);
}

export class ProjectItemFilter extends PXView {
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class AvailableProjectItems extends PXView {
	@columnConfig({
		allowCheckAll: true
	})
	Selected: PXFieldState;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitRate: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisedQty: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRevisedAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	ChangeOrderQty: PXFieldState;
	CuryChangeOrderAmount: PXFieldState;
	CommittedQty: PXFieldState;
	CuryCommittedAmount: PXFieldState;
	CommittedReceivedQty: PXFieldState;
	CommittedInvoicedQty: PXFieldState;
	CuryCommittedInvoicedAmount: PXFieldState;
	CommittedOpenQty: PXFieldState;
	CuryCommittedOpenAmount: PXFieldState;
	ActualQty: PXFieldState;
	CuryActualAmount: PXFieldState;
	CuryActualPlusOpenCommittedAmount: PXFieldState;
	CuryVarianceAmount: PXFieldState;
	Performance: PXFieldState;
	IsProduction: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryLastCostToComplete: PXFieldState;
	CuryCostToComplete: PXFieldState;
	LastPercentCompleted: PXFieldState;
	PercentCompleted: PXFieldState;
	CuryLastCostAtCompletion: PXFieldState;
	CuryCostAtCompletion: PXFieldState;
	RevenueTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevenueInventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
}
