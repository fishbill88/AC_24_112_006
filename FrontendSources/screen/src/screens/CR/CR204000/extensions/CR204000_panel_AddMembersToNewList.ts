import { CR204000 } from "../CR204000";
import { createSingle, PXView, PXFieldState, PXFieldOptions, gridConfig, createCollection, GridFilterBarVisibility } from "client-controls";

export interface CR204000_panel_AddMembersToNewList extends CR204000 {}
export class CR204000_panel_AddMembersToNewList {
	AddMembersToNewListFilterView = createSingle(AddMembersToNewListFilter);
	AddMembersToNewListFilterUdfView = createCollection(FieldValue);
}

export class AddMembersToNewListFilter extends PXView {
	MailListCode: PXFieldState<PXFieldOptions.CommitChanges>;
	Name: PXFieldState;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({initNewRow: true, allowDelete: false, allowInsert: false, allowUpdate: false, showFilterBar: GridFilterBarVisibility.False})
export class FieldValue extends PXView {
	DisplayName: PXFieldState;
	Value: PXFieldState<PXFieldOptions.CommitChanges>;
}
