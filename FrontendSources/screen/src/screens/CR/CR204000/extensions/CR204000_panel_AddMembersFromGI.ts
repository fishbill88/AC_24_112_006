import { CR204000 } from "../CR204000";
import { createSingle, PXView, PXFieldState, PXFieldOptions } from "client-controls";

export interface CR204000_panel_AddMembersFromGI extends CR204000 {}
export class CR204000_panel_AddMembersFromGI {
	AddMembersFromGIFilterView = createSingle(AddMembersFromGIFilter);
}

export class AddMembersFromGIFilter extends PXView {
	GIDesignID: PXFieldState<PXFieldOptions.CommitChanges>;
	SharedGIFilter: PXFieldState<PXFieldOptions.CommitChanges>;
}