import { PXView, createSingle, graphInfo, viewInfo, PXScreen, PXActionState, linkCommand, createCollection, gridConfig, columnConfig, GridColumnDisplayMode, GridPreset } from "client-controls";
import { PXFieldOptions, PXFieldState } from "client-controls/descriptors/fieldstate";

@graphInfo({ graphType: 'PX.Objects.AR.CustomerMaint', primaryView: 'BAccount', showUDFIndicator: true })
export class AR303000 extends PXScreen {

	BAccount = createSingle(Customer);
	CurrentCustomer = createSingle(Customer);
	CustomerBalance = createSingle(CustomerBalance);
	DefAddress = createSingle(Address);
	DefContact = createSingle(ContactInfo);
	PrimaryContactCurrent = createSingle(ContactInfo);

	BillAddress = createSingle(Address);
	BillContact = createSingle(ContactInfo);

	DefPaymentMethodInstance = createSingle(DefPaymentMethodInstance);
	DefPaymentMethodInstanceDetails = createCollection(DefPaymentMethodInstanceDetails);

	DefLocation = createSingle(DefLocation);
	DefLocationAddress = createSingle(Address);
	DefLocationContact = createSingle(ContactInfo);

	Carriers = createCollection(Carriers);
	Balances = createCollection(Balances);
	Locations = createCollection(Locations);
	PaymentMethods = createCollection(PaymentMethods);
	SalesPersons = createCollection(SalesPersons);
	Contacts = createCollection(Contacts);
	ChildAccounts = createCollection(ChildAccounts);
	Answers = createCollection(Answers);
	Activities = createCollection(Activities);

	NotificationSources = createCollection(NotificationSources);

	@viewInfo({ containerName: "NotificationRecipients" })
	NotificationRecipients = createCollection(NotificationRecipients);

	CustomerBillingCycles = createCollection(CustomerBillingCycles);
	ComplianceDocuments = createCollection(ComplianceDocuments);

	//qp-panel
	ChangeIDDialog = createSingle(ChangeIDDialog);
	OnDemandStatementDialog = createSingle(OnDemandStatementDialog);

	AddressLookup: PXActionState;

	ViewMainOnMap: PXActionState;
	ViewRestrictionGroups: PXActionState;
	CustomerDocuments: PXActionState;
	StatementForCustomer: PXActionState;
	NewInvoiceMemo: PXActionState;
	NewSalesOrder: PXActionState;
	NewPayment: PXActionState;
	WriteOffBalance: PXActionState;
	BillingAddressLookup: PXActionState;
	ViewBillAddressOnMap: PXActionState;
	RegenerateLastStatement: PXActionState;
	GenerateOnDemandStatement: PXActionState;
	ARBalanceByCustomer: PXActionState;
	CustomerHistory: PXActionState;
	ARAgedPastDue: PXActionState;
	ARAgedOutstanding: PXActionState;
	ARRegister: PXActionState;
	CustomerDetails: PXActionState;
	CustomerStatement: PXActionState;
	SalesPrice: PXActionState;
	DefLocationAddressLookup: PXActionState;
	ViewDefLocationAddressOnMap: PXActionState;

	//qp-panel
	ChangeID: PXActionState;

}

export class OnDemandStatementDialog extends PXView {
	StatementDate: PXFieldState;
}

export class ChangeIDDialog extends PXView {
	CD: PXFieldState;
}

@gridConfig({ preset: GridPreset.Details })
export class ComplianceDocuments extends PXView {
	ExpirationDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DocumentType: PXFieldState<PXFieldOptions.CommitChanges>;
	CreationDate: PXFieldState;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	Required: PXFieldState;
	Received: PXFieldState;
	ReceivedDate: PXFieldState;
	IsProcessed: PXFieldState;
	IsVoided: PXFieldState;
	IsCreatedAutomatically: PXFieldState;
	SentDate: PXFieldState;

	@linkCommand("ComplianceViewProject")
	ProjectID: PXFieldState;

	@linkCommand("ComplianceViewCostTask")
	CostTaskID: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("ComplianceViewRevenueTask")
	RevenueTaskID: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("ComplianceViewCostCode")
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("ComplianceViewVendor")
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;

	VendorName: PXFieldState;

	@linkCommand("ComplianceDocument$BillID$Link")
	BillID: PXFieldState<PXFieldOptions.CommitChanges>;

	BillAmount: PXFieldState;

	@linkCommand("ComplianceViewCustomer")
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("ComplianceDocument$ApCheckID$Link")
	ApCheckID: PXFieldState<PXFieldOptions.CommitChanges>;

	CheckNumber: PXFieldState;

	@linkCommand("ComplianceDocument$ArPaymentID$Link")
	ArPaymentID: PXFieldState;

	CertificateNumber: PXFieldState;
	CreatedByID: PXFieldState;
	CustomerName: PXFieldState;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DateIssued: PXFieldState;
	EffectiveDate: PXFieldState;
	InsuranceCompany: PXFieldState;
	InvoiceAmount: PXFieldState;

	@linkCommand("ComplianceDocument$InvoiceID$Link")
	InvoiceID: PXFieldState<PXFieldOptions.CommitChanges>;

	IsExpired: PXFieldState;
	IsRequiredJointCheck: PXFieldState;
	JointAmount: PXFieldState;
	JointRelease: PXFieldState;
	JointReleaseReceived: PXFieldState;

	@linkCommand("ComplianceViewJointVendor")
	JointVendorInternalId: PXFieldState;

	JointVendorExternalName: PXFieldState;
	LastModifiedByID: PXFieldState;
	LienWaiverAmount: PXFieldState;
	Limit: PXFieldState;
	MethodSent: PXFieldState;
	PaymentDate: PXFieldState;
	ArPaymentMethodID: PXFieldState;
	ApPaymentMethodID: PXFieldState;
	Policy: PXFieldState;

	@linkCommand("ComplianceDocument$ProjectTransactionID$Link")
	ProjectTransactionID: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("ComplianceDocument$PurchaseOrder$Link")
	PurchaseOrder: PXFieldState<PXFieldOptions.CommitChanges>;

	PurchaseOrderLineItem: PXFieldState;

	@linkCommand("ComplianceDocument$Subcontract$Link")
	Subcontract: PXFieldState<PXFieldOptions.CommitChanges>;

	SubcontractLineItem: PXFieldState;

	@linkCommand("ComplianceDocument$ChangeOrderNumber$Link")
	ChangeOrderNumber: PXFieldState<PXFieldOptions.CommitChanges>;

	ReceiptDate: PXFieldState;
	ReceiveDate: PXFieldState;
	ReceivedBy: PXFieldState;

	@linkCommand("ComplianceViewSecondaryVendor")
	SecondaryVendorID: PXFieldState<PXFieldOptions.CommitChanges>;

	SecondaryVendorName: PXFieldState;
	SourceType: PXFieldState;
	SponsorOrganization: PXFieldState;
	ThroughDate: PXFieldState;
	DocumentTypeValue: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ preset: GridPreset.Details })
export class CustomerBillingCycles extends PXView {
	SrvOrdType: PXFieldState<PXFieldOptions.CommitChanges>;
	BillingCycleID: PXFieldState<PXFieldOptions.CommitChanges>;
	SendInvoicesTo: PXFieldState<PXFieldOptions.CommitChanges>;
	BillShipmentSource: PXFieldState<PXFieldOptions.CommitChanges>;
	FrequencyType: PXFieldState<PXFieldOptions.CommitChanges>;
	WeeklyFrequency: PXFieldState;
	MonthlyFrequency: PXFieldState;
}

@gridConfig({ preset: GridPreset.Details })
export class NotificationRecipients extends PXView {
	Active: PXFieldState;
	ContactType: PXFieldState;
	OriginalContactID: PXFieldState<PXFieldOptions.Hidden>;
	ContactID: PXFieldState;
	Email: PXFieldState;
	Format: PXFieldState;
	AddTo: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	autoRepaint: ["NotificationRecipients"]
})
export class NotificationSources extends PXView {
	Active: PXFieldState;
	OverrideSource: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SetupID: PXFieldState;
	NBranchID: PXFieldState;
	EMailAccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ReportID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	NotificationID: PXFieldState;

	Format: PXFieldState;
	RecipientsBehavior: PXFieldState<PXFieldOptions.CommitChanges>;
}


@gridConfig({
	preset: GridPreset.Inquiry,
	allowUpdate: false,
	topBarItems: {
		NewTask: { config: { commandName: "NewTask", text: "Create Task" } },
		NewEvent: { config: { commandName: "NewEvent", text: "Create Event" } },
		NewMailActivity: { config: { commandName: "NewMailActivity", text: "Create Email" } },
		NewActivity: {
			type: "menu-options",
			config: {
				text: "Create Activity",
				options: {
					NewActivityC: { text: "Create Chat", commandName: "NewActivityC" },
					NewActivityES: { text: "Create Estimation", commandName: "NewActivityES" },
					NewActivityM: { text: "Create Message", commandName: "NewActivityM" },
					NewActivityP: { text: "Create Phone Call", commandName: "NewActivityP" },
					NewActivityW: { text: "Create Work Item", commandName: "NewActivityW" },
					NewActivityN: { text: "Create Note", commandName: "NewActivityN" },
				}
			}
		}
	},
})
export class Activities extends PXView {

	NewTask: PXActionState;
	NewEvent: PXActionState;
	NewMailActivity: PXActionState;
	NewActivity: PXActionState;
	TogglePinActivity: PXActionState;

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
	CreatedByID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;

	@columnConfig({ hideViewLink: true })
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
	preset: GridPreset.Inquiry
})
export class Answers extends PXView {
	@columnConfig({ displayMode: GridColumnDisplayMode.Text, hideViewLink: true })
	AttributeID: PXFieldState;

	isRequired: PXFieldState;
	Value: PXFieldState;
}

@gridConfig({ preset: GridPreset.Details })
export class ChildAccounts extends PXView {
	CustomerID: PXFieldState;
	CustomerName: PXFieldState;
	BaseCuryID: PXFieldState;
	Balance: PXFieldState;
	SignedDepositsBalance: PXFieldState;
	UnreleasedBalance: PXFieldState;
	OpenOrdersBalance: PXFieldState;
	OldInvoiceDate: PXFieldState;
	ConsolidateToParent: PXFieldState;
	ConsolidateStatements: PXFieldState;
	SharedCreditPolicy: PXFieldState;
	StatementCycleId: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry
})
export class Contacts extends PXView {

	CreateContactToolBar: PXActionState;
	MakeContactPrimary: PXActionState;

	IsActive: PXFieldState;

	@linkCommand("ViewContact")
	DisplayName: PXFieldState;

	Salutation: PXFieldState;
	IsPrimary: PXFieldState;
	EMail: PXFieldState;
	Phone1: PXFieldState;

	// hidden by default
	OwnerID: PXFieldState<PXFieldOptions.Hidden>;
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

	// hidden at all
	CanBeMadePrimary: PXFieldState;
}

@gridConfig({ preset: GridPreset.Details })
export class SalesPersons extends PXView {
	SalesPersonID: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesPersonID_SalesPerson_descr: PXFieldState;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID_description: PXFieldState;
	CommisionPct: PXFieldState;
	IsDefault: PXFieldState;
}

@gridConfig({ preset: GridPreset.Details })
export class PaymentMethods extends PXView {

	AddPaymentMethod: PXActionState;
	ViewPaymentMethod: PXActionState;

	IsDefault: PXFieldState;
	PaymentMethodID: PXFieldState;
	Descr: PXFieldState;
	CashAccountID: PXFieldState;
	IsActive: PXFieldState;
	IsCustomerPaymentMethod: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	quickFilterFields: ["LocationCD", "Descr", "Address__City", "Address__State", "Address__CountryID"]
})
export class Locations extends PXView {

	RefreshLocation: PXActionState;
	NewLocation: PXActionState;
	SetDefaultLocation: PXActionState;
	ViewLocation: PXActionState;


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

	Address__PostalCode: PXFieldState<PXFieldOptions.Hidden>;
	Address__State_description: PXFieldState<PXFieldOptions.Hidden>;
	Address__CountryID_description: PXFieldState<PXFieldOptions.Hidden>;
	CPriceClassID: PXFieldState;
	CreatedByID_Description: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedByID_Description: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	CDefProjectID: PXFieldState<PXFieldOptions.Hidden>;
	TaxRegistrationID: PXFieldState<PXFieldOptions.Hidden>;
	CTaxZoneID: PXFieldState;
	CTaxCalcMode: PXFieldState<PXFieldOptions.Hidden>;
	CAvalaraExemptionNumber: PXFieldState<PXFieldOptions.Hidden>;
	CAvalaraCustomerUsageType: PXFieldState<PXFieldOptions.Hidden>;
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

	@columnConfig({ hideViewLink: true })
	CSalesAcctID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CSalesSubID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CARAccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CARSubID: PXFieldState;

	CDiscountAcctID: PXFieldState<PXFieldOptions.Hidden>;
	CDiscountSubID: PXFieldState<PXFieldOptions.Hidden>;
	CFreightAcctID: PXFieldState<PXFieldOptions.Hidden>;
	CFreightSubID: PXFieldState<PXFieldOptions.Hidden>;
	CBranchID: PXFieldState;
	CBranchID_description: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({ preset: GridPreset.Details })
export class Balances extends PXView {
	BaseCuryID: PXFieldState;
	CurrentBal: PXFieldState;
	TotalPrepayments: PXFieldState;
	ConsolidatedBalance: PXFieldState;
	RetainageBalance: PXFieldState;
}

@gridConfig({
	preset: GridPreset.ShortList,
	quickFilterFields: ["CustomerID", "CustomerID_description", "CarrierAccount", "CustomerLocationID"]
})
export class Carriers extends PXView {

	IsActive: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CarrierPluginID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	CarrierAccount: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CustomerLocationID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CountryID: PXFieldState;

	PostalCode: PXFieldState;
}

export class DefLocation extends PXView {
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
	CCarrierID: PXFieldState;
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

	CARAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	CARSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	CSalesAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	CSalesSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	CDiscountAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	CDiscountSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	CFreightAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	CFreightSubID: PXFieldState<PXFieldOptions.CommitChanges>;

	CRetainageAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	CRetainageSubID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ preset: GridPreset.Attributes })
export class DefPaymentMethodInstanceDetails extends PXView {
	DetailID_PaymentMethodDetail_descr: PXFieldState;
	Value: PXFieldState;
}

export class DefPaymentMethodInstance extends PXView {
	CCProcessingCenterID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerCCPID: PXFieldState<PXFieldOptions.CommitChanges>;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
}

export class Customer extends PXView {
	AcctCD: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerCategory: PXFieldState<PXFieldOptions.CommitChanges>;
	AcctName: PXFieldState<PXFieldOptions.CommitChanges>;
	TermsID: PXFieldState;
	StatementCycleId: PXFieldState<PXFieldOptions.CommitChanges>;
	COrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	AutoApplyPayments: PXFieldState;
	FinChargeApply: PXFieldState;
	SmallBalanceAllow: PXFieldState;
	SmallBalanceLimit: PXFieldState;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowOverrideCury: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRateTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowOverrideRate: PXFieldState<PXFieldOptions.CommitChanges>;
	PaymentsByLinesAllowed: PXFieldState;
	RetainageApply: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainagePct: PXFieldState<PXFieldOptions.CommitChanges>;
	CreditRule: PXFieldState<PXFieldOptions.CommitChanges>;
	CreditLimit_Label: PXFieldState;
	CreditLimit: PXFieldState<PXFieldOptions.CommitChanges>;
	CreditDaysPastDue: PXFieldState;
	ParentBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsolidateToParent: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsolidateStatements: PXFieldState<PXFieldOptions.CommitChanges>;
	SharedCreditPolicy: PXFieldState<PXFieldOptions.CommitChanges>;
	MailInvoices: PXFieldState;
	PrintInvoices: PXFieldState<PXFieldOptions.CommitChanges>;
	MailDunningLetters: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintDunningLetters: PXFieldState<PXFieldOptions.CommitChanges>;
	SendStatementByEmail: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintStatements: PXFieldState<PXFieldOptions.CommitChanges>;
	StatementType: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintCuryStatements: PXFieldState<PXFieldOptions.CommitChanges>;
	RequireCustomerSignature: PXFieldState;

	BillingCycleID: PXFieldState<PXFieldOptions.CommitChanges>;
	SendInvoicesTo: PXFieldState;
	BillShipmentSource: PXFieldState;
	DefaultBillingCustomerSource: PXFieldState<PXFieldOptions.CommitChanges>;
	BillCustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	BillLocationID: PXFieldState;

	AcctReferenceNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	LocaleName: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideBillAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideBillContact: PXFieldState<PXFieldOptions.CommitChanges>;
	DefPaymentMethodID: PXFieldState;
	SuggestRelatedItems: PXFieldState<PXFieldOptions.CommitChanges>;

	DiscTakenAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscTakenSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepaymentAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepaymentSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	COGSAcctID: PXFieldState<PXFieldOptions.CommitChanges>;

	PrimaryContactID: PXFieldState<PXFieldOptions.CommitChanges>;

	LegalName: PXFieldState;
	OwnerID: PXFieldState;
}

export class CustomerBalance extends PXView {

	Balance: PXFieldState;
	ConsolidatedBalance: PXFieldState;
	SignedDepositsBalance: PXFieldState;
	RetainageBalance: PXFieldState;

	Balance_Label: PXFieldState;
	ConsolidatedBalance_Label: PXFieldState;
	SignedDepositsBalance_Label: PXFieldState;
	RetainageBalance_Label: PXFieldState;

	UnreleasedBalance: PXFieldState;
	OpenOrdersBalance: PXFieldState;
	RemainingCreditLimit: PXFieldState;
	OldInvoiceDate: PXFieldState;
}

export class Address extends PXView {
	AddressLine1: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine2: PXFieldState<PXFieldOptions.CommitChanges>;
	City: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState;
	Latitude: PXFieldState;
	Longitude: PXFieldState;
}

export class ContactInfo extends PXView {

	Phone1Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone2Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone2: PXFieldState<PXFieldOptions.CommitChanges>;
	FaxType: PXFieldState<PXFieldOptions.CommitChanges>;
	Fax: PXFieldState<PXFieldOptions.CommitChanges>;
	EMail: PXFieldState<PXFieldOptions.CommitChanges>;
	WebSite: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState<PXFieldOptions.CommitChanges>;
	Attention: PXFieldState<PXFieldOptions.CommitChanges>;

	FirstName: PXFieldState<PXFieldOptions.CommitChanges>;
	LastName: PXFieldState<PXFieldOptions.CommitChanges>;
	Salutation: PXFieldState<PXFieldOptions.CommitChanges>;
}
