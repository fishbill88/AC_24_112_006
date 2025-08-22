import { CR304000 } from "../CR304000";
import {
	PXView,
	PXFieldState,
	linkCommand,
	createCollection,
	PXFieldOptions,
	gridConfig,
	viewInfo,
	createSingle,
} from "client-controls";

export interface CR304000_QuickEstimate extends CR304000 {}
export class CR304000_QuickEstimate {
	@viewInfo({ containerName: "Quick Estimate" })
	SelectedEstimateRecord = createSingle(AMEstimateItem2);
}

export class AMEstimateItem2 extends PXView {
	EstimateID: PXFieldState;
	RevisionID: PXFieldState;
	InventoryCD: PXFieldState;
	IsNonInventory: PXFieldState;
	SubItemID: PXFieldState;
	SiteID: PXFieldState;
	ItemDesc: PXFieldState;
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
	ExtCostDisplay: PXFieldState;
	ReferenceMaterialCost: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderQty: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState;
	CuryUnitCost: PXFieldState<PXFieldOptions.CommitChanges>;
	MarkupPct: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	PriceOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryExtPrice: PXFieldState<PXFieldOptions.CommitChanges>;
}
