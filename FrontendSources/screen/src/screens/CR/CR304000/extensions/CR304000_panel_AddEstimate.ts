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
	columnConfig,
} from "client-controls";

export interface CR304000_AddEstimate extends CR304000 {}
export class CR304000_AddEstimate {
	@viewInfo({ containerName: "Add Estimate" })
	OrderEstimateItemFilter = createSingle(AMEstimateItem);
}

export class AMEstimateItem extends PXView {
	EstimateID: PXFieldState<PXFieldOptions.CommitChanges>;
	AddExisting: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisionID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryCD: PXFieldState<PXFieldOptions.CommitChanges>;
	IsNonInventory: PXFieldState;
	SubItemID: PXFieldState;
	SiteID: PXFieldState;
	ItemDesc: PXFieldState<PXFieldOptions.CommitChanges>;
	EstimateClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
}
