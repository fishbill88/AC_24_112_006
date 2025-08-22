import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, GridColumnShowHideMode,
	PXView, PXFieldState, PXFieldOptions, gridConfig, columnConfig, linkCommand, GridPreset }
	from 'client-controls';

@graphInfo({graphType: 'PX.Objects.CR.CRMarketingListMaint', primaryView: 'MailLists', showUDFIndicator: true })
export class CR204000 extends PXScreen {
	MailLists = createSingle(CRMarketingList);
	ListMembers = createCollection(CRMarketingListMember);
	MarketingCampaigns = createCollection(CRCampaignToCRMarketingListLink);
	Activities = createCollection(CRActivity);
	HubSpotSyncRecs = createCollection(HSSyncRecord);
}

export class CRMarketingList extends PXView {
	MailListCode: PXFieldState;
	Name: PXFieldState;
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
	GIDesignID: PXFieldState<PXFieldOptions.CommitChanges>;
	SharedGIFilter: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	allowInsert: true,
	allowDelete: false,
	allowImport: true,
	topBarItems: {
		DeleteSelectedMembers: {
			config: {
				commandName: "DeleteSelectedMembers",
				images: { normal: "main@RecordDel" }
			}
		},
		AddMembersMenu: {
			type: "menu-options",
			config: {
				text: "Add Members",
				options: {
					AddMembersFromGI: { text: "Add from Generic Inquiry", commandName: "AddMembersFromGI" },
					AddMembersFromMarketingLists: { text: "Add from Marketing Lists", commandName: "AddMembersFromMarketingLists" },
					AddMembersFromCampaigns: { text: "Add from Campaigns", commandName: "AddMembersFromCampaigns" },
				}
			}
		},
		ManageSubscriptionMenu: {
			type: "menu-options",
			config: {
				text: "Manage Subscription",
				options: {
					UnsubscribeAll: { text: "Unsubscribe All", commandName: "UnsubscribeAll" },
					SubscribeAll: { text: "Subscribe All", commandName: "SubscribeAll" },
				}
			}
		},
		CopyMembers: {
			config: {
				commandName: "CopyMembers",
				text: "Copy All"
			}
		},
		ClearMembers: {
			config: {
				commandName: "ClearMembers",
				text: "Clear All"
			}
		},
	},
})
export class CRMarketingListMember extends PXView {
	DeleteSelectedMembers: PXActionState;
	CopyMembers: PXActionState;
	AddMembersMenu: PXActionState;
	ManageSubscriptionMenu: PXActionState;
	ClearMembers: PXActionState;
	AddMembersFromGI: PXActionState;
	AddMembersFromMarketingLists: PXActionState;
	AddMembersFromCampaigns: PXActionState;
	UnsubscribeAll: PXActionState;
	SubscribeAll: PXActionState;

	IsSubscribed: PXFieldState;
	@linkCommand('ListMembers_Contact_ViewDetails')
	@columnConfig({ textField: "Contact__DisplayName", width: 280, })
	ContactID: PXFieldState;
	Contact__ContactType: PXFieldState;
	Contact__Salutation: PXFieldState;
	@linkCommand('ListMembers_BAccount_ViewDetails')
	Contact__BAccountID: PXFieldState;
	Contact__FullName: PXFieldState;
	Contact__Email: PXFieldState;
	CreatedDateTime: PXFieldState;

	LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({allowCheckAll: true})
	Selected: PXFieldState<PXFieldOptions.Hidden>;
	Contact__IsActive: PXFieldState<PXFieldOptions.Hidden>;
	Contact__ClassID: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Phone1: PXFieldState<PXFieldOptions.Hidden>;
	Contact__LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	Contact__CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Source: PXFieldState<PXFieldOptions.Hidden>;
	Contact__AssignDate: PXFieldState<PXFieldOptions.Hidden>;
	Contact__DuplicateStatus: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Phone2: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Phone3: PXFieldState<PXFieldOptions.Hidden>;
	Contact__DateOfBirth: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Fax: PXFieldState<PXFieldOptions.Hidden>;
	Contact__WebSite: PXFieldState<PXFieldOptions.Hidden>;
	Contact__ConsentAgreement: PXFieldState<PXFieldOptions.Hidden>;
	Contact__ConsentDate: PXFieldState<PXFieldOptions.Hidden>;
	Contact__ConsentExpirationDate: PXFieldState<PXFieldOptions.Hidden>;
	Contact__ParentBAccountID: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Gender: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Method: PXFieldState<PXFieldOptions.Hidden>;
	Contact__NoCall: PXFieldState<PXFieldOptions.Hidden>;
	Contact__NoEMail: PXFieldState<PXFieldOptions.Hidden>;
	Contact__NoFax: PXFieldState<PXFieldOptions.Hidden>;
	Contact__NoMail: PXFieldState<PXFieldOptions.Hidden>;
	Contact__NoMarketing: PXFieldState<PXFieldOptions.Hidden>;
	Contact__NoMassMail: PXFieldState<PXFieldOptions.Hidden>;
	Contact__CampaignID: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Phone1Type: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Phone2Type: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Phone3Type: PXFieldState<PXFieldOptions.Hidden>;
	Contact__FaxType: PXFieldState<PXFieldOptions.Hidden>;
	Contact__MaritalStatus: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Spouse: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Status: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Resolution: PXFieldState<PXFieldOptions.Hidden>;
	Contact__LanguageID: PXFieldState<PXFieldOptions.Hidden>;
	Contact__OwnerID: PXFieldState<PXFieldOptions.Hidden>;
	Contact__OwnerID_description: PXFieldState<PXFieldOptions.Hidden>;
	Address__AddressLine1: PXFieldState<PXFieldOptions.Hidden>;
	Address__AddressLine2: PXFieldState<PXFieldOptions.Hidden>;
	Address__City: PXFieldState<PXFieldOptions.Hidden>;
	Address__State: PXFieldState<PXFieldOptions.Hidden>;
	Address__PostalCode: PXFieldState<PXFieldOptions.Hidden>;
	Address__CountryID: PXFieldState<PXFieldOptions.Hidden>;
	BAccount__ClassID: PXFieldState<PXFieldOptions.Hidden>;
	BAccount__WorkgroupID: PXFieldState<PXFieldOptions.Hidden>;
	BAccount__OwnerID: PXFieldState<PXFieldOptions.Hidden>;
	BAccount__ParentBAccountID: PXFieldState<PXFieldOptions.Hidden>;
	BAccount__CampaignSourceID: PXFieldState<PXFieldOptions.Hidden>;
	Address2__AddressLine1: PXFieldState<PXFieldOptions.Hidden>;
	Address2__AddressLine2: PXFieldState<PXFieldOptions.Hidden>;
	Address2__City: PXFieldState<PXFieldOptions.Hidden>;
	Address2__State: PXFieldState<PXFieldOptions.Hidden>;
	Address2__PostalCode: PXFieldState<PXFieldOptions.Hidden>;
	Address2__CountryID: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	allowDelete: false,
	allowInsert: false,
	fastFilterByAllFields: false
})
export class CRCampaignToCRMarketingListLink extends PXView {
	@linkCommand('MarketingCampaigns_CRCampaign_ViewDetails')
	CampaignID: PXFieldState;
	CRCampaign__CampaignName: PXFieldState;
	CRCampaign__Status: PXFieldState<PXFieldOptions.Hidden>;
	CRCampaign__StartDate: PXFieldState;
	CRCampaign__EndDate: PXFieldState;
	CRCampaign__PromoCodeID: PXFieldState;
	@linkCommand('MarketingCampaigns_OwnerID_ViewDetails')
	CRCampaign__OwnerID: PXFieldState;
	LastUpdateDate: PXFieldState;
	@linkCommand('CreatedByID_ViewDetails')
	CRCampaign__CreatedByID: PXFieldState<PXFieldOptions.Hidden>;
	CRCampaign__CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand('LastModifiedByID_ViewDetails')
	CRCampaign__LastModifiedByID: PXFieldState<PXFieldOptions.Hidden>;
	CRCampaign__LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
	adjustPageSize: true
})
export class HSSyncRecord extends PXView {
	SyncHubSpot: PXActionState;
	PushToHubSpot: PXActionState;
	PullFromHubSpot: PXActionState;

	SYProvider__Name: PXFieldState;
	@linkCommand('GoToHubSpot')
	RemoteID: PXFieldState<PXFieldOptions.CommitChanges>;
	SyncStatus: PXFieldState;
	LastSource: PXFieldState;
	LastOperation: PXFieldState;
	LastErrorMessageSimplified: PXFieldState;
	LastAttemptTS: PXFieldState;
	RemoteTS: PXFieldState;
	AttemptCount: PXFieldState;
	HSEntitySetup__ImportScenario: PXFieldState;
	HSEntitySetup__ExportScenario: PXFieldState;
	SyncRecordID: PXFieldState<PXFieldOptions.Hidden>;
	LastErrorMessage: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
})
export class CRActivity extends PXView {
	IsPinned: PXFieldState;
	IsCompleteIcon: PXFieldState;
	PriorityIcon: PXFieldState;
	CRReminder__ReminderIcon: PXFieldState;
	ClassIcon: PXFieldState;
	ClassInfo: PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.False})
	RefNoteID: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand('ViewActivity')
	Subject: PXFieldState;
	UIStatus: PXFieldState;
	Released: PXFieldState;
	StartDate: PXFieldState;
	CreatedDateTime: PXFieldState;
	TimeSpent: PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.False})
	CreatedByID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;
	WorkgroupID: PXFieldState;
	@linkCommand('OpenActivityOwner')
	OwnerID: PXFieldState;
	BAccountID: PXFieldState<PXFieldOptions.Hidden>;
	ContactID: PXFieldState<PXFieldOptions.Hidden>;
	ProjectID: PXFieldState<PXFieldOptions.Hidden>;
	ProjectTaskID: PXFieldState<PXFieldOptions.Hidden>;
	Body: PXFieldState<PXFieldOptions.Hidden>;
}
