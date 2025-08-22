import { CR204000 } from "../CR204000";
import { createCollection, PXView, PXFieldState, gridConfig, columnConfig, linkCommand, GridPreset } from "client-controls";

export interface CR204000_panel_AddMembersFromCampaigns extends CR204000 {}
export class CR204000_panel_AddMembersFromCampaigns {
	AddMembersFromCampaignsFilterView = createCollection(CRCampaign);
}

@gridConfig({
	preset: GridPreset.Details,
	quickFilterFields: ["CampaignID", "CampaignName", "PromoCodeID", "OwnerID"],
	allowDelete: false,
	allowImport: false,
	allowInsert: false
})
export class CRCampaign extends PXView {
	@columnConfig({allowCheckAll: true})
	Selected: PXFieldState;
	@linkCommand('AddMembersFromCampaignsFilterView_ViewDetails')
	CampaignID: PXFieldState;
	CampaignName: PXFieldState;
	CampaignType: PXFieldState;
	Status: PXFieldState;
	StartDate: PXFieldState;
	EndDate: PXFieldState;
	PromoCodeID: PXFieldState;
	CreatedDateTime: PXFieldState;
	LastModifiedDateTime: PXFieldState;
	OwnerID: PXFieldState;
	OwnerID_description: PXFieldState;
}
