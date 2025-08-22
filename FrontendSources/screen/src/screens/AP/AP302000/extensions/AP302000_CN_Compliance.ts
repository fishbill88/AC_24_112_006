import { AP302000 } from '../AP302000';
import { PXView, createCollection, PXFieldState, PXFieldOptions, linkCommand, gridConfig, PXActionState, featureInstalled, GridPreset } from 'client-controls';

export interface AP302000_CN_Compliance extends AP302000 { }

@featureInstalled('PX.Objects.CS.FeaturesSet+Construction')
export class AP302000_CN_Compliance {

	ComplianceViewProject: PXActionState;
	ComplianceViewCostTask: PXActionState;
	ComplianceViewRevenueTask: PXActionState;
	ComplianceViewCostCode: PXActionState;
	ComplianceViewCustomer: PXActionState;
	ComplianceViewVendor: PXActionState;
	ComplianceDocument$BillID$Link: PXActionState;
	ComplianceDocument$ArPaymentID$Link: PXActionState;
	ComplianceDocument$InvoiceID$Link: PXActionState;
	ComplianceViewJointVendor: PXActionState;
	ComplianceDocument$ApCheckID$Link: PXActionState;
	ComplianceDocument$ProjectTransactionID$Link: PXActionState;
	ComplianceDocument$Subcontract$Link: PXActionState;
	ComplianceDocument$ChangeOrderNumber$Link: PXActionState;
	ComplianceDocument$PurchaseOrder$Link: PXActionState;
	ComplianceViewSecondaryVendor: PXActionState;

	//PX.Objects.CN.Compliance.AP.GraphExtensions.ApInvoiceEntryExt

	ComplianceDocuments = createCollection(ComplianceDocuments);
	ComplianceDetails = createCollection(ComplianceDetails);

}

@gridConfig({ preset: GridPreset.Details })
export class ComplianceDocuments extends PXView {

	ComplianceViewDetails: PXActionState;
	SetAsFinal: PXActionState;

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
	CostCodeID: PXFieldState;

	@linkCommand("ComplianceViewCustomer")
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;

	CustomerName: PXFieldState;

	@linkCommand("ComplianceViewVendor")
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;

	VendorName: PXFieldState;

	@linkCommand("ComplianceDocument$BillID$Link")
	BillID: PXFieldState<PXFieldOptions.CommitChanges>;

	BillAmount: PXFieldState;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ArPaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	ApPaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("ComplianceDocument$ArPaymentID$Link")
	ArPaymentID: PXFieldState<PXFieldOptions.CommitChanges>;

	CertificateNumber: PXFieldState;
	CreatedByID: PXFieldState;
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

	@linkCommand("ComplianceDocument$ApCheckID$Link")
	ApCheckID: PXFieldState<PXFieldOptions.CommitChanges>;

	CheckNumber: PXFieldState;
	PaymentDate: PXFieldState;
	Policy: PXFieldState;

	@linkCommand("ComplianceDocument$ProjectTransactionID$Link")
	ProjectTransactionID: PXFieldState<PXFieldOptions.CommitChanges>;

	PurchaseOrderLineItem: PXFieldState;

	@linkCommand("ComplianceDocument$Subcontract$Link")
	Subcontract: PXFieldState<PXFieldOptions.CommitChanges>;

	SubcontractLineItem: PXFieldState;

	@linkCommand("ComplianceDocument$ChangeOrderNumber$Link")
	ChangeOrderNumber: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("ComplianceDocument$PurchaseOrder$Link")
	PurchaseOrder: PXFieldState<PXFieldOptions.CommitChanges>;

	ReceiveDate: PXFieldState;
	ReceivedBy: PXFieldState;

	@linkCommand("ComplianceViewSecondaryVendor")
	SecondaryVendorID: PXFieldState<PXFieldOptions.CommitChanges>;

	SecondaryVendorName: PXFieldState;
	SourceType: PXFieldState;
	SponsorOrganization: PXFieldState;
	ThroughDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DocumentTypeValue: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ preset: GridPreset.Details, allowDelete: false, allowInsert: false, allowUpdate: false })
export class ComplianceDetails extends PXView {

	DocType: PXFieldState;
	RefNbr: PXFieldState;
	LineNbr: PXFieldState;
	APInvoice__CuryID: PXFieldState;
	AmountPaid: PXFieldState;

}
