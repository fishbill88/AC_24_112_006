import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	viewInfo,
	handleEvent,
	CustomEventType,
	RowSelectedHandlerArgs,
	PXViewCollection,
	PXView,
	PXFieldState,
	gridConfig,
	selectorSettings,
	PXFieldOptions,
	linkCommand,
	columnConfig,
	GridColumnShowHideMode,
	GridColumnType,
	GridPreset,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CR.BusinessAccountMaint",
	primaryView: "BAccount",
	udfTypeField: "ClassID", showUDFIndicator: true
})
export class CR303000 extends PXScreen {
	OwnerTeamsCardOffline: PXActionState;
	OwnerTeamsCardAvailable: PXActionState;
	OwnerTeamsCardBusy: PXActionState;
	OwnerTeamsCardAway: PXActionState;
	AddressLookup: PXActionState;
	ViewMainOnMap: PXActionState;
	NewMailActivity: PXActionState;
	ContactTeamsCardOffline: PXActionState;
	ContactTeamsCardAvailable: PXActionState;
	ContactTeamsCardBusy: PXActionState;
	ContactTeamsCardAway: PXActionState;
	DefLocationAddressLookup: PXActionState;
	ViewDefLocationAddressOnMap: PXActionState;
	CreateContactCancel: PXActionState;
	CreateContactFinish: PXActionState;
	CreateContactFinishRedirect: PXActionState;
	StatusIconContactOffline: PXActionState;
	StatusIconContactAvailable: PXActionState;
	StatusIconContactBusy: PXActionState;
	StatusIconContactAway: PXActionState;
	ContactChat: PXActionState;
	ContactCall: PXActionState;
	ContactMeeting: PXActionState;
	StatusIconOwnerOffline: PXActionState;
	StatusIconOwnerAvailable: PXActionState;
	StatusIconOwnerBusy: PXActionState;
	StatusIconOwnerAway: PXActionState;
	OwnerChat: PXActionState;
	OwnerCall: PXActionState;
	OwnerMeeting: PXActionState;
	AddressLookupSelectAction: PXActionState;
	viewMergingDuplicate: PXActionState;
	viewLinkingDuplicate: PXActionState;
	viewLinkingDuplicateRefContact: PXActionState;
	viewMergingDuplicateBAccount: PXActionState;
	viewLinkingDuplicateBAccount: PXActionState;

	@viewInfo({ containerName: "Account Summary" })
	BAccount = createSingle(BAccount);
	CurrentBAccount = createSingle(BAccount2);
	@viewInfo({ containerName: "General" })
	DefAddress = createSingle(Address);
	@viewInfo({ containerName: "General" })
	DefContact = createSingle(Contact);
	@viewInfo({ containerName: "General" })
	PrimaryContactCurrent = createSingle(Contact2);
	@viewInfo({ containerName: "Records for Merging" })
	DuplicatesForMerging = createCollection(CRDuplicateRecord);
	BAccountActivityStatistics = createSingle(CRActivityStatistics);

	@viewInfo({ containerName: "Contacts" })
	Contacts = createCollection(Contact3);

	@viewInfo({ containerName: "Locations" })
	Locations = createCollection(Location);

	@viewInfo({ containerName: "Shipping" })
	DefLocation = createSingle(Location2);
	@viewInfo({ containerName: "Shipping" })
	DefLocationAddress = createSingle(Address2);
	@viewInfo({ containerName: "Shipping" })
	DefLocationContact = createSingle(Contact4);

	@viewInfo({ containerName: "Leads" })
	Leads = createCollection(CRLead);

	@viewInfo({ containerName: "Opportunities" })
	Opportunities = createCollection(CROpportunity);

	@viewInfo({ containerName: "Cases" })
	Cases = createCollection(CRCase);

	@viewInfo({ containerName: "Contracts" })
	Contracts = createCollection(Contract);

	@viewInfo({ containerName: "Orders" })
	Orders = createCollection(SOOrder);

	@viewInfo({ containerName: "Campaigns" })
	Members = createCollection(CRCampaignMembers);

	@viewInfo({ containerName: "Mailings" })
	NotificationSources = createCollection(NotificationSource);

	@viewInfo({ containerName: "Recipients" })
	NotificationRecipients = createCollection(NotificationRecipient);

	@viewInfo({ containerName: "Specify New ID" })
	ChangeIDDialog = createSingle(ChangeIDParam);
	@viewInfo({ containerName: "Create Contact" })
	ContactInfo = createSingle(ContactFilter);
	@viewInfo({ containerName: "Create Contact" })
	ContactInfoAttributes = createCollection(FieldValue);

	@viewInfo({ containerName: "Create Contact" })
	ContactInfoUDF = createCollection(FieldValue2);

	@viewInfo({ containerName: "Teams Contact" })
	TeamsContactCard = createSingle(SMTeamsMember);
	@viewInfo({ containerName: "Teams Contact" })
	TeamsOwnerCard = createSingle(SMTeamsMember2);
	@viewInfo({ containerName: "Address Lookup" })
	AddressLookupFilter = createSingle(AddressLookupFilter);
	@viewInfo({ containerName: "Merge Conflicts" })
	Merge_Filter = createSingle(MergeEntitiesFilter);
	@viewInfo({ containerName: "Merge Conflicts" })
	Merge_VisibleComparisonRows = createCollection(ComparisonRow);
}

export class BAccount extends PXView {
	AcctCD: PXFieldState;
	AcctName: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	ClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrimaryContactID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class BAccount2 extends PXView {
	AcctName: PXFieldState<PXFieldOptions.CommitChanges>;
	Type: PXFieldState<PXFieldOptions.Disabled>;
	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideSalesTerritory: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesTerritoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	ParentBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	AcctReferenceNbr: PXFieldState;
	CampaignSourceID: PXFieldState;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowOverrideCury: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class Address extends PXView {
	AddressLine1: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine2: PXFieldState<PXFieldOptions.CommitChanges>;
	City: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}

export class Contact extends PXView {
	Phone1Type: PXFieldState;
	Phone1: PXFieldState;
	Phone2Type: PXFieldState;
	Phone2: PXFieldState;
	FaxType: PXFieldState;
	Fax: PXFieldState;
	EMail: PXFieldState<PXFieldOptions.CommitChanges>;
	WebSite: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsentAgreement: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsentDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsentExpirationDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DuplicateFound: PXFieldState;
	DuplicateStatus: PXFieldState<PXFieldOptions.Disabled>;
	Method: PXFieldState;
	NoCall: PXFieldState;
	NoMarketing: PXFieldState;
	NoEMail: PXFieldState;
	NoMassMail: PXFieldState;
	LanguageID: PXFieldState;
}

export class Contact2 extends PXView {
	FirstName: PXFieldState<PXFieldOptions.CommitChanges>;
	LastName: PXFieldState<PXFieldOptions.CommitChanges>;
	Salutation: PXFieldState<PXFieldOptions.CommitChanges>;
	EMail: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone2Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone2: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsentAgreement: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsentDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsentExpirationDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	allowUpdate: false,
	adjustPageSize: true,
})
export class CRDuplicateRecord extends PXView {
	DuplicateMerge: PXActionState;
	DuplicateAttach: PXActionState;

	@linkCommand("ViewMergingDuplicate")
	DuplicateContact__BAccountID: PXFieldState;
	BAccountR__AcctName: PXFieldState;
	BAccountR__Status: PXFieldState;
	DuplicateContact__Email: PXFieldState;
	Phone1: PXFieldState;
	BAccountR__OwnerID: PXFieldState;
	DuplicateContact__WebSite: PXFieldState;
	DuplicateContact__Phone2: PXFieldState;
	DuplicateContact__Phone3: PXFieldState;
	Address__AddressLine1: PXFieldState;
	Address__AddressLine2: PXFieldState;
	Address__State: PXFieldState;
	Address__City: PXFieldState;
	Address__CountryID: PXFieldState;
	DuplicateContact__WorkgroupID: PXFieldState;
	CRActivityStatistics__LastIncomingActivityDate: PXFieldState;
	CRActivityStatistics__LastOutgoingActivityDate: PXFieldState;
	DuplicateContact__ContactID: PXFieldState;
	DuplicateContact__CreatedDateTime: PXFieldState;
	DuplicateContact__LastModifiedDateTime: PXFieldState;
	BAccountR__Type: PXFieldState;
	CanBeMerged: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
	adjustPageSize: true,
	topBarItems: {
		CreateContactToolBar: {
			index: 2,
			config: {
				commandName: "CreateContactToolBar",
				images: { normal: "main@RecordAdd" },
			},
		},
		MakeContactPrimary: {
			config: {
				commandName: "MakeContactPrimary",
				text: "Set as Primary",
			},
		},
	},
})
export class Contact3 extends PXView {
	CreateContactToolBar: PXActionState;
	MakeContactPrimary: PXActionState;

	IsActive: PXFieldState;
	IsPrimary: PXFieldState;
	@linkCommand("ViewContact")
	DisplayName: PXFieldState;
	Salutation: PXFieldState;
	EMail: PXFieldState;
	Phone1: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OwnerID: PXFieldState;
	FullName: PXFieldState<PXFieldOptions.Hidden>;
	ClassID: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	Source: PXFieldState<PXFieldOptions.Hidden>;
	AssignDate: PXFieldState<PXFieldOptions.Hidden>;
	DuplicateStatus: PXFieldState<PXFieldOptions.Hidden>;
	Phone2: PXFieldState<PXFieldOptions.Hidden>;
	Phone3: PXFieldState<PXFieldOptions.Hidden>;
	DateOfBirth: PXFieldState<PXFieldOptions.Hidden>;
	Fax: PXFieldState<PXFieldOptions.Hidden>;
	Gender: PXFieldState<PXFieldOptions.Hidden>;
	Method: PXFieldState<PXFieldOptions.Hidden>;
	NoCall: PXFieldState<PXFieldOptions.Hidden>;
	NoEMail: PXFieldState<PXFieldOptions.Hidden>;
	NoFax: PXFieldState<PXFieldOptions.Hidden>;
	NoMail: PXFieldState<PXFieldOptions.Hidden>;
	NoMarketing: PXFieldState<PXFieldOptions.Hidden>;
	NoMassMail: PXFieldState<PXFieldOptions.Hidden>;
	CampaignID: PXFieldState<PXFieldOptions.Hidden>;
	Phone1Type: PXFieldState<PXFieldOptions.Hidden>;
	Phone2Type: PXFieldState<PXFieldOptions.Hidden>;
	Phone3Type: PXFieldState<PXFieldOptions.Hidden>;
	FaxType: PXFieldState<PXFieldOptions.Hidden>;
	MaritalStatus: PXFieldState<PXFieldOptions.Hidden>;
	Spouse: PXFieldState<PXFieldOptions.Hidden>;
	Status: PXFieldState<PXFieldOptions.Hidden>;
	Resolution: PXFieldState<PXFieldOptions.Hidden>;
	LanguageID: PXFieldState<PXFieldOptions.Hidden>;
	ContactID: PXFieldState<PXFieldOptions.Hidden>;
	Address__CountryID: PXFieldState<PXFieldOptions.Hidden>;
	Address__State: PXFieldState<PXFieldOptions.Hidden>;
	Address__City: PXFieldState<PXFieldOptions.Hidden>;
	Address__AddressLine1: PXFieldState<PXFieldOptions.Hidden>;
	Address__AddressLine2: PXFieldState<PXFieldOptions.Hidden>;
	Address__PostalCode: PXFieldState<PXFieldOptions.Hidden>;
	CanBeMadePrimary: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	allowUpdate: false,
	adjustPageSize: true,
	suppressNoteFiles: true,
	topBarItems: {
		NewLocation: {
			index: 2,
			config: {
				commandName: "NewLocation",
				images: { normal: "main@RecordAdd" },
			},
		},
	},
})
export class Location extends PXView {
	NewLocation: PXActionState;
	SetDefaultLocation: PXActionState;

	IsActive: PXFieldState;
	@linkCommand("ViewLocation")
	LocationCD: PXFieldState;
	Descr: PXFieldState;
	IsDefault: PXFieldState;
	Address__City: PXFieldState;
	@columnConfig({ hideViewLink: true })
	Address__State: PXFieldState;
	@columnConfig({ hideViewLink: true })
	Address__CountryID: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Hidden>;
	Address__PostalCode: PXFieldState<PXFieldOptions.Hidden>;
	Address__State_description: PXFieldState<PXFieldOptions.Hidden>;
	Address__CountryID_description: PXFieldState<PXFieldOptions.Hidden>;
	Address__AddressLine1: PXFieldState<PXFieldOptions.Hidden>;
	Address__AddressLine2: PXFieldState<PXFieldOptions.Hidden>;
	CBranchID: PXFieldState<PXFieldOptions.Hidden>;
	CPriceClassID: PXFieldState<PXFieldOptions.Hidden>;
	CDefProjectID: PXFieldState<PXFieldOptions.Hidden>;
	TaxRegistrationID: PXFieldState<PXFieldOptions.Hidden>;
	CAvalaraExemptionNumber: PXFieldState<PXFieldOptions.Hidden>;
	CSiteID: PXFieldState<PXFieldOptions.Hidden>;
	CCarrierID: PXFieldState<PXFieldOptions.Hidden>;
	CShipTermsID: PXFieldState<PXFieldOptions.Hidden>;
	CShipZoneID: PXFieldState<PXFieldOptions.Hidden>;
	CFOBPointID: PXFieldState<PXFieldOptions.Hidden>;
	CResedential: PXFieldState<PXFieldOptions.Hidden>;
	CSaturdayDelivery: PXFieldState<PXFieldOptions.Hidden>;
	CInsurance: PXFieldState<PXFieldOptions.Hidden>;
	CShipComplete: PXFieldState<PXFieldOptions.Hidden>;
	COrderPriority: PXFieldState<PXFieldOptions.Hidden>;
	CLeadTime: PXFieldState<PXFieldOptions.Hidden>;
	CCalendarID: PXFieldState<PXFieldOptions.Hidden>;
}

export class Location2 extends PXView {
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	CBranchID: PXFieldState;
	CPriceClassID: PXFieldState;
	CDefProjectID: PXFieldState;
	TaxRegistrationID: PXFieldState;
	CTaxZoneID: PXFieldState;
	CTaxCalcMode: PXFieldState;
	CAvalaraExemptionNumber: PXFieldState;
	CAvalaraCustomerUsageType: PXFieldState;
	CSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	CCarrierID: PXFieldState<PXFieldOptions.CommitChanges>;
	CShipTermsID: PXFieldState;
	CShipZoneID: PXFieldState;
	CFOBPointID: PXFieldState;
	CResedential: PXFieldState;
	CSaturdayDelivery: PXFieldState;
	CInsurance: PXFieldState;
	CShipComplete: PXFieldState;
	COrderPriority: PXFieldState;
	CLeadTime: PXFieldState;
	CCalendarID: PXFieldState;
}

export class Address2 extends PXView {
	AddressLine1: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine2: PXFieldState<PXFieldOptions.CommitChanges>;
	City: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	Latitude: PXFieldState;
	Longitude: PXFieldState;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}

export class Contact4 extends PXView {
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1Type: PXFieldState;
	Phone1: PXFieldState;
	Phone2Type: PXFieldState;
	Phone2: PXFieldState;
	FaxType: PXFieldState;
	Fax: PXFieldState;
	EMail: PXFieldState<PXFieldOptions.CommitChanges>;
	WebSite: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
	adjustPageSize: true,
	fastFilterByAllFields: false,
	topBarItems: {
		AddOpportunity: {
			index: 2,
			config: {
				commandName: "CreateLead",
				images: { normal: "main@RecordAdd" },
			},
		},
	},
})
export class CRLead extends PXView {
	CreateLead: PXActionState;

	@linkCommand("Leads_ViewDetails")
	@columnConfig({
		allowUpdate: false,
		allowShowHide: GridColumnShowHideMode.False,
	})
	MemberName: PXFieldState;
	Salutation: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState;
	Address__City: PXFieldState<PXFieldOptions.Hidden>;
	Address__State: PXFieldState<PXFieldOptions.Hidden>;
	Address__CountryID: PXFieldState<PXFieldOptions.Hidden>;
	EMail: PXFieldState;
	Phone1: PXFieldState;
	Phone2: PXFieldState<PXFieldOptions.Hidden>;
	Phone3: PXFieldState<PXFieldOptions.Hidden>;
	Source: PXFieldState<PXFieldOptions.Hidden>;
	CampaignID: PXFieldState;
	Status: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OwnerID: PXFieldState;
	WorkgroupID: PXFieldState<PXFieldOptions.Hidden>;
	CRActivityStatistics__LastIncomingActivityDate: PXFieldState<PXFieldOptions.Hidden>;
	CRActivityStatistics__LastOutgoingActivityDate: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedByID: PXFieldState<PXFieldOptions.Hidden>;
	ContactID: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	allowUpdate: false,
	adjustPageSize: true,
	fastFilterByAllFields: false,
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
	StageID: PXFieldState;
	CROpportunityProbability__Probability: PXFieldState<PXFieldOptions.Hidden>;
	Status: PXFieldState;
	CuryProductsAmount: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
	CloseDate: PXFieldState;
	@linkCommand("Opportunities_BAccount_ViewDetails")
	BAccount__AcctCD: PXFieldState<PXFieldOptions.Hidden>;
	BAccount__AcctName: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("Opportunities_Contact_ViewDetails")
	Contact__DisplayName: PXFieldState<PXFieldOptions.Hidden>;
	WorkgroupID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true })
	OwnerID: PXFieldState;
	@linkCommand("Opportunities_CLassID_ViewDetails")
	ClassID: PXFieldState<PXFieldOptions.Hidden>;
	CROpportunityClass__Description: PXFieldState<PXFieldOptions.Hidden>;
	ContactID: PXFieldState<PXFieldOptions.Hidden>;
	CRLead__ContactID: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	allowUpdate: false,
	adjustPageSize: true,
	fastFilterByAllFields: false,
	topBarItems: {
		AddOpportunity: {
			index: 2,
			config: {
				commandName: "AddCase",
				images: { normal: "main@RecordAdd" },
			},
		},
	},
})
export class CRCase extends PXView {
	AddCase: PXActionState;

	CaseCD: PXFieldState;
	Subject: PXFieldState;
	CaseClassID: PXFieldState;
	ContractID: PXFieldState<PXFieldOptions.Hidden>;
	Severity: PXFieldState;
	Status: PXFieldState;
	Resolution: PXFieldState;
	@columnConfig({ allowUpdate: false })
	ReportedOnDateTime: PXFieldState;
	@columnConfig({ allowUpdate: false })
	TimeEstimated: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	ResolutionDate: PXFieldState;
	WorkgroupID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true })
	OwnerID: PXFieldState;
	Contact__ContactID: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	allowUpdate: false,
	adjustPageSize: true,
	fastFilterByAllFields: false,
})
export class Contract extends PXView {
	ContractCD: PXFieldState;
	Description: PXFieldState;
	Status: PXFieldState;
	ExpireDate: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	allowUpdate: false,
	adjustPageSize: true,
})
export class SOOrder extends PXView {
	@columnConfig({ hideViewLink: true })
	OrderType: PXFieldState;
	@linkCommand("Orders_ViewDetails")
	OrderNbr: PXFieldState;
	OrderDesc: PXFieldState<PXFieldOptions.Hidden>;
	CustomerOrderNbr: PXFieldState<PXFieldOptions.Hidden>;
	Status: PXFieldState<PXFieldOptions.Hidden>;
	RequestDate: PXFieldState;
	ShipDate: PXFieldState;
	ShipVia: PXFieldState<PXFieldOptions.Hidden>;
	ShipZoneID: PXFieldState<PXFieldOptions.Hidden>;
	OrderWeight: PXFieldState<PXFieldOptions.Hidden>;
	OrderVolume: PXFieldState<PXFieldOptions.Hidden>;
	OrderQty: PXFieldState;
	CuryOrderTotal: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
	ContactID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	MarginPct: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	fastFilterByAllFields: false,
})
export class CRCampaignMembers extends PXView {
	@linkCommand("Members_CRCampaign_ViewDetails")
	CampaignID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CRCampaign__CampaignName: PXFieldState;
	@linkCommand("MarketingListID_ViewDetails")
	CRMarketingList__MailListCode: PXFieldState<PXFieldOptions.Hidden>;
	CRCampaign__Status: PXFieldState;
	CRCampaign__StartDate: PXFieldState;
	CRCampaign__EndDate: PXFieldState;
	CRCampaign__PromoCodeID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CRCampaign__OwnerID: PXFieldState<PXFieldOptions.Hidden>;
	Contact__ContactType: PXFieldState<PXFieldOptions.Hidden>;
	Contact__FullName: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Phone1: PXFieldState<PXFieldOptions.Hidden>;
	CRCampaignMembers__CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	CRCampaign__CreatedByID: PXFieldState<PXFieldOptions.Hidden>;
	CRCampaign__CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	CRCampaign__LastModifiedByID: PXFieldState<PXFieldOptions.Hidden>;
	CRCampaign__LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({ 
	preset: GridPreset.Details,
	adjustPageSize: true
})
export class NotificationSource extends PXView {
	Format: PXFieldState;
	NBranchID: PXFieldState;
	Active: PXFieldState;
	SetupID: PXFieldState;
	@selectorSettings("ScreenID", "")
	ReportID: PXFieldState;
	@selectorSettings("Name", "")
	NotificationID: PXFieldState;
	EMailAccountID: PXFieldState;
	OverrideSource: PXFieldState;
	RecipientsBehavior: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
	adjustPageSize: true,
})
export class NotificationRecipient extends PXView {
	ContactType: PXFieldState;
	@selectorSettings("DisplayName", "")
	ContactID: PXFieldState;
	Active: PXFieldState;
	@columnConfig({
		allowUpdate: false,
		allowShowHide: GridColumnShowHideMode.False,
	})
	OriginalContactID: PXFieldState;
	Email: PXFieldState;
	Format: PXFieldState;
	AddTo: PXFieldState;
}

export class ChangeIDParam extends PXView {
	CD: PXFieldState;
}

export class ContactFilter extends PXView {
	FirstName: PXFieldState<PXFieldOptions.CommitChanges>;
	LastName: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState<PXFieldOptions.CommitChanges>;
	Salutation: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone2Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone2: PXFieldState<PXFieldOptions.CommitChanges>;
	Email: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactClass: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	initNewRow: true,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
})
export class FieldValue extends PXView {
	DisplayName: PXFieldState;
	Value: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	initNewRow: true,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
})
export class FieldValue2 extends PXView {
	DisplayName: PXFieldState;
	Value: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class SMTeamsMember extends PXView {
	PhotoFileName: PXFieldState;
	DisplayName: PXFieldState;
	TeamsStatus: PXFieldState;
	JobTitle: PXFieldState;
	CompanyName: PXFieldState;
	Email: PXFieldState;
	MobilePhone: PXFieldState;
}

export class SMTeamsMember2 extends PXView {
	PhotoFileName: PXFieldState;
	DisplayName: PXFieldState;
	TeamsStatus: PXFieldState;
	JobTitle: PXFieldState;
	CompanyName: PXFieldState;
	Email: PXFieldState;
	MobilePhone: PXFieldState;
}

export class AddressLookupFilter extends PXView {
	SearchAddress: PXFieldState;
	ViewName: PXFieldState;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	AddressLine3: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState;
	State: PXFieldState;
	PostalCode: PXFieldState;
	Latitude: PXFieldState;
	Longitude: PXFieldState;
}

export class MergeEntitiesFilter extends PXView {
	TargetRecord: PXFieldState<PXFieldOptions.CommitChanges>;
	Caption: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
})
export class ComparisonRow extends PXView {
	FieldDisplayName: PXFieldState;
	@columnConfig({ allowCheckAll: true })
	LeftValueSelected: PXFieldState;
	LeftValue: PXFieldState;
	@columnConfig({ allowCheckAll: true })
	RightValueSelected: PXFieldState;
	RightValue: PXFieldState;
}

export class CRActivityStatistics extends PXView {
	LastIncomingActivityDate: PXFieldState;
	LastOutgoingActivityDate: PXFieldState;
}
