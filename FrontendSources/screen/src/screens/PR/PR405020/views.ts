import { PXView, PXFieldState, gridConfig, linkCommand, columnConfig } from 'client-controls';

@gridConfig({ syncPosition: true })
export class PRPayment extends PXView  {
	TransactionDate: PXFieldState;
	DocType: PXFieldState;
	@linkCommand('viewStubReport')
	RefNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	PayGroupID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	PayPeriodID: PXFieldState;
	NetAmount: PXFieldState;
	GrossAmount: PXFieldState;
	StartDate: PXFieldState;
	EndDate: PXFieldState;
	TotalHours: PXFieldState;
	AverageRate: PXFieldState;
}

export class PayrollDocumentsFilter extends PXView  {
	ShowTaxFormsTab: PXFieldState;
}
