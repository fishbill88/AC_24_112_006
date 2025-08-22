import {
	PXView,
	PXFieldState,
	graphInfo,
	createSingle,
	PXScreen,
	PXFieldOptions,
	linkCommand,
	PXActionState,
	viewInfo,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CR.LeadMaint",
	primaryView: "Lead",
	udfTypeField: "ClassID",
	showUDFIndicator: true,
})
export class CR301000 extends PXScreen {
	ContactTeamsCardOffline: PXActionState;
	ContactTeamsCardAvailable: PXActionState;
	ContactTeamsCardBusy: PXActionState;
	ContactTeamsCardAway: PXActionState;
	OwnerTeamsCardOffline: PXActionState;
	OwnerTeamsCardAvailable: PXActionState;
	OwnerTeamsCardBusy: PXActionState;
	OwnerTeamsCardAway: PXActionState;
	NewMailActivity: PXActionState;
	ViewOnMap: PXActionState;
	CreateContactCancel: PXActionState;
	CreateContactFinish: PXActionState;
	CreateContactFinishRedirect: PXActionState;
	AddressLookupSelectAction: PXActionState;
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

	@viewInfo({ containerName: "Lead Summary" })
	Lead = createSingle(CRLead);

	@viewInfo({ containerName: "Contact Info" })
	AddressCurrent = createSingle(Address);

	LeadActivityStatistics = createSingle(CRActivityStatistics);

	@viewInfo({ containerName: "Address Lookup" })
	AddressLookupFilter = createSingle(AddressLookupFilter);

	@viewInfo({ containerName: "Teams Contact" })
	TeamsContactCard = createSingle(SMTeamsMember);
	@viewInfo({ containerName: "Teams Contact" })
	TeamsOwnerCard = createSingle(SMTeamsMember);
}

export class CRLead extends PXView {
	ViewLinkingDuplicateRefContact: PXActionState;
	ViewMergingDuplicate: PXActionState;
	ViewMergingDuplicateRefContact: PXActionState;
	ViewMergingDuplicateBAccount: PXActionState;
	ViewLinkingDuplicate: PXActionState;
	ViewLinkingDuplicateBAccount: PXActionState;

	ContactID: PXFieldState;
	Title: PXFieldState<PXFieldOptions.Hidden>;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	Resolution: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	RefContactID: PXFieldState<PXFieldOptions.CommitChanges>;
	BAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	Source: PXFieldState;
	CampaignID: PXFieldState<PXFieldOptions.CommitChanges>;
	DuplicateFound: PXFieldState<PXFieldOptions.Hidden>;
	DuplicateStatus: PXFieldState<PXFieldOptions.Disabled>;

	OverrideRefContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FirstName: PXFieldState<PXFieldOptions.CommitChanges>;
	LastName: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState<PXFieldOptions.CommitChanges>;
	Salutation: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("NewMailActivity")
	EMail: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1Type: PXFieldState;
	Phone1: PXFieldState;
	Phone2Type: PXFieldState;
	Phone2: PXFieldState;
	Phone3Type: PXFieldState;
	Phone3: PXFieldState;
	FaxType: PXFieldState;
	Fax: PXFieldState;
	WebSite: PXFieldState<PXFieldOptions.CommitChanges>;

	ConsentAgreement: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsentDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsentExpirationDate: PXFieldState<PXFieldOptions.CommitChanges>;

	ClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideSalesTerritory: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesTerritoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExtRefNbr: PXFieldState;
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	Method: PXFieldState;
	NoCall: PXFieldState;
	NoMarketing: PXFieldState;
	NoEMail: PXFieldState;
	NoMassMail: PXFieldState;
	LanguageID: PXFieldState;
}

export class Address extends PXView {
	AddressLookup: PXActionState;

	AddressLine1: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine2: PXFieldState<PXFieldOptions.CommitChanges>;
	City: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState;
}

export class CRActivityStatistics extends PXView {
	LastIncomingActivityDate: PXFieldState;
	LastOutgoingActivityDate: PXFieldState;
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

export class SMTeamsMember extends PXView {
	PhotoFileName: PXFieldState;
	DisplayName: PXFieldState;
	TeamsStatus: PXFieldState;
	JobTitle: PXFieldState;
	CompanyName: PXFieldState;
	Email: PXFieldState;
	MobilePhone: PXFieldState;
}
