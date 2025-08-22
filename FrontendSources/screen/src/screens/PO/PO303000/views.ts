import {
	PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, PXFieldOptions,
	columnConfig, GridColumnShowHideMode, PXActionState, GridPreset } from "client-controls";

// Views

export class POLandedCostDocHeader extends PXView {
	RefNbr: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	DocDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	@headerDescription
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	VendorLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	CreateBill: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAllocatedTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryLineTotal: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryVatTaxableTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryVatExemptTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryTaxTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryDocTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryControlTotal: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class POLandedCostDoc extends PXView {
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	TermsID: PXFieldState<PXFieldOptions.CommitChanges>;
	BillDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DueDate: PXFieldState;
	DiscDate: PXFieldState;
	CuryDiscAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	PayToVendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
	adjustPageSize: true
})
export class POLandedCostDetail extends PXView {
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server, hideViewLink: true })
	BranchID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	LandedCostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	AllocationMethod: PXFieldState;
	CuryLineAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	TaxCategoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	APDocType: PXFieldState;
	APRefNbr: PXFieldState;
	INDocType: PXFieldState;
	INRefNbr: PXFieldState;
	LineNbr: PXFieldState;
	SortOrder: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	adjustPageSize: true
})
export class POLandedCostReceiptLine extends PXView {
	AddPOReceipt: PXActionState;
	AddPOReceiptLine: PXActionState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server, hideViewLink: true })
	BranchID: PXFieldState;
	InventoryID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SiteID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	ReceiptQty: PXFieldState;
	ExtWeight: PXFieldState;
	ExtVolume: PXFieldState;
	LineAmt: PXFieldState;
	@columnConfig({ hideViewLink: true })
	POReceiptBaseCuryID: PXFieldState;
	CuryAllocatedLCAmt: PXFieldState;
	POReceiptNbr: PXFieldState;
	POReceiptLineNbr: PXFieldState;
	LineNbr: PXFieldState;
	SortOrder: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true
})
export class POLandedCostTaxTran extends PXView {
	@columnConfig({ hideViewLink: true })
	TaxID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxRate: PXFieldState;
	CuryTaxableAmt: PXFieldState;
	CuryTaxAmt: PXFieldState;
}

export class POReceiptFilter extends PXView {
	ReceiptType: PXFieldState<PXFieldOptions.CommitChanges>;
	ReceiptNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry
})
export class POReceipt extends PXView {
	@columnConfig({allowCheckAll: true})
	Selected: PXFieldState;
	ReceiptType: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ReceiptNbr: PXFieldState;
	InvoiceNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	VendorID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
	ReceiptDate: PXFieldState;
	OrderQty: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry
})
export class POReceiptLineAdd extends PXView {
	@columnConfig({allowCheckAll: true})
	Selected: PXFieldState;
	PONbr: PXFieldState;
	POType: PXFieldState;
	ReceiptNbr: PXFieldState;
	InvoiceNbr: PXFieldState;
	VendorID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	InventoryID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SiteID: PXFieldState;
	ReceiptQty: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	TranCostFinal: PXFieldState;
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
