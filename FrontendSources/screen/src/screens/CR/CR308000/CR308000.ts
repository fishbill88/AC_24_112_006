import {
	PXView,
	PXFieldState,
	gridConfig,
	PXFieldOptions,
	linkCommand,
	columnConfig,
	GridColumnShowHideMode,
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	GridPreset,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CR.CRMassMailMaint",
	primaryView: "MassMails",
})
export class CR308000 extends PXScreen {
	@viewInfo({ containerName: "" })
	MassMails = createSingle(CRMassMail);
	@viewInfo({ containerName: "Leads/Contacts/Employees" })
	Leads = createCollection(Contact);
	@viewInfo({ containerName: "Campaigns" })
	Campaigns = createCollection(CRCampaign);
	@viewInfo({ containerName: "Marketing List" })
	MailLists = createCollection(CRMarketingList);
	@viewInfo({ containerName: "Messages" })
	History = createCollection(CRActivity);
	@viewInfo({ containerName: "Activities" })
	Activities = createCollection(CRActivity2);
}

export class CRMassMail extends PXView {
	MassMailCD: PXFieldState;
	MailAccountID: PXFieldState;
	MailSubject: PXFieldState;
	MailTo: PXFieldState;
	MailCc: PXFieldState;
	MailBcc: PXFieldState;
	Source: PXFieldState<PXFieldOptions.CommitChanges>;
	PlannedDate: PXFieldState;
	Status: PXFieldState;
	SentDateTime: PXFieldState<PXFieldOptions.Disabled>;
	MailContent: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	allowUpdate: false,
	adjustPageSize: true,
})
export class Contact extends PXView {
	@columnConfig({
		allowUpdate: false,
		allowCheckAll: true,
		allowShowHide: GridColumnShowHideMode.False,
	})
	Selected: PXFieldState;
	@columnConfig({ allowUpdate: false })
	ContactType: PXFieldState;
	@linkCommand("Leads_ViewDetails")
	@columnConfig({ allowUpdate: false })
	DisplayName: PXFieldState;
	@columnConfig({ allowUpdate: false })
	BAccount__AcctCD: PXFieldState;
	@columnConfig({ allowUpdate: false })
	FullName: PXFieldState;
	@columnConfig({ allowUpdate: false })
	BAccount__ClassID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	IsActive: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	ClassID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	Source: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	Status: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	Title: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	Salutation: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({
		allowUpdate: false,
		allowShowHide: GridColumnShowHideMode.False,
	})
	ContactID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	EMail: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Address__AddressLine1: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	Address__AddressLine2: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	Phone1: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	Phone2: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	Phone3: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	Fax: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	WebSite: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	DateOfBirth: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	LastModifiedByID_Modifier_Username: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	WorkgroupID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	OwnerID: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	allowUpdate: true,
	adjustPageSize: true,
	fastFilterByAllFields: false,
	suppressNoteFiles: true,
})
export class CRCampaign extends PXView {
	Selected: PXFieldState;
	@linkCommand("MassEmailCampaign_CRMarketingCampaign_ViewDetails")
	@columnConfig({ allowUpdate: false })
	CampaignID: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	CampaignName: PXFieldState;
	Status: PXFieldState;
	StartDate: PXFieldState;
	EndDate: PXFieldState;
	SendFilter: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CampaignType: PXFieldState<PXFieldOptions.Hidden>;;
	PromoCodeID: PXFieldState<PXFieldOptions.Hidden>;;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	allowUpdate: false,
	adjustPageSize: true,
	fastFilterByAllFields: false,
	suppressNoteFiles: true,
})
export class CRMarketingList extends PXView {
	@columnConfig({ allowUpdate: false, allowCheckAll: true })
	Selected: PXFieldState;
	@linkCommand("MassEmailMarketingLists_CRMarketingList_ViewDetails")
	@columnConfig({ allowUpdate: false })
	MailListCode: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Name: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
	adjustPageSize: true,
})
export class CRActivity extends PXView {
	@linkCommand("History_ViewDetails")
	@columnConfig({ allowUpdate: false })
	Subject: PXFieldState;
	@columnConfig({ allowUpdate: false })
	MailTo: PXFieldState;
	@columnConfig({ allowUpdate: false })
	StartDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	MPStatus: PXFieldState;
	@linkCommand("ViewEntity")
	@columnConfig({ allowUpdate: false })
	Source: PXFieldState;
	@linkCommand("ViewDocument")
	@columnConfig({ allowUpdate: false })
	DocumentSource: PXFieldState;
	@linkCommand("History_Contact_ViewDetails")
	@columnConfig({ allowUpdate: false })
	ContactID: PXFieldState;
	@linkCommand("History_BAccount_ViewDetails")
	@columnConfig({ allowUpdate: false })
	BAccountID: PXFieldState;
}

gridConfig({
	preset: GridPreset.Inquiry,
})
export class CRActivity2 extends PXView {
	@columnConfig({ allowUpdate: false, visible: false })
	IsPinned: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	IsCompleteIcon: PXFieldState;
	@columnConfig({ allowUpdate: false })
	PriorityIcon: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CRReminder__ReminderIcon: PXFieldState;
	@columnConfig({ allowUpdate: false })
	ClassIcon: PXFieldState;
	@columnConfig({ allowUpdate: false })
	ClassInfo: PXFieldState;
	@linkCommand("ViewActivity")
	@columnConfig({ allowUpdate: false })
	Subject: PXFieldState;
	@columnConfig({ allowUpdate: false })
	UIStatus: PXFieldState;
	@columnConfig({ allowUpdate: false, visible: false })
	Released: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	StartDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CreatedDateTime: PXFieldState;
	@columnConfig({ allowUpdate: false })
	TimeSpent: PXFieldState;
	@columnConfig({
		allowUpdate: false,
		allowShowHide: GridColumnShowHideMode.False,
		visible: false,
	})
	CreatedByID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false, visible: false })
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	WorkgroupID: PXFieldState;
	@linkCommand("OpenActivityOwner")
	@columnConfig({ allowUpdate: false })
	OwnerID: PXFieldState;
	@columnConfig({ allowUpdate: false, visible: false })
	Source: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false, visible: false })
	BAccountID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false, visible: false })
	ContactID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false, visible: false })
	ProjectID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false, visible: false })
	ProjectTaskID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ visible: false })
	body: PXFieldState<PXFieldOptions.Hidden>;
}