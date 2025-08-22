import {
	PXView,
	PXFieldState,
	gridConfig,
	headerDescription,
	ICurrencyInfo,
	disabled,
	selectorSettings,
	PXFieldOptions,
	linkCommand,
	columnConfig,
	GridColumnShowHideMode,
	GridColumnType,
	PXActionState,
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	handleEvent,
	CustomEventType,
	RowSelectedHandlerArgs,
	PXViewCollection,
	GridColumnGeneration,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CR.OpportunityMaint",
	primaryView: "Opportunity",
	udfTypeField: "ClassID", showUDFIndicator: true
})
export class CR304000 extends PXScreen {
	ContactTeamsCardOffline: PXActionState;
	ContactTeamsCardAvailable: PXActionState;
	ContactTeamsCardBusy: PXActionState;
	ContactTeamsCardAway: PXActionState;
	OwnerTeamsCardOffline: PXActionState;
	OwnerTeamsCardAvailable: PXActionState;
	OwnerTeamsCardBusy: PXActionState;
	OwnerTeamsCardAway: PXActionState;
	ShowMatrixPanel: PXActionState;
	AddressLookup: PXActionState;
	ViewMainOnMap: PXActionState;
	BillingAddressLookup: PXActionState;
	ViewBillingOnMap: PXActionState;
	ShippingAddressLookup: PXActionState;
	ViewShippingOnMap: PXActionState;
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
	CreateCustomerInPanel: PXActionState;
	CreateSalesOrderInPanel: PXActionState;
	AddressLookupSelectAction: PXActionState;

	@viewInfo({ containerName: "Opportunity Summary" })
	Opportunity = createSingle(CROpportunity);

	OpportunityCurrent = createSingle(CROpportunity2);

	currencyinfo = createSingle(CurrencyInfo);

	@viewInfo({ containerName: "Teams Contact" })
	TeamsContactCard = createSingle(SMTeamsMember);
	@viewInfo({ containerName: "Teams Contact" })
	TeamsOwnerCard = createSingle(SMTeamsMember2);

	@viewInfo({ containerName: "Address Lookup" })
	AddressLookupFilter = createSingle(AddressLookupFilter);
}

export class CROpportunity extends PXView {
	Products$ImportAction: PXActionState;

	OpportunityID: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	ClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	StageID: PXFieldState<PXFieldOptions.CommitChanges>;
	CloseDate: PXFieldState<PXFieldOptions.CommitChanges>;
	Subject: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Multiline>;
	BAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	ManualTotalEntry: PXFieldState<PXFieldOptions.CommitChanges>;
	AMCuryEstimateTotal: PXFieldState<PXFieldOptions.Disabled>;
	QuotedAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryLineDiscountTotal: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDiscTot: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryTaxTotal: PXFieldState<PXFieldOptions.Disabled>;
	TotalAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryProductsAmount: PXFieldState<PXFieldOptions.Disabled>;
	ChkServiceManagement: PXFieldState;
}

export class CurrencyInfo extends PXView implements ICurrencyInfo {
	CuryInfoID: PXFieldState;
	BaseCuryID: PXFieldState;
	BaseCalc: PXFieldState;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisplayCuryID: PXFieldState;
	CuryRateTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	BasePrecision: PXFieldState;
	CuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEffDate: PXFieldState<PXFieldOptions.CommitChanges>;
	RecipRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleCuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleRecipRate: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CROpportunity2 extends PXView {
	AllowOverrideContactAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	Resolution: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideSalesTerritory: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesTerritoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	ParentBAccountID: PXFieldState;
	LanguageID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsActive: PXFieldState;
	CuryWgtAmount: PXFieldState<PXFieldOptions.Disabled>;
	ClosingDate: PXFieldState<PXFieldOptions.Disabled>;
	Source: PXFieldState<PXFieldOptions.CommitChanges>;
	CampaignSourceID: PXFieldState<PXFieldOptions.CommitChanges>;
	LeadID: PXFieldState<PXFieldOptions.Disabled>;
	Details: PXFieldState;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	TermsID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExternalRef: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxRegistrationID: PXFieldState;
	TaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCalcMode: PXFieldState<PXFieldOptions.CommitChanges>;
	ExternalTaxExemptionNumber: PXFieldState;
	AvalaraCustomerUsageType: PXFieldState;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	CarrierID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipTermsID: PXFieldState;
	ShipZoneID: PXFieldState;
	FOBPointID: PXFieldState;
	Resedential: PXFieldState;
	SaturdayDelivery: PXFieldState;
	Insurance: PXFieldState;
	ShipComplete: PXFieldState;
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
})
export class FieldValue extends PXView {
	@columnConfig({ allowUpdate: false })
	DisplayName: PXFieldState;
	Value: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	initNewRow: true,
	allowDelete: false,
	allowInsert: false,
})
export class FieldValue2 extends PXView {
	@columnConfig({ allowUpdate: false })
	DisplayName: PXFieldState;
	Value: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	initNewRow: true,
	allowDelete: false,
	allowInsert: false,
})
export class FieldValue3 extends PXView {
	@columnConfig({ allowUpdate: false })
	DisplayName: PXFieldState;
	Value: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	initNewRow: true,
	allowDelete: false,
	allowInsert: false,
})
export class FieldValue4 extends PXView {
	@columnConfig({ allowUpdate: false })
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

export class EntryHeader extends PXView {
	TemplateItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ColAttributeID: PXFieldState<PXFieldOptions.CommitChanges>;
	RowAttributeID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowAvailable: PXFieldState<PXFieldOptions.CommitChanges>;
}
