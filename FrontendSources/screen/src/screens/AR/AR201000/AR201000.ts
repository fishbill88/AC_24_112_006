import {
	PXScreen, createSingle, createCollection, graphInfo, PXView, PXFieldState, PXFieldOptions, columnConfig, gridConfig, TextAlign, ICurrencyInfo, PXActionState, linkCommand, GridColumnShowHideMode
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AR.CustomerClassMaint', primaryView: 'CustomerClassRecord' })
export class AR201000 extends PXScreen {

	CustomerClassRecord = createSingle(CustomerClass);
	CurCustomerClassRecord = createSingle(CustomerClass);

	DunningSetup = createCollection(ARDunningCustomerClass);
	Mapping = createCollection(CSAttributeGroupList);
	NotificationSources = createCollection(NotificationSource);
	NotificationRecipients = createCollection(NotificationRecipient);
}


export class CustomerClass extends PXView {

	CustomerClassID: PXFieldState;
	Descr: PXFieldState;
	ChkServiceManagement: PXFieldState;
	LocaleName: PXFieldState;
	CountryID: PXFieldState;
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxZoneID: PXFieldState;
	RequireTaxZone: PXFieldState;
	TaxCalcMode: PXFieldState;
	AvalaraCustomerUsageType: PXFieldState;
	RequireAvalaraCustomerUsageType: PXFieldState;
	DefaultLocationCDFromBranch: PXFieldState;
	PriceClassID: PXFieldState;
	GroupMask: PXFieldState;
	SalesPersonID: PXFieldState;
	ShipVia: PXFieldState;
	ShipTermsID: PXFieldState;
	ShipComplete: PXFieldState;
	CreditRule: PXFieldState<PXFieldOptions.CommitChanges>;
	CreditLimit: PXFieldState;
	OverLimitAmount: PXFieldState;
	CreditDaysPastDue: PXFieldState;
	DefaultBillingCycleID: PXFieldState<PXFieldOptions.CommitChanges>;
	SendInvoicesTo: PXFieldState;
	BillShipmentSource: PXFieldState;
	DefaultBillingCustomerSource: PXFieldState<PXFieldOptions.CommitChanges>;
	BillCustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	BillLocationID: PXFieldState;
	TermsID: PXFieldState;
	StatementCycleId: PXFieldState<PXFieldOptions.CommitChanges>;
	DefPaymentMethodID: PXFieldState;
	AutoApplyPayments: PXFieldState;
	FinChargeApply: PXFieldState<PXFieldOptions.CommitChanges>;
	FinChargeID: PXFieldState<PXFieldOptions.CommitChanges>;
	SmallBalanceAllow: PXFieldState<PXFieldOptions.CommitChanges>;
	SmallBalanceLimit: PXFieldState;
	CuryID: PXFieldState;
	AllowOverrideCury: PXFieldState;
	CuryRateTypeID: PXFieldState;
	AllowOverrideRate: PXFieldState;
	DiscountLimit: PXFieldState;
	PaymentsByLinesAllowed: PXFieldState;
	RetainageApply: PXFieldState;
	MailInvoices: PXFieldState;
	PrintInvoices: PXFieldState<PXFieldOptions.CommitChanges>;
	MailDunningLetters: PXFieldState;
	PrintDunningLetters: PXFieldState;
	SendStatementByEmail: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintStatements: PXFieldState<PXFieldOptions.CommitChanges>;
	StatementType: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintCuryStatements: PXFieldState;
	SavePaymentProfiles: PXFieldState;
	DisablePayLink: PXFieldState<PXFieldOptions.CommitChanges>;
	DeliveryMethod: PXFieldState;
	AllowOverrideDeliveryMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	PayLinkPaymentMethod: PXFieldState;
	ARAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	ARSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscountAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscountSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	FreightAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	FreightSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscTakenAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscTakenSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepaymentAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepaymentSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	COGSAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	COGSSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	MiscAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	MiscSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	UnrealizedGainAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	UnrealizedGainSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	UnrealizedLossAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	UnrealizedLossSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainageAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainageSubID: PXFieldState<PXFieldOptions.CommitChanges>;
}


export class ARDunningCustomerClass extends PXView {
	DunningLetterLevel: PXFieldState;
	DueDays: PXFieldState<PXFieldOptions.CommitChanges>;
	DaysToSettle: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	DunningFee: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CSAttributeGroupList extends PXView {
	CRAttribute_ViewDetails: PXActionState;

	@linkCommand("CRAttribute_ViewDetails")
	AttributeID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsActive: PXFieldState;
	Description: PXFieldState;
	SortOrder: PXFieldState;
	Required: PXFieldState;
	CSAttribute__IsInternal: PXFieldState;
	ControlType: PXFieldState;
	DefaultValue: PXFieldState;
}

@gridConfig({ syncPosition: true, autoRepaint: ["NotificationRecipients"] })
export class NotificationSource extends PXView {
	Active: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SetupID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	NBranchID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	EMailAccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ReportID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	NotificationID: PXFieldState;

	Format: PXFieldState;
	RecipientsBehavior: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class NotificationRecipient extends PXView {
	Active: PXFieldState;
	ContactType: PXFieldState;
	OriginalContactID: PXFieldState;
	Format: PXFieldState;
	AddTo: PXFieldState;
	ContactID: PXFieldState;
}

