import { PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig } from 'client-controls';

export class PayBillsFilter extends PXView {
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	PayTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	PayAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	PayDate: PXFieldState<PXFieldOptions.CommitChanges>;
	PayFinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowPayInLessThan: PXFieldState<PXFieldOptions.CommitChanges>;
	PayInLessThan: PXFieldState<PXFieldOptions.CommitChanges>;
	Days: PXFieldState<PXFieldOptions.Disabled>;
	ShowDueInLessThan: PXFieldState<PXFieldOptions.CommitChanges>;
	DueInLessThan: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowDiscountExpiresInLessThan: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscountExpiresInLessThan: PXFieldState<PXFieldOptions.CommitChanges>;
	TakeDiscAlways: PXFieldState<PXFieldOptions.CommitChanges>;
	GLBalance: PXFieldState<PXFieldOptions.Disabled>;
	CashBalance: PXFieldState<PXFieldOptions.Disabled>;
	CurySelTotal: PXFieldState<PXFieldOptions.Disabled>;
	SelCount: PXFieldState<PXFieldOptions.Disabled>;
	APQuickBatchGeneration: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ allowInsert: false, allowUpdate: false, adjustPageSize: true })
export class APAdjust extends PXView {
	@columnConfig({ allowUpdate: false, allowCheckAll: true })
	Selected: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AdjdDocType: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AdjdRefNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AdjdLineNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	SeparateCheck: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryAdjgDiscAmt: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryAdjgAmt: PXFieldState;
	@columnConfig({ allowUpdate: false })
	VendorID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	VendorID_Vendor_acctName: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APTran__ProjectID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APTran__TaskID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APTran__CostCodeID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APTran__AccountID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APTran__InventoryID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__VendorLocationID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__SuppliedByVendorID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__IsRetainageDocument: PXFieldState;
	@linkCommand('ViewOriginalDocument')
	@columnConfig({ allowUpdate: false })
	APInvoice__OrigRefNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__PayDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__DueDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__DiscDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__DocDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__IsJointPayees: PXFieldState;
	@columnConfig({ allowUpdate: false })
	JointPayeeExternalName: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryJointAmountOwed: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryJointBalance: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryDocBal: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryDiscBal: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__CuryID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__InvoiceNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__DocDesc: PXFieldState;
}

@gridConfig({ initNewRow: true, allowDelete: false, allowInsert: false, allowUpdate: false, adjustPageSize: true })
export class APAdjust2 extends PXView {
	Selected: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AdjdDocType: PXFieldState;
	@linkCommand('ViewInvoice')
	@columnConfig({ allowUpdate: false })
	AdjdRefNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	SeparateCheck: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryAdjgDiscAmt: PXFieldState;
	@linkCommand('EditAmountPaid')
	@columnConfig({ allowUpdate: false })
	CuryAdjgAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowUpdate: false })
	VendorID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AdjdLineNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	VendorID_Vendor_acctName: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APTran__ProjectID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APTran__TaskID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APTran__CostCodeID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APTran__AccountID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APTran__InventoryID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__VendorLocationID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__SuppliedByVendorID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__IsRetainageDocument: PXFieldState;
	@linkCommand('ViewInvoice')
	@columnConfig({ allowUpdate: false })
	APInvoice__OrigRefNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__PayDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__DueDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__DiscDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__DocDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryDocBal: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryDiscBal: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__CuryID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__InvoiceNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__DocDesc: PXFieldState;
}
