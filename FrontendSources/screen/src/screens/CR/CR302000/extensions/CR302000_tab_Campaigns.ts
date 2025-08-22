import { CR302000 } from "../CR302000";
import {
	PXView,
	PXFieldState,
	linkCommand,
	createCollection,
	PXFieldOptions,
	gridConfig,
	viewInfo,
	columnConfig,
	GridColumnDisplayMode,
	GridPreset,
} from "client-controls";

export interface CR302000_Campaigns extends CR302000 {}
export class CR302000_Campaigns {
	@viewInfo({ containerName: "Campaigns" })
	Members = createCollection(CRCampaignMembers);
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	fastFilterByAllFields: false,
})
export class CRCampaignMembers extends PXView {
	CampaignID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CRCampaign__CampaignName: PXFieldState;
	@linkCommand("MarketingListID_ViewDetails")
	CRMarketingList__MailListCode: PXFieldState<PXFieldOptions.Hidden>;
	CRCampaign__Status: PXFieldState;
	CRCampaign__StartDate: PXFieldState;
	CRCampaign__EndDate: PXFieldState;
	CRCampaign__PromoCodeID: PXFieldState;
	CRCampaign__OwnerID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ displayMode: GridColumnDisplayMode.Both })
	CRCampaign__CampaignType: PXFieldState<PXFieldOptions.Hidden>;
	CRCampaignMembers__CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("CreatedByID_ViewDetails")
	CRCampaign__CreatedByID: PXFieldState<PXFieldOptions.Hidden>;
	CRCampaign__CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("LastModifiedByID_ViewDetails")
	CRCampaign__LastModifiedByID: PXFieldState<PXFieldOptions.Hidden>;
	CRCampaign__LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}
