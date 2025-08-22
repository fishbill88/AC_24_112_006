import {
	columnConfig,
	gridConfig,
	linkCommand,
	ICurrencyInfo,
	PXActionState,
	PXFieldOptions,
	PXFieldState,
	PXView,
	GridPreset
} from "client-controls";

export class Document extends PXView {
	RefNbr: PXFieldState;
	Status: PXFieldState;
	Hold: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Hidden>;
	InvoiceDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoiceNbr: PXFieldState;
	ProjectNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.Multiline>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState;
	CuryProgressiveTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryTransactionalTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryTaxTotalWithRetainage: PXFieldState<PXFieldOptions.Disabled>;
	CuryDocTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryRetainageTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryAmountDue: PXFieldState<PXFieldOptions.Disabled>;
	RetainagePct: PXFieldState<PXFieldOptions.Disabled>;
	CuryAllocatedRetainedTotal: PXFieldState<PXFieldOptions.Disabled>;
}

export class Project extends PXView {
	CuryID: PXFieldState<PXFieldOptions.Disabled>;
}

export class Overflow extends PXView {
	CuryOverflowTotal: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: true
})
export class Details extends PXView {
	@linkCommand("ViewTranDocument")
	RefNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	InventoryID: PXFieldState;
	Description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ResourceID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	BAccountID: PXFieldState;
	Date: PXFieldState;
	Billable: PXFieldState;
	Qty: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	ProjectCuryAmount: PXFieldState;
	InvoicedQty: PXFieldState;
	ProjectCuryInvoicedAmount: PXFieldState;
	ProjectCuryID: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: true
})
export class Unbilled extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;
	@linkCommand("ViewTranDocument")
	RefNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	InventoryID: PXFieldState;
	Description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ResourceID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	BAccountID: PXFieldState;
	Date: PXFieldState;
	Billable: PXFieldState;
	Qty: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	BillableQty: PXFieldState;
	TranCuryUnitRate: PXFieldState;
	TranCuryAmount: PXFieldState;
	@columnConfig({ hideViewLink: true })
	TranCuryID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	AccountGroupID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OffsetAccountID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OffsetSubID: PXFieldState;
}

export class DocumentSettings extends PXView {
	ARInvoiceDocType: PXFieldState<PXFieldOptions.CommitChanges>;
	ARInvoiceRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	TermsID: PXFieldState<PXFieldOptions.CommitChanges>;
	DueDate: PXFieldState;
	DiscDate: PXFieldState;
	TaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExternalTaxExemptionNumber: PXFieldState<PXFieldOptions.CommitChanges>;
	AvalaraCustomerUsageType: PXFieldState<PXFieldOptions.CommitChanges>;
	IsMigratedRecord: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details
})
export class ProgressiveLines extends PXView {
	UploadFromBudget: PXActionState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewProgressLineTask")
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewProgressLineInventory")
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	PMRevenueBudget__RevisedQty: PXFieldState;
	PMRevenueBudget__CuryRevisedAmount: PXFieldState;
	ActualQty: PXFieldState;
	PMRevenueBudget__CuryActualAmount: PXFieldState;
	PMRevenueBudget__CuryInvoicedAmount: PXFieldState;
	PreviouslyInvoicedQty: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryPreviouslyInvoiced: PXFieldState<PXFieldOptions.CommitChanges>;
	CompletedPct: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	CuryUnitPrice: PXFieldState;
	CuryAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryMaterialStoredAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryPrepaidAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryLineTotal: PXFieldState<PXFieldOptions.CommitChanges>;
	CurrentInvoicedPct: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainagePct: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAllocatedRetainedAmount: PXFieldState;
	CuryRetainage: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCategoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState;
	DefCode: PXFieldState;
	SortOrder: PXFieldState;
	LineNbr: PXFieldState;
	ProgressBillingBase: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true
})
export class TransactionLines extends PXView {
	UploadUnbilled: PXActionState;
	ViewTransactionDetails: PXActionState;

	@columnConfig({hideViewLink: true})
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewTransactLineTask")
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewTransactLineInventory")
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	@columnConfig({hideViewLink: true})
	ResourceID: PXFieldState;
	@linkCommand("ViewVendor")
	VendorID: PXFieldState;
	Date: PXFieldState;
	BillableQty: PXFieldState;
	CuryBillableAmount: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryPrepaidAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryMaxAmount: PXFieldState;
	CuryAvailableAmount: PXFieldState;
	CuryLineTotal: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryOverflowAmount: PXFieldState;
	Option: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainagePct: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRetainage: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	TaxCategoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	SubID: PXFieldState;
	DefCode: PXFieldState;
	SortOrder: PXFieldState;
	LineNbr: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details
})
export class Taxes extends PXView {
	TaxID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxRate: PXFieldState<PXFieldOptions.Disabled>;
	CuryTaxableAmt: PXFieldState;
	CuryExemptedAmt: PXFieldState;
	CuryTaxAmt: PXFieldState;
	CuryRetainedTaxableAmt: PXFieldState;
	CuryRetainedTaxAmt: PXFieldState;
	Tax__TaxType: PXFieldState;
	Tax__PendingTax: PXFieldState;
	Tax__ReverseTax: PXFieldState;
	Tax__ExemptTax: PXFieldState;
	Tax__StatisticalTax: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowDelete: false
})
export class Revisions extends PXView {
	@columnConfig({width: 120})
	RevisionID: PXFieldState;
	CuryDocTotal: PXFieldState;
	CuryRetainageTotal: PXFieldState;
	CuryTaxTotal: PXFieldState;
	ARInvoiceDocType: PXFieldState;
	ARInvoiceRefNbr: PXFieldState<PXFieldOptions.Disabled>;
	ReversedARInvoiceDocType: PXFieldState;
	ReversedARInvoiceRefNbr: PXFieldState<PXFieldOptions.Disabled>;
	Description: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class Approval extends PXView {
	ApproverEmployee__AcctCD: PXFieldState;
	ApproverEmployee__AcctName: PXFieldState;
	WorkgroupID: PXFieldState;
	ApprovedByEmployee__AcctCD: PXFieldState;
	ApprovedByEmployee__AcctName: PXFieldState;
	OrigOwnerID: PXFieldState<PXFieldOptions.Hidden>;
	ApproveDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Status: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Reason: PXFieldState;
	AssignmentMapID: PXFieldState<PXFieldOptions.Hidden>;
	RuleID: PXFieldState<PXFieldOptions.Hidden>;
	StepID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}

export class Billing_Contact extends PXView {
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1: PXFieldState;
	Email: PXFieldState;
}

export class Billing_Address extends PXView {
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}

export class Shipping_Contact extends PXView {
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1: PXFieldState;
	Email: PXFieldState;
}

export class Shipping_Address extends PXView {
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
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

export class ReassignApprovalFilter extends PXView {
	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ReasonApproveRejectParams extends PXView {
	Reason: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Multiline>;
}
