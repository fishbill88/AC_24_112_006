import { CR204000 } from "../CR204000";
import { gridConfig, PXView, PXFieldState, columnConfig, linkCommand, createCollection, GridPreset } from "client-controls";

export interface CR204000_panel_AddMembersFromMarketingList extends CR204000 {}
export class CR204000_panel_AddMembersFromMarketingList {
	AddMembersFromMarketingListsFilterView = createCollection(CRMarketingListAlias);
}

@gridConfig({
	preset: GridPreset.Details,
	quickFilterFields: ["MailListCode", "Name", "OwnerID"],
	allowDelete: false,
	allowInsert: false,
	allowImport: false
})
export class CRMarketingListAlias extends PXView {
	@columnConfig({allowCheckAll: true})
	Selected: PXFieldState;
	@linkCommand('AddMembersFromMarketingListsFilterView_ViewDetails')
	MailListCode: PXFieldState;
	Name: PXFieldState;
	Status: PXFieldState;
	OwnerID: PXFieldState;
	OwnerID_description: PXFieldState;
	CreatedDateTime: PXFieldState;
	LastModifiedDateTime: PXFieldState;
}