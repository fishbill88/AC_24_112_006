import {
	PXView, PXFieldState, gridConfig, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, PXActionState, GridPreset
} from "client-controls";

// Views

export class ChangeIDParam extends PXView {
	CD: PXFieldState;
}

@gridConfig({ preset: GridPreset.Inquiry })
export class Vendor extends PXView {
	VStatus: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrimaryContactID: PXFieldState<PXFieldOptions.CommitChanges>;
	LegalName: PXFieldState<PXFieldOptions.CommitChanges>;
	AcctReferenceNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	ParentBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocaleName: PXFieldState<PXFieldOptions.CommitChanges>;
	LandedCostVendor: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxAgency: PXFieldState<PXFieldOptions.CommitChanges>;
	IsLaborUnion: PXFieldState<PXFieldOptions.CommitChanges>;
	Vendor1099: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorT5018: PXFieldState<PXFieldOptions.CommitChanges>;
	BoxT5018: PXFieldState<PXFieldOptions.CommitChanges>;
	SocialInsNum: PXFieldState<PXFieldOptions.CommitChanges>;
	BusinessNumber: PXFieldState<PXFieldOptions.CommitChanges>;
	Box1099: PXFieldState;
	ForeignEntity: PXFieldState;
	FATCA: PXFieldState;
	SDEnabled: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorDefaultCostCodeId: PXFieldState;
	VendorDefaultInventoryId: PXFieldState;
	TermsID: PXFieldState;
	VOrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState;
	AllowOverrideCury: PXFieldState;
	CuryRateTypeID: PXFieldState;
	AllowOverrideRate: PXFieldState;
	RetainageApply: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainagePct: PXFieldState<PXFieldOptions.CommitChanges>;
	ShouldGenerateLienWaivers: PXFieldState;
	TinType: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscTakenAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscTakenSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepaymentAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepaymentSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	POAccrualAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	POAccrualSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrebookAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrebookSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesTaxAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesTaxSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	PurchTaxAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	PurchTaxSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxExpenseSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	SVATReversalMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	SVATInputTaxEntryRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	SVATOutputTaxEntryRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	SVATTaxInvoiceNumberingID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxPeriodType: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxReportFinPeriod: PXFieldState;
	UpdClosedTaxPeriods: PXFieldState;
	AutoGenerateTaxBill: PXFieldState;
	TaxReportRounding: PXFieldState;
	TaxReportPrecision: PXFieldState;
	TaxUseVendorCurPrecision: PXFieldState<PXFieldOptions.CommitChanges>;
	PayToVendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	PaymentsByLinesAllowed: PXFieldState;

	@linkCommand('viewDetails')
	@columnConfig({ allowUpdate: false })
	AcctCD: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AcctName: PXFieldState;

	OwnerID: PXFieldState;

}

export class VendorBalanceSummary extends PXView {
	Balance: PXFieldState;
	Balance_Label: PXFieldState;
	DepositsBalance: PXFieldState;
	DepositsBalance_Label: PXFieldState;
	RetainageBalance: PXFieldState;
	RetainageBalance_Label: PXFieldState;
}

export class Address extends PXView {
	AddressLine1: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine2: PXFieldState<PXFieldOptions.CommitChanges>;
	City: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
	Latitude: PXFieldState;
	Longitude: PXFieldState;
}

@gridConfig({ syncPosition: true, allowDelete: false, allowInsert: false, allowUpdate: false, adjustPageSize: true })
export class ContactInfo extends PXView {

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
	FirstName: PXFieldState<PXFieldOptions.CommitChanges>;
	LastName: PXFieldState<PXFieldOptions.CommitChanges>;
	Salutation: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState;
	Attention: PXFieldState;

	@columnConfig({ allowUpdate: false })
	IsActive: PXFieldState;

	@linkCommand('ViewContact')
	@columnConfig({ allowUpdate: false })
	DisplayName: PXFieldState;

	@columnConfig({ allowUpdate: false })
	IsPrimary: PXFieldState;

	@columnConfig({ allowUpdate: false })
	OwnerID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	ClassID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	LastModifiedDateTime: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CreatedDateTime: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Source: PXFieldState;

	@columnConfig({ allowUpdate: false })
	AssignDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	DuplicateStatus: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Phone3: PXFieldState;

	@columnConfig({ allowUpdate: false })
	DateOfBirth: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Gender: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Method: PXFieldState;

	@columnConfig({ allowUpdate: false })
	NoCall: PXFieldState;

	@columnConfig({ allowUpdate: false })
	NoEMail: PXFieldState;

	@columnConfig({ allowUpdate: false })
	NoFax: PXFieldState;

	@columnConfig({ allowUpdate: false })
	NoMail: PXFieldState;

	@columnConfig({ allowUpdate: false })
	NoMarketing: PXFieldState;

	@columnConfig({ allowUpdate: false })
	NoMassMail: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CampaignID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Phone3Type: PXFieldState;

	@columnConfig({ allowUpdate: false })
	MaritalStatus: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Spouse: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Status: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Resolution: PXFieldState;

	@columnConfig({ allowUpdate: false })
	LanguageID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	ContactID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Address__CountryID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Address__State: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Address__City: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Address__AddressLine1: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Address__AddressLine2: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Address__PostalCode: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CanBeMadePrimary: PXFieldState;

}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	allowUpdate: false
})
export class ContactInfoForGrid extends PXView {

	CreateContactToolBar: PXActionState;
	MakeContactPrimary: PXActionState;

	@columnConfig({ allowUpdate: false })
	IsActive: PXFieldState;

	@linkCommand('ViewContact')
	@columnConfig({ allowUpdate: false })
	DisplayName: PXFieldState;

	Salutation: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false })
	IsPrimary: PXFieldState;

	EMail: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1: PXFieldState;

}

@gridConfig({ syncPosition: true, preset: GridPreset.Inquiry })
export class LocationForGrid extends PXView {

	NewLocation: PXActionState;
	SetDefaultLocation: PXActionState;

	@columnConfig({ allowUpdate: false })
	IsActive: PXFieldState;

	@linkCommand('ViewLocation')
	@columnConfig({ allowUpdate: false })
	LocationCD: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Descr: PXFieldState;

	@columnConfig({ allowUpdate: false })
	IsDefault: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Address__City: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	Address__State: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	Address__CountryID: PXFieldState;

	Address__PostalCode: PXFieldState<PXFieldOptions.Hidden>;
	Address__State_description: PXFieldState<PXFieldOptions.Hidden>;
	Address__CountryID_description: PXFieldState<PXFieldOptions.Hidden>;

	@columnConfig({ hideViewLink: true })
	VExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	VExpenseSubID: PXFieldState<PXFieldOptions.CommitChanges>;

	VBranchID: PXFieldState<PXFieldOptions.Hidden>;
	VBranchID_description: PXFieldState<PXFieldOptions.Hidden>;
	CreatedByID_Description: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedByID_Description: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	TaxRegistrationID: PXFieldState<PXFieldOptions.Hidden>;

	VTaxZoneID: PXFieldState;

	VTaxCalcMode: PXFieldState<PXFieldOptions.Hidden>;
	VSiteID: PXFieldState<PXFieldOptions.Hidden>;
	VCarrierID: PXFieldState<PXFieldOptions.Hidden>;
	VShipTermsID: PXFieldState<PXFieldOptions.Hidden>;
	VFOBPointID: PXFieldState<PXFieldOptions.Hidden>;
	VLeadTime: PXFieldState<PXFieldOptions.Hidden>;
	VAllowAPBillBeforeReceipt: PXFieldState<PXFieldOptions.Hidden>;
	VRcptQtyMin: PXFieldState<PXFieldOptions.Hidden>;
	VRcptQtyMax: PXFieldState<PXFieldOptions.Hidden>;
	VRcptQtyThreshold: PXFieldState<PXFieldOptions.Hidden>;
	VRcptQtyAction: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({ syncPosition: true, allowDelete: false, allowInsert: false, allowUpdate: false, adjustPageSize: true })
export class Location extends PXView {

	@columnConfig({ allowUpdate: false })
	IsActive: PXFieldState;

	@linkCommand('ViewLocation')
	@columnConfig({ allowUpdate: false })
	LocationCD: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Descr: PXFieldState;

	@columnConfig({ allowUpdate: false })
	IsDefault: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Address__City: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	Address__State: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	Address__CountryID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	VExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	VExpenseSubID: PXFieldState<PXFieldOptions.CommitChanges>;

	VTaxZoneID: PXFieldState;

	OverrideRemitAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideRemitContact: PXFieldState<PXFieldOptions.CommitChanges>;
	VPaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	VCashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	VPaymentByType: PXFieldState;
	VPaymentLeadTime: PXFieldState;
	VSeparateCheck: PXFieldState;
	VPrepaymentPct: PXFieldState;
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	VBranchID: PXFieldState;
	VPrintOrder: PXFieldState;
	VEmailOrder: PXFieldState;
	TaxRegistrationID: PXFieldState;
	VTaxCalcMode: PXFieldState;
	VSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	VCarrierID: PXFieldState;
	VShipTermsID: PXFieldState;
	VFOBPointID: PXFieldState;
	VLeadTime: PXFieldState;
	VAllowAPBillBeforeReceipt: PXFieldState;
	VRcptQtyMin: PXFieldState;
	VRcptQtyMax: PXFieldState;
	VRcptQtyThreshold: PXFieldState;
	VRcptQtyAction: PXFieldState;
	VAPAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	VAPSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	VDiscountAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	VDiscountSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	VRetainageAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	VRetainageSubID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false })
	Address__PostalCode: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Address__State_description: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Address__CountryID_description: PXFieldState;

	@columnConfig({ allowUpdate: false })
	VBranchID_description: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CreatedByID_Description: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CreatedDateTime: PXFieldState;

	@columnConfig({ allowUpdate: false })
	LastModifiedByID_Description: PXFieldState;

	@columnConfig({ allowUpdate: false })
	LastModifiedDateTime: PXFieldState;

}

@gridConfig({ preset: GridPreset.Attributes })
export class VendorPaymentMethodDetail extends PXView {
	@columnConfig({ allowUpdate: false })
	PaymentMethodDetail__descr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DetailValue: PXFieldState;
}

@gridConfig({ preset: GridPreset.Details })
export class VendorBalanceSummaryByBaseCurrency extends PXView {
	@columnConfig({ allowUpdate: false })
	BaseCuryID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Balance: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DepositsBalance: PXFieldState;
	@columnConfig({ allowUpdate: false })
	RetainageBalance: PXFieldState;
}

@gridConfig({ preset: GridPreset.Inquiry })
export class CSAnswers extends PXView {
	@columnConfig({ allowUpdate: false, allowShowHide: GridColumnShowHideMode.False })
	AttributeID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	isRequired: PXFieldState;
	@columnConfig({ allowUpdate: false, allowShowHide: GridColumnShowHideMode.False })
	Value: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	allowUpdate: false
})
export class CRActivity extends PXView {

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

	@linkCommand('ViewActivity')
	Subject: PXFieldState;

	UIStatus: PXFieldState;
	Released: PXFieldState;
	StartDate: PXFieldState;
	CreatedDateTime: PXFieldState;
	TimeSpent: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	CreatedByID: PXFieldState;

	CreatedByID_Creator_Username: PXFieldState;
	WorkgroupID: PXFieldState;

	@linkCommand('OpenActivityOwner')
	OwnerID: PXFieldState;

	Source: PXFieldState;
	BAccountID: PXFieldState;
	ContactID: PXFieldState;
	ProjectID: PXFieldState;
	ProjectTaskID: PXFieldState;
	Body: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Details,
	autoRepaint: ["NotificationRecipients"]
})
export class NotificationSource extends PXView {

	@columnConfig({ allowUpdate: false })
	Format: PXFieldState;

	@columnConfig({ allowUpdate: false })
	NBranchID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Active: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	SetupID: PXFieldState;

	@selectorSettings('ScreenID', '')
	@columnConfig({ allowUpdate: false })

	@columnConfig({ hideViewLink: true })
	ReportID: PXFieldState;

	@selectorSettings('Name', '')
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	NotificationID: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	EMailAccountID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	OverrideSource: PXFieldState;

	@columnConfig({ allowUpdate: false })
	RecipientsBehavior: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true
})
export class NotificationRecipient extends PXView {
	@columnConfig({ allowUpdate: false })
	ContactType: PXFieldState;
	@selectorSettings('DisplayName', '')
	@columnConfig({ allowUpdate: false })
	ContactID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Active: PXFieldState;
	@columnConfig({ allowUpdate: false, allowShowHide: GridColumnShowHideMode.False })
	OriginalContactID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Email: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Format: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AddTo: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
})
export class ComplianceDocument extends PXView {

	@columnConfig({ allowUpdate: false })
	ExpirationDate: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false })
	DocumentType: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false })
	CreationDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Status: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false })
	Required: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Received: PXFieldState;

	@columnConfig({ allowUpdate: false })
	ReceivedDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	IsProcessed: PXFieldState;

	@columnConfig({ allowUpdate: false })
	IsVoided: PXFieldState;

	@columnConfig({ allowUpdate: false })
	IsCreatedAutomatically: PXFieldState;

	@columnConfig({ allowUpdate: false })
	SentDate: PXFieldState;

	@linkCommand('ComplianceViewProject')
	@columnConfig({ allowUpdate: false })
	ProjectID: PXFieldState;

	@linkCommand('ComplianceViewCostTask')
	@columnConfig({ allowUpdate: false })
	CostTaskID: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand('ComplianceViewRevenueTask')
	@columnConfig({ allowUpdate: false })
	RevenueTaskID: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand('ComplianceViewCostCode')
	@columnConfig({ allowUpdate: false })
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand('ComplianceViewVendor')
	@columnConfig({ allowUpdate: false })
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false })
	VendorName: PXFieldState;

	@linkCommand('ComplianceDocument$BillID$Link')
	@columnConfig({ allowUpdate: false })
	BillID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false })
	BillAmount: PXFieldState;

	@columnConfig({ allowUpdate: false })
	AccountID: PXFieldState;

	@linkCommand('ComplianceDocument$ApCheckID$Link')
	@columnConfig({ allowUpdate: false })
	ApCheckID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false })
	CheckNumber: PXFieldState;

	@linkCommand('ComplianceDocument$ArPaymentID$Link')
	@columnConfig({ allowUpdate: false })
	ArPaymentID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false })
	CertificateNumber: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CreatedByID: PXFieldState;

	@linkCommand('ComplianceViewCustomer')
	@columnConfig({ allowUpdate: false })
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false })
	CustomerName: PXFieldState;

	@columnConfig({ allowUpdate: false })
	DateIssued: PXFieldState;

	@columnConfig({ allowUpdate: false })
	EffectiveDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	InsuranceCompany: PXFieldState;

	@columnConfig({ allowUpdate: false })
	InvoiceAmount: PXFieldState;

	@linkCommand('ComplianceDocument$InvoiceID$Link')
	@columnConfig({ allowUpdate: false })
	InvoiceID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false })
	IsExpired: PXFieldState;

	@columnConfig({ allowUpdate: false })
	IsRequiredJointCheck: PXFieldState;

	@columnConfig({ allowUpdate: false })
	JointAmount: PXFieldState;

	@columnConfig({ allowUpdate: false })
	JointRelease: PXFieldState;

	@columnConfig({ allowUpdate: false })
	JointReleaseReceived: PXFieldState;

	@linkCommand('ComplianceViewJointVendor')
	@columnConfig({ allowUpdate: false })
	JointVendorInternalId: PXFieldState;

	@columnConfig({ allowUpdate: false })
	JointVendorExternalName: PXFieldState;

	@columnConfig({ allowUpdate: false })
	LastModifiedByID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	LienWaiverAmount: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Limit: PXFieldState;

	@columnConfig({ allowUpdate: false })
	MethodSent: PXFieldState;

	@columnConfig({ allowUpdate: false })
	PaymentDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	ArPaymentMethodID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	ApPaymentMethodID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Policy: PXFieldState;

	@linkCommand('ComplianceDocument$ProjectTransactionID$Link')
	@columnConfig({ allowUpdate: false })
	ProjectTransactionID: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand('ComplianceDocument$PurchaseOrder$Link')
	@columnConfig({ allowUpdate: false })
	PurchaseOrder: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false })
	PurchaseOrderLineItem: PXFieldState;

	@linkCommand('ComplianceDocument$Subcontract$Link')
	@columnConfig({ allowUpdate: false })
	Subcontract: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false })
	SubcontractLineItem: PXFieldState;

	@linkCommand('ComplianceDocument$ChangeOrderNumber$Link')
	@columnConfig({ allowUpdate: false })
	ChangeOrderNumber: PXFieldState<PXFieldOptions.CommitChanges>;


	@columnConfig({ allowUpdate: false })
	ReceiptDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	ReceiveDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	ReceivedBy: PXFieldState;

	@linkCommand('ComplianceViewSecondaryVendor')
	@columnConfig({ allowUpdate: false })
	SecondaryVendorID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false })
	SecondaryVendorName: PXFieldState;

	@columnConfig({ allowUpdate: false })
	SourceType: PXFieldState;

	@columnConfig({ allowUpdate: false })
	SponsorOrganization: PXFieldState;

	@columnConfig({ allowUpdate: false })
	ThroughDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	DocumentTypeValue: PXFieldState<PXFieldOptions.CommitChanges>;


}
