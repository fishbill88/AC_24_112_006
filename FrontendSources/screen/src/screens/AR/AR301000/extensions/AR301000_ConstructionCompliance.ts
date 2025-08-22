import {
	AR301000
} from '../AR301000';

import {
	PXView, createCollection, PXFieldState, PXFieldOptions, featureInstalled, PXActionState,
	linkCommand, localizable, gridConfig, columnConfig, viewInfo
} from 'client-controls';


export interface AR301000_ConstructionCompliance extends AR301000 { }

@featureInstalled('PX.Objects.CS.FeaturesSet+Construction')
export class AR301000_ConstructionCompliance {

	ComplianceViewProject: PXActionState;
	ComplianceViewCostTask: PXActionState;
	ComplianceViewRevenueTask: PXActionState;
	ComplianceViewCostCode: PXActionState;
	ComplianceViewCustomer: PXActionState;
	ComplianceViewVendor: PXActionState;
	ComplianceDocument$BillID$Link: PXActionState;
	ComplianceDocument$ApCheckID$Link: PXActionState;
	ComplianceDocument$ArPaymentID$Link: PXActionState;
	ComplianceDocument$InvoiceID$Link: PXActionState;
	ComplianceViewJointVendor: PXActionState;
	ComplianceDocument$ProjectTransactionID$Link: PXActionState;
	ComplianceDocument$PurchaseOrder$Link: PXActionState;
	ComplianceDocument$Subcontract$Link: PXActionState;
	ComplianceDocument$ChangeOrderNumber$Link: PXActionState;
	ComplianceViewSecondaryVendor: PXActionState;

	@viewInfo({ containerName: "Compliance" })
	ComplianceDocuments = createCollection(ComplianceDocument);
}

@gridConfig({ syncPosition: true, initNewRow: true })
export class ComplianceDocument extends PXView {

	ExpirationDate: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
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

	@linkCommand("ComplianceViewCustomer")
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;

	CustomerName: PXFieldState;

	@linkCommand("ComplianceViewVendor")
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;

	VendorName: PXFieldState;

	@linkCommand("ComplianceDocument$BillID$Link")
	BillID: PXFieldState<PXFieldOptions.CommitChanges>;

	BillAmount: PXFieldState;

	@linkCommand("ComplianceDocument$ApCheckID$Link")
	ApCheckID: PXFieldState<PXFieldOptions.CommitChanges>;

	CheckNumber: PXFieldState;

	@linkCommand("ComplianceDocument$ArPaymentID$Link")
	ArPaymentID: PXFieldState;

	CertificateNumber: PXFieldState;
	CreatedByID: PXFieldState;

	@linkCommand("ComplianceDocument$InvoiceID$Link")
	InvoiceID: PXFieldState<PXFieldOptions.CommitChanges>;

	DateIssued: PXFieldState;
	EffectiveDate: PXFieldState;
	InsuranceCompany: PXFieldState;
	InvoiceAmount: PXFieldState;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
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
