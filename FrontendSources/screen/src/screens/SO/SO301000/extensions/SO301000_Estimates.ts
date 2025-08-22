import {
	SO301000
} from '../SO301000';

import {
	PXView,
	createCollection,
	createSingle,
	PXFieldState,
	PXFieldOptions,
	viewInfo,
	columnConfig,
	linkCommand,
	PXActionState,
	gridConfig
} from 'client-controls';


export interface SO301000_Estimates extends SO301000 {}
export class SO301000_Estimates {
	@viewInfo({containerName: "Estimates"})
	OrderEstimateRecords = createCollection(AMEstimateItem);

	OrderEstimateItemFilter = createSingle(OrderEstimateItemFilter);
	SelectedEstimateRecord = createSingle(SelectedEstimateRecord);
}

@gridConfig({
	syncPosition: true,
	adjustPageSize: true,
	wrapToolbar: true,
	allowInsert: false,
	allowDelete: false
})
export class AMEstimateItem extends PXView {
	AddEstimate: PXActionState;
	QuickEstimate: PXActionState;
	RemoveEstimate: PXActionState;

	@columnConfig({hideViewLink: true}) AMEstimateItem__BranchID: PXFieldState;
	@columnConfig({hideViewLink: true}) AMEstimateItem__InventoryCD: PXFieldState;
	AMEstimateItem__ItemDesc: PXFieldState;
	@columnConfig({hideViewLink: true}) AMEstimateItem__SubItemID: PXFieldState;
	@columnConfig({hideViewLink: true}) AMEstimateItem__SiteID: PXFieldState;
	@columnConfig({hideViewLink: true}) AMEstimateItem__UOM: PXFieldState;
	OrderQty: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitPrice: PXFieldState;
	CuryExtPrice: PXFieldState;
	@linkCommand("ViewEstimate") EstimateID: PXFieldState;
	@columnConfig({hideViewLink: true}) RevisionID: PXFieldState;
	@columnConfig({hideViewLink: true}) TaxCategoryID: PXFieldState;
	@columnConfig({hideViewLink: true}) AMEstimateItem__OwnerID: PXFieldState;
	@columnConfig({hideViewLink: true}) AMEstimateItem__EngineerID: PXFieldState;
	AMEstimateItem__RequestDate: PXFieldState;
	AMEstimateItem__PromiseDate: PXFieldState;
	@columnConfig({hideViewLink: true}) AMEstimateItem__EstimateClassID: PXFieldState;
}

export class OrderEstimateItemFilter extends PXView {
	EstimateID: PXFieldState<PXFieldOptions.CommitChanges>;
	AddExisting: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisionID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryCD: PXFieldState;
	IsNonInventory: PXFieldState;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemDesc: PXFieldState;
	EstimateClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class SelectedEstimateRecord extends PXView {
	EstimateID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisionID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryCD: PXFieldState<PXFieldOptions.CommitChanges>;
	IsNonInventory: PXFieldState;
	SubItemID: PXFieldState;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemDesc: PXFieldState<PXFieldOptions.CommitChanges>;
	EstimateClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	FixedLaborCost: PXFieldState<PXFieldOptions.CommitChanges>;
	FixedLaborOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	VariableLaborCost: PXFieldState<PXFieldOptions.CommitChanges>;
	VariableLaborOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	MachineCost: PXFieldState<PXFieldOptions.CommitChanges>;
	MachineOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	MaterialCost: PXFieldState<PXFieldOptions.CommitChanges>;
	MaterialOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	ToolCost: PXFieldState<PXFieldOptions.CommitChanges>;
	ToolOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	FixedOverheadCost: PXFieldState<PXFieldOptions.CommitChanges>;
	FixedOverheadOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	VariableOverheadCost: PXFieldState<PXFieldOptions.CommitChanges>;
	VariableOverheadOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	SubcontractCost: PXFieldState<PXFieldOptions.CommitChanges>;
	SubcontractOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	ExtCostDisplay: PXFieldState<PXFieldOptions.CommitChanges>;
	ReferenceMaterialCost: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderQty: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitCost: PXFieldState<PXFieldOptions.CommitChanges>;
	MarkupPct: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	PriceOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryExtPrice: PXFieldState<PXFieldOptions.CommitChanges>;
}

