import {
	PXView,
	PXFieldState,
	graphInfo,
	PXScreen,
	createCollection,
	createSingle,
	gridConfig,
	PXFieldOptions,
	columnConfig,
	PXActionState,
	linkCommand,
	GridPreset,
	GridFilterBarVisibility,
} from "client-controls";

@graphInfo({
	graphType: "PX.DataSync.HubSpot.HSMarketingListMembersMaint",
	primaryView: "Filter",
	bpEventsIndicator: false,
	udfTypeField: "",
})
export class HS205050 extends PXScreen {
	GoToHubSpotList: PXActionState;

	Filter = createSingle(HSMarketingListMembersFilter);
	Records = createCollection(HSMarketingListMember);
}

export class HSMarketingListMembersFilter extends PXView {
	MarketingListID: PXFieldState<PXFieldOptions.CommitChanges>;
	HubSpotListName: PXFieldState<PXFieldOptions.Disabled>;
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	SubscribedOnly: PXFieldState<PXFieldOptions.CommitChanges>;
	DeleteInTarget: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	showFilterBar: GridFilterBarVisibility.OnDemand,
	allowUpdate: false,
	autoAdjustColumns: true,
	quickFilterFields: [
		"DisplayName",
		"RemoteName",
		"LastErrorMessageSimplified",
	],
})
export class HSMarketingListMember extends PXView {
	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;

	@columnConfig({ allowCheckAll: true, width: 35 }) Selected: PXFieldState;
	MarketingListMemberID: PXFieldState<PXFieldOptions.Hidden>;
	IsSubscribed: PXFieldState;
	HSSyncRecord__EntityType: PXFieldState;
	@linkCommand("Contact_ViewDetails") LocalID: PXFieldState;

	@linkCommand("GoToHubSpotContact")
	RemoteName: PXFieldState<PXFieldOptions.CommitChanges>;

	HSSyncRecord__SyncStatus: PXFieldState;
	MembershipSyncStatus: PXFieldState;
}
