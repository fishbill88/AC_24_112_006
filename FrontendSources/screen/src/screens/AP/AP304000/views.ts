import { PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, ICurrencyInfo } from "client-controls";

// Views

export class APQuickCheck extends PXView {
	DocType: PXFieldState;
	RefNbr: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	AdjDate: PXFieldState<PXFieldOptions.CommitChanges>;
	AdjFinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	DocDesc: PXFieldState;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	PaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExtRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryLineTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryTaxTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryOrigWhTaxAmt: PXFieldState<PXFieldOptions.Disabled>;
	CuryRoundDiff: PXFieldState<PXFieldOptions.Disabled>;
	CuryOrigDocAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDocBal: PXFieldState<PXFieldOptions.Disabled>;
	CuryTaxAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryOrigDiscAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryChargeAmt: PXFieldState<PXFieldOptions.Disabled>;
	BatchNbr: PXFieldState<PXFieldOptions.Disabled>;
	PrebookBatchNbr: PXFieldState<PXFieldOptions.Disabled>;
	VoidBatchNbr: PXFieldState<PXFieldOptions.Disabled>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	APAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	APSubID: PXFieldState;
	PrebookAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrebookSubID: PXFieldState;
	DocDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	Cleared: PXFieldState<PXFieldOptions.CommitChanges>;
	ClearDate: PXFieldState<PXFieldOptions.CommitChanges>;
	OrigRefNbr: PXFieldState<PXFieldOptions.Disabled>;
	TaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCalcMode: PXFieldState<PXFieldOptions.CommitChanges>;
	TermsID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryVatTaxableTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryVatExemptTotal: PXFieldState<PXFieldOptions.Disabled>;
	PrintCheck: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ initNewRow: true, syncPosition: true, allowDelete: false, allowInsert: false, adjustPageSize: true })
export class APTran extends PXView {
	@columnConfig({ allowUpdate: false })
	InventoryID: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	UOM: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Qty: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryUnitCost: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowUpdate: false })
	ManualPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowUpdate: false })
	CuryLineAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	AccountID: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	SubID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Box1099: PXFieldState;
	@columnConfig({ allowUpdate: false })
	TranDesc: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	BranchID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DeferredCode: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DefScheduleID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DRTermStartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowUpdate: false })
	DRTermEndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	TaxCategoryID: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	ProjectID: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	TaskID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CostCodeID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	LineNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AccountID_Account_description: PXFieldState;
	@columnConfig({ allowUpdate: false })
	NonBillable: PXFieldState;
}

@gridConfig({ adjustPageSize: true })
export class TaxTran extends PXView {
	@columnConfig({ allowUpdate: false })
	TaxID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowUpdate: false })
	TaxRate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryTaxableAmt: PXFieldState;
	@columnConfig({ allowUpdate: false })
	TaxUOM: PXFieldState;
	@columnConfig({ allowUpdate: false })
	TaxableQty: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryTaxAmt: PXFieldState;
	@columnConfig({ allowUpdate: false })
	NonDeductibleTaxRate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryExpenseAmt: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Tax__TaxType: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Tax__PendingTax: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Tax__ReverseTax: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Tax__ExemptTax: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Tax__StatisticalTax: PXFieldState;
}

@gridConfig({ allowDelete: false, allowInsert: false, allowUpdate: false, adjustPageSize: true })
export class EPApproval extends PXView {
	@columnConfig({ allowUpdate: false })
	ApproverEmployee__AcctCD: PXFieldState;
	@columnConfig({ allowUpdate: false })
	ApproverEmployee__AcctName: PXFieldState;
	@columnConfig({ allowUpdate: false })
	WorkgroupID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	ApprovedByEmployee__AcctCD: PXFieldState;
	@columnConfig({ allowUpdate: false })
	ApprovedByEmployee__AcctName: PXFieldState;
	@columnConfig({ allowUpdate: false })
	OrigOwnerID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	ApproveDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Status: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Reason: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AssignmentMapID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	RuleID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	StepID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CreatedDateTime: PXFieldState;
}

export class APContact extends PXView {
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1: PXFieldState;
	Email: PXFieldState;
}

export class APAddress extends PXView {
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({ adjustPageSize: true })
export class APPaymentChargeTran extends PXView {
	@columnConfig({ allowUpdate: false })
	EntryTypeID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowUpdate: false })
	SubID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryTranAmt: PXFieldState;
	@columnConfig({ allowUpdate: false })
	TranDesc: PXFieldState;
}

export class ReasonApproveRejectFilter extends PXView {
	Reason: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ReassignApprovalFilter extends PXView {
	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;
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
