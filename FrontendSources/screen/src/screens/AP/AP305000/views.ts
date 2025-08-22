import { PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, PXActionState } from "client-controls";

export class CABatch extends PXView {
	BatchNbr: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	TranDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ExtRefNbr: PXFieldState;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	PaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	ReferenceID: PXFieldState<PXFieldOptions.Disabled>;
	ExportTime: PXFieldState;
	TranDesc: PXFieldState;
	CuryDetailTotal: PXFieldState<PXFieldOptions.Disabled>;
	BatchSeqNbr: PXFieldState;
	DateSeqNbr: PXFieldState<PXFieldOptions.Disabled>;
	CountOfPayments: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({ syncPosition: true, adjustPageSize: true })
export class CABatchDetail extends PXView {

	AddPayments: PXActionState;

	@columnConfig({ allowUpdate: false })
	OrigDocType: PXFieldState;
	@linkCommand('ViewAPDocument')
	@columnConfig({ allowUpdate: false })
	APPayment__RefNbr: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	APPayment__VendorID: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	APPayment__VendorLocationID: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	APPayment__CuryID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APPayment__DocDesc: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	APPayment__PaymentMethodID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APPayment__ExtRefNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APPayment__DocDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APPayment__CuryOrigDocAmt: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APRegisterAlias__DocDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AddendaPaymentRelatedInfo: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APPayment__Status: PXFieldState;
}

@gridConfig({ adjustPageSize: true, suppressNoteFiles: true })
export class APPayment extends PXView {
	@columnConfig({ allowUpdate: false, allowCheckAll: true })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowUpdate: false })
	RefNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DocDesc: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DocDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryOrigDocAmt: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__RefNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__InvoiceNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__DocDesc: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APInvoice__DocDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APAdjust__CuryAdjgAmt: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Vendor__AcctName: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DocType: PXFieldState;
	@columnConfig({ allowUpdate: false })
	VendorID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	VendorID_BAccountR_acctName: PXFieldState;
	@columnConfig({ allowUpdate: false })
	VendorLocationID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	ExtRefNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DepositAfter: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CashAccountID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CashAccountID_CashAccount_Descr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	PaymentMethodID: PXFieldState;
}

export class AddPaymentsFilter extends PXView {
	NextPaymentRefNumber: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class VoidFilter extends PXView {
	VoidDateOption: PXFieldState<PXFieldOptions.CommitChanges>;
	VoidDate: PXFieldState<PXFieldOptions.CommitChanges>;
}
