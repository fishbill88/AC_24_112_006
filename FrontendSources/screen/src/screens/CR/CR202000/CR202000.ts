import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	columnConfig,
	linkCommand,
	GridColumnShowHideMode,
	GridPreset,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CR.CampaignMaint",
	primaryView: "Campaign",
	udfTypeField: "CampaignType",
	showUDFIndicator: true
})
export class CR202000 extends PXScreen {
	Campaign = createSingle(CRCampaign);
	CalcCampaignCurrent = createSingle(CRCampaignStandalone);
	Activities = createCollection(CRActivity);
	CampaignMarketingLists = createCollection(CRMarketingList);
	CampaignMembers = createCollection(CRCampaignMembers);
	Leads = createCollection(CRLead);
	Opportunities = createCollection(CROpportunity);
	Answers = createCollection(CSAnswers);
}

export class CRCampaign extends PXView {
	CampaignID: PXFieldState;
	CampaignType: PXFieldState<PXFieldOptions.CommitChanges>;
	CampaignName: PXFieldState;
	Status: PXFieldState;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState;
	EndDate: PXFieldState;
	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpectedResponse: PXFieldState;
	PlannedBudget: PXFieldState;
	ExpectedRevenue: PXFieldState;
	PromoCodeID: PXFieldState;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
}

export class CRCampaignStandalone extends PXView {
	Contacts: PXFieldState;
	MembersContacted: PXFieldState<PXFieldOptions.Disabled>;
	MembersResponded: PXFieldState<PXFieldOptions.Disabled>;
	LeadsGenerated: PXFieldState<PXFieldOptions.Disabled>;
	LeadsConverted: PXFieldState<PXFieldOptions.Disabled>;
	Opportunities: PXFieldState<PXFieldOptions.Disabled>;
	ClosedOpportunities: PXFieldState<PXFieldOptions.Disabled>;
	OpportunitiesValue: PXFieldState;
	ClosedOpportunitiesValue: PXFieldState;
}

@gridConfig({ allowDelete: false, allowInsert: false, allowUpdate: false })
export class CRActivity extends PXView {
	NewTask: PXActionState;
	NewEvent: PXActionState;
	NewMailActivity: PXActionState;
	NewActivity: PXActionState;

	IsPinned: PXFieldState;
	IsCompleteIcon: PXFieldState;
	PriorityIcon: PXFieldState;
	CRReminder__ReminderIcon: PXFieldState;
	ClassIcon: PXFieldState;
	ClassInfo: PXFieldState;
	@linkCommand("ViewActivity")
	Subject: PXFieldState;
	UIStatus: PXFieldState;
	Released: PXFieldState;
	StartDate: PXFieldState;
	CreatedDateTime: PXFieldState;
	TimeSpent: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	CreatedByID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;
	WorkgroupID: PXFieldState;
	@linkCommand("OpenActivityOwner")
	OwnerID: PXFieldState;
	Source: PXFieldState<PXFieldOptions.Hidden>;
	BAccountID: PXFieldState<PXFieldOptions.Hidden>;
	ContactID: PXFieldState<PXFieldOptions.Hidden>;
	ProjectID: PXFieldState<PXFieldOptions.Hidden>;
	ProjectTaskID: PXFieldState<PXFieldOptions.Hidden>;
	Body: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: true,
	adjustPageSize: true,
	suppressNoteFiles: true,
	fastFilterByAllFields: false,
})
export class CRMarketingList extends PXView {
	SelectedForCampaign: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("CampaignMarketingLists_CRMarketingList_ViewDetails")
	MailListCode: PXFieldState;
	Name: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OwnerID: PXFieldState;
	LastUpdateDate: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Hidden>;
	WorkgroupID: PXFieldState<PXFieldOptions.Hidden>;
	Method: PXFieldState<PXFieldOptions.Hidden>;
	Type: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("GIDesignID_ViewDetails")
	GIDesignID: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("CreatedByID_ViewDetails")
	CreatedByID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("GIDesignID_ViewDetails")
	LastModifiedByID: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Details,
	allowImport: true,
	allowUpdate: false,
	adjustPageSize: true,
})
export class CRCampaignMembers extends PXView {
	UpdateListMembers: PXActionState;
	NewCampaignMemberActivity: PXActionState;
	ClearMembers: PXActionState;

	@columnConfig({ allowUpdate: false, allowCheckAll: true })
	Selected: PXFieldState;
	@linkCommand("Contact_ViewDetails")
	@columnConfig({ textField: "Contact__DisplayName", width: 280, })
	ContactID: PXFieldState<PXFieldOptions.CommitChanges>;
	Contact__ContactType: PXFieldState;
	@linkCommand("CampaignMembers_CRMarketingList_ViewDetails")
	CRMarketingList__MailListCode: PXFieldState;
	@linkCommand("CampaignMembers_BAccount_ViewDetails")
	Contact__BAccountID: PXFieldState;
	Contact__FullName: PXFieldState;
	Contact__EMail: PXFieldState;
	Contact__Phone1: PXFieldState;
	OpportunityCreatedCount: PXFieldState;
	ActivitiesLogged: PXFieldState;

	Contact__Salutation: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Phone1Type: PXFieldState<PXFieldOptions.Hidden>;
	Contact__IsActive: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({
		allowUpdate: false,
		allowShowHide: GridColumnShowHideMode.False,
	})
	CampaignID: PXFieldState<PXFieldOptions.Hidden>;
	EmailSendCount: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Phone2Type: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Phone2: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Phone3Type: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Phone3: PXFieldState<PXFieldOptions.Hidden>;
	Contact__FaxType: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Fax: PXFieldState<PXFieldOptions.Hidden>;
	Contact__WebSite: PXFieldState<PXFieldOptions.Hidden>;
	Contact__DateOfBirth: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Gender: PXFieldState<PXFieldOptions.Hidden>;
	Contact__MaritalStatus: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Spouse: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("CreatedByID_ViewDetails")
	Contact__CreatedByID: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("LastModifiedByID_ViewDetails")
	Contact__LastModifiedByID: PXFieldState<PXFieldOptions.Hidden>;
	Contact__CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	Contact__LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	Contact__WorkgroupID: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("OwnerID_ViewDetails")
	Contact__OwnerID: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("CampaignMembers_CRContactClass_ViewDetails")
	Contact__ClassID: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Source: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Title: PXFieldState<PXFieldOptions.Hidden>;
	Contact__FirstName: PXFieldState;
	Contact__MidName: PXFieldState;
	Contact__LastName: PXFieldState;
	Address__AddressLine1: PXFieldState<PXFieldOptions.Hidden>;
	Address__AddressLine2: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Status: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	Contact__IsNotEmployee: PXFieldState;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	Address__City: PXFieldState<PXFieldOptions.Hidden>;
	Address__State: PXFieldState<PXFieldOptions.Hidden>;
	Address__PostalCode: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("CountryID_ViewDetails")
	Address__CountryID: PXFieldState<PXFieldOptions.Hidden>;
	Contact__ConsentAgreement: PXFieldState<PXFieldOptions.Hidden>;
	Contact__ConsentDate: PXFieldState<PXFieldOptions.Hidden>;
	Contact__ConsentExpirationDate: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Method: PXFieldState<PXFieldOptions.Hidden>;
	Contact__NoCall: PXFieldState<PXFieldOptions.Hidden>;
	Contact__NoMarketing: PXFieldState<PXFieldOptions.Hidden>;
	Contact__NoEMail: PXFieldState<PXFieldOptions.Hidden>;
	Contact__NoMassMail: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("ParentBAccountID_ViewDetails")
	Contact__ParentBAccountID: PXFieldState<PXFieldOptions.Hidden>;
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
	preset: GridPreset.Inquiry,
	allowUpdate: false,
	adjustPageSize: true,
	autoAdjustColumns: true,
	topBarItems: {
		AddContact: {
			index: 2,
			config: {
				commandName: "AddContact",
				images: { normal: "main@RecordAdd" },
			},
		},
	},
})
export class CRLead extends PXView {
	AddContact: PXActionState;

	@linkCommand("Leads_ViewDetails")
	DisplayName: PXFieldState;
	Status: PXFieldState;
	Resolution: PXFieldState;
	FullName: PXFieldState;
	@columnConfig({ hideViewLink: false })
	OwnerID: PXFieldState;

	@linkCommand("Leads_CRLeadClass_ViewDetails")
	ClassID: PXFieldState<PXFieldOptions.Hidden>;
	Source: PXFieldState<PXFieldOptions.Hidden>;
	WorkgroupID: PXFieldState<PXFieldOptions.Hidden>;
	IsActive: PXFieldState<PXFieldOptions.Hidden>;
	Title: PXFieldState<PXFieldOptions.Hidden>;
	Salutation: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	ContactID: PXFieldState<PXFieldOptions.Hidden>;
	EMail: PXFieldState;
	Address__AddressLine1: PXFieldState<PXFieldOptions.Hidden>;
	Address__AddressLine2: PXFieldState<PXFieldOptions.Hidden>;
	Phone1: PXFieldState<PXFieldOptions.Hidden>;
	Phone2: PXFieldState<PXFieldOptions.Hidden>;
	Phone3: PXFieldState<PXFieldOptions.Hidden>;
	Fax: PXFieldState<PXFieldOptions.Hidden>;
	WebSite: PXFieldState<PXFieldOptions.Hidden>;
	DateOfBirth: PXFieldState<PXFieldOptions.Hidden>;
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedByID_Modifier_Username: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	allowUpdate: false,
	adjustPageSize: true,
	autoAdjustColumns: true,
	topBarItems: {
		AddOpportunity: {
			index: 2,
			config: {
				commandName: "AddOpportunity",
				images: { normal: "main@RecordAdd" },
			},
		},
	},
})
export class CROpportunity extends PXView {
	AddOpportunity: PXActionState;

	@linkCommand("Opportunities_ViewDetails")
	OpportunityID: PXFieldState;
	Subject: PXFieldState;
	Status: PXFieldState;
	StageID: PXFieldState;
	@columnConfig({ hideViewLink: false })
	CuryID: PXFieldState;
	CuryProductsAmount: PXFieldState;
	CloseDate: PXFieldState;
	@columnConfig({ hideViewLink: false })
	OwnerID: PXFieldState;

	CROpportunityProbability__Probability: PXFieldState<PXFieldOptions.Hidden>;
	Resolution: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("Opportunities_CROpportunityClass_ViewDetails")
	ClassID: PXFieldState<PXFieldOptions.Hidden>;
	Source: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("Opportunities_BAccount_ViewDetails")
	BAccountID: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("Opportunities_Contact_ViewDetails")
	ContactID: PXFieldState<PXFieldOptions.Hidden>;
	CROpportunityClass__Description: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	allowUpdate: false,
	adjustPageSize: true,
	fastFilterByAllFields: false,
})
export class CSAnswers extends PXView {
	isRequired: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	AttributeID: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	Value: PXFieldState;
}
