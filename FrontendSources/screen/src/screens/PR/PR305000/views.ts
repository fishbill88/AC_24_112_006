import {
	PXView,
	PXFieldState,
	gridConfig,
	PXFieldOptions,
	linkCommand,
	columnConfig,
	PXActionState,
	GridPreset
} from 'client-controls';

export class CABatch extends PXView  {
	BatchNbr: PXFieldState;
	TranDate: PXFieldState<PXFieldOptions.CommitChanges>;
	CashAccountID: PXFieldState<PXFieldOptions.Disabled>;
	PaymentMethodID: PXFieldState<PXFieldOptions.Disabled>;
	TranDesc: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry
})
export class CABatchDetail extends PXView  {
	AddPayment: PXActionState;

	OrigDocType: PXFieldState;
	@linkCommand('ViewPRDocument')
	PRPayment__RefNbr: PXFieldState;
	PRPayment__Status: PXFieldState;
	PRPayment__DocDesc: PXFieldState;
	@columnConfig({ hideViewLink: true })
	PRPayment__EmployeeID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	PRPayment__PayGroupID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	PRPayment__PayPeriodID: PXFieldState;
	PRDirectDepositSplit__Amount: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry
})
export class PRPaymentBatchExportHistory extends PXView  {
	ShowExportDetails: PXActionState;

	UserID_Description: PXFieldState;
	ExportDateTime: PXFieldState;
	Reason: PXFieldState;
	BatchTotal: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry
})
export class PRPayment extends PXView  {
	RefNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	EmployeeID: PXFieldState;
	EmployeeID_description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	PayGroupID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	PayPeriodID: PXFieldState;
	TransactionDate: PXFieldState;
}

export class PRPaymentBatchFilter extends PXView  {
	PaymentBatchStatus: PXFieldState<PXFieldOptions.Disabled>;
	BatchTotal: PXFieldState<PXFieldOptions.Disabled>;
	ExportReason: PXFieldState<PXFieldOptions.CommitChanges>;
	OtherExportReason: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintReason: PXFieldState<PXFieldOptions.CommitChanges>;
	OtherPrintReason: PXFieldState<PXFieldOptions.CommitChanges>;
	NextCheckNbr: PXFieldState;
}

export class PRPayment2 extends PXView  {
	Selected: PXFieldState;
	DocType: PXFieldState;
	RefNbr: PXFieldState;
	Status: PXFieldState;
	DocDesc: PXFieldState;
	EmployeeID: PXFieldState;
	EmployeeID_description: PXFieldState;
	PayGroupID: PXFieldState;
	PayPeriodID: PXFieldState;
	NetAmount: PXFieldState;
}

export class PRPaymentBatchExportHistory2 extends PXView  {
	UserID_Description: PXFieldState;
	ExportDateTime: PXFieldState;
	Reason: PXFieldState;
}

export class PRPaymentBatchExportDetails extends PXView  {
	DocType: PXFieldState;
	RefNbr: PXFieldState;
	DocDesc: PXFieldState;
	@columnConfig({ hideViewLink: true })
	EmployeeID: PXFieldState;
	EmployeeID_description: PXFieldState;
	PayGroupID: PXFieldState;
	PayPeriodID: PXFieldState;
	NetAmount: PXFieldState;
	ExtRefNbr: PXFieldState;
}

export class PRPaymentBatchExportHistory3 extends PXView  {
	ExportDateTime: PXFieldState;
}
