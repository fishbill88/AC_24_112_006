import { PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, GridPreset } from 'client-controls';

export class PrintChecksFilter extends PXView  {
	PaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState;
	NextCheckNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	PaymentBatchNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	CashBalance: PXFieldState<PXFieldOptions.Disabled>;
	SelTotal: PXFieldState<PXFieldOptions.Disabled>;
	SelCount: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry
})
export class PRPayment extends PXView  {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	RefNbr: PXFieldState;
	TransactionDate: PXFieldState;
	DocType: PXFieldState;
	EmployeeID: PXFieldState;
	EmployeeID_PREmployee_acctName: PXFieldState;
	@columnConfig({ hideViewLink: true })
	PayGroupID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	PayPeriodID: PXFieldState;
	NetAmount: PXFieldState;
}
