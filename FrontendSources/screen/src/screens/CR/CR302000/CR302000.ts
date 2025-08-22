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
	graphType: "PX.Objects.CR.ContactMaint",
	primaryView: "Contact",
	udfTypeField: "ClassID",
	showUDFIndicator: true,
})
export class CR302000 extends PXScreen {
	ContactTeamsCardOffline: PXActionState;
	ContactTeamsCardAvailable: PXActionState;
	ContactTeamsCardBusy: PXActionState;
	ContactTeamsCardAway: PXActionState;
	OwnerTeamsCardOffline: PXActionState;
	OwnerTeamsCardAvailable: PXActionState;
	OwnerTeamsCardBusy: PXActionState;
	OwnerTeamsCardAway: PXActionState;
	NewMailActivity: PXActionState;
	AddressLookup: PXActionState;
	ViewOnMap: PXActionState;
	ResetPassword: PXActionState;
	ActivateLogin: PXActionState;
	EnableLogin: PXActionState;
	DisableLogin: PXActionState;
	UnlockLogin: PXActionState;
	ResetPasswordOK: PXActionState;
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

	@viewInfo({ containerName: "Contact Summary" })
	Contact = createSingle(CRContact);
	AddressCurrent = createSingle(Address);
	ContactActivityStatistics = createSingle(CRActivityStatistics);

	@viewInfo({ containerName: "Teams Contact" })
	TeamsContactCard = createSingle(SMTeamsMember);
	@viewInfo({ containerName: "Teams Contact" })
	TeamsOwnerCard = createSingle(SMTeamsMember);
}

export class CRContact extends PXView {
	ViewLinkingDuplicateRefContact: PXActionState;
	ViewMergingDuplicate: PXActionState;
	ViewMergingDuplicateRefContact: PXActionState;
	ViewMergingDuplicateBAccount: PXActionState;
	ViewLinkingDuplicate: PXActionState;
	ViewLinkingDuplicateBAccount: PXActionState;

	ContactID: PXFieldState;
	Status: PXFieldState;
	BAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	DuplicateFound: PXFieldState<PXFieldOptions.Hidden>;
	DuplicateStatus: PXFieldState<PXFieldOptions.Disabled>;

	Title: PXFieldState;
	FirstName: PXFieldState;
	LastName: PXFieldState;
	FullName: PXFieldState;
	Salutation: PXFieldState;
	@linkCommand("NewMailActivity")
	EMail: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1: PXFieldState;
	Phone2Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone2: PXFieldState;
	Phone3Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone3: PXFieldState;
	FaxType: PXFieldState<PXFieldOptions.CommitChanges>;
	Fax: PXFieldState;
	WebSite: PXFieldState<PXFieldOptions.CommitChanges>;
	TeamsID: PXFieldState<PXFieldOptions.CommitChanges>;

	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;

	ConsentAgreement: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsentDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsentExpirationDate: PXFieldState<PXFieldOptions.CommitChanges>;

	//export class CRMInfo extends CRContact {
	ClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideSalesTerritory: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesTerritoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	ParentBAccountID: PXFieldState;
	ExtRefNbr: PXFieldState;
	Source: PXFieldState;
	Synchronize: PXFieldState;

	Method: PXFieldState;
	NoCall: PXFieldState;
	NoMarketing: PXFieldState;
	NoEMail: PXFieldState;
	NoMassMail: PXFieldState;
	LanguageID: PXFieldState;

	Img: PXFieldState;
	DateOfBirth: PXFieldState;
	Gender: PXFieldState;
	MaritalStatus: PXFieldState;
	Spouse: PXFieldState;
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

export class SMTeamsMember extends PXView {
	PhotoFileName: PXFieldState;
	DisplayName: PXFieldState;
	TeamsStatus: PXFieldState;
	JobTitle: PXFieldState;
	CompanyName: PXFieldState;
	Email: PXFieldState;
	MobilePhone: PXFieldState;
}
