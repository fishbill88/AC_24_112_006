import {
	gridConfig,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true
})
export class Items extends PXView {
	ClassID: PXFieldState;
	Description: PXFieldState;
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
}
