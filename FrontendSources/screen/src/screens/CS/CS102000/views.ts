import {
	PXView, PXFieldState,
	gridConfig, columnConfig, linkCommand,
	PXFieldOptions, PXActionState
} from 'client-controls';

export class ChangeIDParam extends PXView  {
	CD: PXFieldState;
}

export class BAccount extends PXView  {
	AcctCD: PXFieldState;
	AcctName: PXFieldState<PXFieldOptions.CommitChanges>;
	OrganizationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Active: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class BAccount2 extends PXView  {
	CarrierFacility: PXFieldState;
	BranchRoleName: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchCountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultPrinterID: PXFieldState;
	LegalName: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxRegistrationID: PXFieldState<PXFieldOptions.CommitChanges>;
	MTDApplicationID: PXFieldState;
	Reporting1099: PXFieldState<PXFieldOptions.CommitChanges>;
	IsDunningCompanyBranchID: PXFieldState;
	TCC: PXFieldState;
	CFSFiler: PXFieldState;
	ForeignEntity: PXFieldState;
	ContactName: PXFieldState;
	CTelNumber: PXFieldState;
	CEmail: PXFieldState;
	NameControl: PXFieldState;
	BranchLogoName: PXFieldState;
	BranchLogoNameGetter: PXFieldState<PXFieldOptions.Disabled>;
	BranchLogoNameReport: PXFieldState;
	BranchLogoNameReportGetter: PXFieldState<PXFieldOptions.Disabled>;
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

export class Contact extends PXView  {
	FullName: PXFieldState;
	Attention: PXFieldState;
	EMail: PXFieldState<PXFieldOptions.CommitChanges>;
	WebSite: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1: PXFieldState;
	Phone2: PXFieldState;
	Fax: PXFieldState;
}

export class Address extends PXView  {
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}

export class Location extends PXView  {
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

export class Contact2 extends PXView  {
	FullName: PXFieldState;
	Attention: PXFieldState;
	EMail: PXFieldState<PXFieldOptions.CommitChanges>;
	WebSite: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1: PXFieldState;
	Phone2: PXFieldState;
	Fax: PXFieldState;
}

export class Address2 extends PXView  {
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({syncPosition: true, allowDelete: false, allowInsert: false})
export class EPEmployee extends PXView  {
	NewContact: PXActionState;

	@linkCommand('ViewContact')
	AcctCD: PXFieldState;

	Contact__DisplayName: PXFieldState;
	DepartmentID: PXFieldState;
	Address__City: PXFieldState;
	Address__State: PXFieldState;
	Contact__Phone1: PXFieldState;
	Contact__EMail: PXFieldState;
	VStatus: PXFieldState;
}

@gridConfig({syncPosition: true})
export class Ledger extends PXView  {
	@linkCommand('ViewLedger')
	LedgerCD: PXFieldState;
	Descr: PXFieldState;
	BalanceType: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BaseCuryID: PXFieldState;
	ConsolAllowed: PXFieldState;
}
