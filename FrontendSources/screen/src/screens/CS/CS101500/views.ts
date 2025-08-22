import {
	PXView, PXFieldState, PXActionState,
	gridConfig, columnConfig,
	PXFieldOptions, linkCommand
} from 'client-controls';


export class BAccount extends PXView {
	AcctCD: PXFieldState;
	AcctName: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class State extends PXView {
	IsBranchTabVisible: PXFieldState;
	IsDeliverySettingsTabVisible: PXFieldState;
	IsGLAccountsTabVisible: PXFieldState;
	IsRutRotTabVisible: PXFieldState;
	IsGroup: PXFieldState;
	IsCompanyGroupsVisible: PXFieldState;
}

export class LedgerCreateParameters extends PXView {
	LedgerCD: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ChangeIDParam extends PXView {
	CD: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class BAccount2 extends PXView {
	LegalName: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxRegistrationID: PXFieldState<PXFieldOptions.CommitChanges>;
	MTDApplicationID: PXFieldState;
}

export class Contact extends PXView {
	AddressLookup: PXActionState;
	ViewMainOnMap: PXActionState;

	FullName: PXFieldState;
	Attention: PXFieldState;
	EMail: PXFieldState<PXFieldOptions.CommitChanges>;
	WebSite: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1: PXFieldState;
	Phone2: PXFieldState;
	Fax: PXFieldState;
}

export class Address extends PXView {
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}

export class Organization extends PXView {
	OrganizationType: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState;
	UseForAnomalyService: PXFieldState<PXFieldOptions.CommitChanges>;
	CarrierFacility: PXFieldState;
	BaseCuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	FileTaxesByBranches: PXFieldState<PXFieldOptions.CommitChanges>;
	Reporting1099ByBranches: PXFieldState<PXFieldOptions.CommitChanges>;
	Reporting1099: PXFieldState<PXFieldOptions.CommitChanges>;
	RoleName: PXFieldState<PXFieldOptions.CommitChanges>;
	CountryID: PXFieldState;
	OrganizationLocalizationCode: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultPrinterID: PXFieldState;
	TCC: PXFieldState;
	CFSFiler: PXFieldState;
	ForeignEntity: PXFieldState;
	ContactName: PXFieldState;
	CTelNumber: PXFieldState;
	CEmail: PXFieldState;
	NameControl: PXFieldState;
	LogoName: PXFieldState;
	LogoNameGetter: PXFieldState<PXFieldOptions.Disabled>;
	LogoNameReport: PXFieldState;
	LogoNameReportGetter: PXFieldState<PXFieldOptions.Disabled>;
	OverrideThemeVariables: PXFieldState<PXFieldOptions.CommitChanges>;
	PrimaryColor: PXFieldState<PXFieldOptions.CommitChanges>;
	BackgroundColor: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowsRUTROT: PXFieldState<PXFieldOptions.CommitChanges>;
	RUTROTCuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	RUTROTClaimNextRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	RUTROTOrgNbrValidRegEx: PXFieldState;
	DefaultRUTROTType: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxAgencyAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	BalanceOnProcess: PXFieldState<PXFieldOptions.CommitChanges>;
	ROTPersonalAllowanceLimit: PXFieldState;
	ROTExtraAllowanceLimit: PXFieldState;
	ROTDeductionPct: PXFieldState;
	RUTPersonalAllowanceLimit: PXFieldState;
	RUTExtraAllowanceLimit: PXFieldState;
	RUTDeductionPct: PXFieldState;
}

export class Location extends PXView {
	CAvalaraExemptionNumber: PXFieldState;
	CAvalaraCustomerUsageType: PXFieldState;
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	VTaxZoneID: PXFieldState;
	CShipComplete: PXFieldState;
	CMPSalesSubID: PXFieldState;
	CMPExpenseSubID: PXFieldState;
	CMPFreightSubID: PXFieldState;
	CMPDiscountSubID: PXFieldState;
	CMPGainLossSubID: PXFieldState;
	CMPPayrollSubID: PXFieldState;
}

export class CommonSetup extends PXView {
	DecPlQty: PXFieldState<PXFieldOptions.CommitChanges>;
	DecPlPrcCst: PXFieldState;
	WeightUOM: PXFieldState;
	LinearUOM: PXFieldState;
	VolumeUOM: PXFieldState;
}

export class Company extends PXView {
	PhoneMask: PXFieldState;
}

@gridConfig({ allowInsert: false, allowDelete: false, syncPosition: true })
export class Branch extends PXView {
	AddBranch: PXActionState;

	@linkCommand('ViewBranch')
	BranchCD: PXFieldState;

	AcctName: PXFieldState;
	Active: PXFieldState;
	IsDunningCompanyBranchID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	RoleName: PXFieldState;
	Roles__Descr: PXFieldState;
}

export class Contact2 extends PXView {
	FullName: PXFieldState;
	Attention: PXFieldState;
	EMail: PXFieldState<PXFieldOptions.CommitChanges>;
	WebSite: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1: PXFieldState;
	Phone2: PXFieldState;
	Fax: PXFieldState;
}

export class Address2 extends PXView {
	DefLocationAddressLookup: PXActionState;

	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ allowDelete: false, allowInsert: false })
export class EPEmployee extends PXView {
	NewContact: PXActionState;

	@columnConfig({ hideViewLink: true })
	BranchAlias__BranchCD: PXFieldState;

	@linkCommand('ViewContact')
	AcctCD: PXFieldState;

	Contact__DisplayName: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DepartmentID: PXFieldState;

	Address__City: PXFieldState;

	@columnConfig({ hideViewLink: true })
	Address__State: PXFieldState;

	Contact__Phone1: PXFieldState;
	Contact__EMail: PXFieldState;
	VStatus: PXFieldState;
}

@gridConfig({ syncPosition: true })
export class OrganizationLedgerLink extends PXView {
	@linkCommand('ViewLedger')
	LedgerID: PXFieldState;

	Ledger__Descr: PXFieldState;
	Ledger__BalanceType: PXFieldState;

	@columnConfig({ hideViewLink: true })
	Ledger__BaseCuryID: PXFieldState;
	Ledger__ConsolAllowed: PXFieldState;
}

@gridConfig({ syncPosition: true })
export class GroupOrganizationLink extends PXView {
	SetAsPrimary: PXActionState;

	@linkCommand('ViewGroup')
	GroupID: PXFieldState;
	Organization__OrganizationName: PXFieldState;
	PrimaryGroup: PXFieldState<PXFieldOptions.CommitChanges>;
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
