import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	linkCommand,
	columnConfig,
	gridConfig,
	GridColumnShowHideMode,
	GridPreset
} from "client-controls";

export class Document extends PXView {
	Module: PXFieldState;
	RefNbr: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	Description: PXFieldState<PXFieldOptions.Multiline>;
	OrigDocType: PXFieldState<PXFieldOptions.CommitChanges>;
	OrigDocNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	OrigNoteID: PXFieldState;
	QtyTotal: PXFieldState<PXFieldOptions.Disabled>;
	BillableQtyTotal: PXFieldState<PXFieldOptions.Disabled>;
	AmtTotal: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true
})
export class Transactions extends PXView {
	ViewAllocationSorce: PXActionState;
	SelectProjectRate: PXActionState;
	SelectBaseRate: PXActionState;
	CuryToggle: PXActionState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewProject")
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	DirectCostType: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("ViewTask")
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	ResourceID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewCustomer")
	BAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	LocationID: PXFieldState;
	@linkCommand("ViewInventory")
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShiftID: PXFieldState;
	Description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	Billable: PXFieldState;
	BillableQty: PXFieldState<PXFieldOptions.CommitChanges>;
	TranCuryUnitRate: PXFieldState<PXFieldOptions.CommitChanges>;
	TranCuryAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	TranCuryId: PXFieldState<PXFieldOptions.CommitChanges>;
	BaseCuryRate: PXFieldState;
	ProjectCuryAmount: PXFieldState;
	ProjectCuryId: PXFieldState;
	ProjectCuryRate: PXFieldState;
	StartDate: PXFieldState<PXFieldOptions.Hidden>;
	EndDate: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OffsetAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	OffsetSubID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OffsetAccountGroupID: PXFieldState;
	Date: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	BatchNbr: PXFieldState;
	EarningType: PXFieldState<PXFieldOptions.CommitChanges>;
	OvertimeMultiplier: PXFieldState;
	UseBillableQty: PXFieldState;
	Allocated: PXFieldState;
	Released: PXFieldState;
	ExcludedFromAllocation: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	Billed: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true
})
export class ComplianceDocuments extends PXView {
	ExpirationDate: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	DocumentType: PXFieldState<PXFieldOptions.CommitChanges>;
	CreationDate: PXFieldState;
	@columnConfig({ hideViewLink: true })
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
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ComplianceDocument$ProjectTransactionID$Link")
	ProjectTransactionID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ComplianceDocument$ApCheckID$Link")
	ApCheckID: PXFieldState<PXFieldOptions.CommitChanges>;
	CheckNumber: PXFieldState;
	@linkCommand("ComplianceDocument$ArPaymentID$Link")
	ArPaymentID: PXFieldState<PXFieldOptions.CommitChanges>;
	BillAmount: PXFieldState;
	@linkCommand("ComplianceDocument$BillID$Link")
	BillID: PXFieldState<PXFieldOptions.CommitChanges>;
	CertificateNumber: PXFieldState;
	CreatedByID: PXFieldState;
	DateIssued: PXFieldState;
	DocumentTypeValue: PXFieldState<PXFieldOptions.CommitChanges>;
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
	Policy: PXFieldState;
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
}

export class ProjectCuryInfo extends PXView {
	CuryRateTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEffDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DisplayCuryID: PXFieldState;
	BaseCuryID: PXFieldState;
	SampleCuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleRecipRate: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class BaseCuryInfo extends PXView {
	CuryRateTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEffDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DisplayCuryID: PXFieldState;
	BaseCuryID: PXFieldState;
	SampleCuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleRecipRate: PXFieldState<PXFieldOptions.CommitChanges>;
}

