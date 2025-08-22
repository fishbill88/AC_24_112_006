import { CR204000 } from "../CR204000";
import { createSingle, PXView, PXFieldState, PXFieldOptions } from "client-controls";

export interface CR204000_panel_CopyMembers extends CR204000 {}
export class CR204000_panel_CopyMembers {
	CopyMembersFilterView = createSingle(CopyMembersFilter);
}

export class CopyMembersFilter extends PXView {
	AddMembersOption: PXFieldState<PXFieldOptions.CommitChanges>;
}