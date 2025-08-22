import { CR204000 } from "../CR204000";
import { columnConfig, PXView, PXFieldState, linkCommand, gridConfig, createCollection, GridPreset } from "client-controls";

export interface CR204000_panel_AddMembersToExistingList extends CR204000 {}
export class CR204000_panel_AddMembersToExistingList {
	AddMembersToExistingListsFilterView = createCollection(CRMarketingListAlias);
}

@gridConfig({
	preset: GridPreset.Details,
	quickFilterFields: ["MailListCode", "Name", "OwnerID"],
	allowInsert: false,
	allowDelete: false,
	allowImport: false,
	adjustPageSize: true
}) 
export class CRMarketingListAlias extends PXView {
	@columnConfig({allowCheckAll: true})
	Selected: PXFieldState;
	@linkCommand('AddMembersToExistingListsFilterView_ViewDetails')
	MailListCode: PXFieldState;
	Name: PXFieldState;
	Status: PXFieldState;
	OwnerID: PXFieldState;
	OwnerID_description: PXFieldState;
	CreatedDateTime: PXFieldState;
	LastModifiedDateTime: PXFieldState;
}