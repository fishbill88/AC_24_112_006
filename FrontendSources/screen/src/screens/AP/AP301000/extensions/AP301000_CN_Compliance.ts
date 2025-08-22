import { AP301000 } from '../AP301000';
import { PXView, createCollection, PXFieldState, PXActionState, linkCommand, PXFieldOptions, gridConfig, GridPreset } from 'client-controls';

export interface AP3010000_Compliance extends AP301000 { }

export class AP3010000_Compliance {

	// PX.Objects.CN.Compliance.AP.GraphExtensions+ApInvoiceEntryExt

	ComplianceViewProject: PXActionState;
	ComplianceViewCostTask: PXActionState;
	ComplianceViewRevenueTask: PXActionState;
	ComplianceViewCostCode: PXActionState;
	ComplianceViewVendor: PXActionState;
	ComplianceViewJointVendor: PXActionState;
	ComplianceDocument$ApCheckID$Link: PXActionState;
	ComplianceDocument$BillID$Link: PXActionState;
	ComplianceDocument$ArPaymentID$Link: PXActionState;
	ComplianceViewCustomer: PXActionState;
	ComplianceDocument$InvoiceID$Link: PXActionState;
	ComplianceDocument$ProjectTransactionID$Link: PXActionState;
	ComplianceDocument$Subcontract$Link: PXActionState;
	ComplianceDocument$ChangeOrderNumber$Link: PXActionState;
	ComplianceDocument$PurchaseOrder$Link: PXActionState;
	ComplianceViewSecondaryVendor: PXActionState;

	ComplianceDocuments = createCollection(ComplianceDocument);

}

@gridConfig({ preset: GridPreset.Details })
export class ComplianceDocument extends PXView {

	LinkToPayment: PXFieldState;
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

	@linkCommand("ComplianceViewJointVendor")
	JointVendorInternalId: PXFieldState<PXFieldOptions.CommitChanges>;

	JointVendorExternalName: PXFieldState;

	@linkCommand("ComplianceDocument$ApCheckID$Link")
	ApCheckID: PXFieldState<PXFieldOptions.CommitChanges>;

	CheckNumber: PXFieldState;
	JointAmount: PXFieldState;
	LienWaiverAmount: PXFieldState;

	@linkCommand("ComplianceDocument$BillID$Link")
	BillID: PXFieldState<PXFieldOptions.CommitChanges>;

	BillAmount: PXFieldState;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("ComplianceDocument$ArPaymentID$Link")
	ArPaymentID: PXFieldState<PXFieldOptions.CommitChanges>;

	CertificateNumber: PXFieldState;
	CreatedByID: PXFieldState;

	@linkCommand("ComplianceViewCustomer")
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;

	CustomerName: PXFieldState;
	DateIssued: PXFieldState;
	EffectiveDate: PXFieldState;
	InsuranceCompany: PXFieldState;
	InvoiceAmount: PXFieldState;

	@linkCommand("ComplianceDocument$InvoiceID$Link")
	InvoiceID: PXFieldState<PXFieldOptions.CommitChanges>;

	IsExpired: PXFieldState;
	IsRequiredJointCheck: PXFieldState;
	JointRelease: PXFieldState;
	JointReleaseReceived: PXFieldState;
	LastModifiedByID: PXFieldState;
	Limit: PXFieldState;
	MethodSent: PXFieldState;
	PaymentDate: PXFieldState;
	ApPaymentMethodID: PXFieldState;
	ArPaymentMethodID: PXFieldState;
	Policy: PXFieldState;

	@linkCommand("ComplianceDocument$ProjectTransactionID$Link")
	ProjectTransactionID: PXFieldState<PXFieldOptions.CommitChanges>;

	PurchaseOrderLineItem: PXFieldState;

	@linkCommand("ComplianceDocument$Subcontract$Link")
	Subcontract: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("ComplianceDocument$ChangeOrderNumber$Link")
	ChangeOrderNumber: PXFieldState<PXFieldOptions.CommitChanges>;

	SubcontractLineItem: PXFieldState;

	@linkCommand("ComplianceDocument$PurchaseOrder$Link")
	PurchaseOrder: PXFieldState<PXFieldOptions.CommitChanges>;

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
