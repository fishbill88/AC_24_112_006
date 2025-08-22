import { CR303000 } from "../CR303000";
import {
	PXView,
	PXFieldState,
	linkCommand,
	createCollection,
	PXFieldOptions,
	gridConfig,
	PXActionState,
	viewInfo,
} from "client-controls";

export interface CR303000_MarketingLists extends CR303000 {}
export class CR303000_MarketingLists {
	@viewInfo({ containerName: "Marketing Lists" })
	Subscriptions = createCollection(CRMarketingListMember);
}

@gridConfig({
	syncPosition: true,
	allowInsert: false,
	allowDelete: false,
	fastFilterByAllFields: false,
})
export class CRMarketingListMember extends PXView {
	SubscribeAll: PXActionState;
	UnsubscribeAll: PXActionState;

	IsSubscribed: PXFieldState;

	@linkCommand("ViewMarketingList") CRMarketingList__MailListCode: PXFieldState;
	CRMarketingList__Name: PXFieldState;
	CRMarketingList__Type: PXFieldState;
	CRMarketingList__Status: PXFieldState<PXFieldOptions.Hidden>;
	CRMarketingList__WorkgroupID: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("OwnerID_ViewDetails")
	CRMarketingList__OwnerID: PXFieldState<PXFieldOptions.Hidden>;
	CRMarketingList__Method: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("GIDesignID_ViewDetails")
	CRMarketingList__GIDesignID: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("CreatedByID_ViewDetails")
	CRMarketingList__CreatedByID: PXFieldState<PXFieldOptions.Hidden>;
	CRMarketingList__CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("LastModifiedByID_ViewDetails")
	CRMarketingList__LastModifiedByID: PXFieldState<PXFieldOptions.Hidden>;
	CRMarketingList__LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}
