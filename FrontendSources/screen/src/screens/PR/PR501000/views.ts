import { PXView, PXFieldState, PXFieldOptions, gridConfig, columnConfig} from 'client-controls';

export class PRDocumentProcessFilter extends PXView  {
	Operation: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	mergeToolbarWith: 'ScreenToolbar',
})
export class PRPayment extends PXView  {
	@columnConfig({ allowCheckAll: true	})
	Selected: PXFieldState;
	RefNbr: PXFieldState;
	DocType: PXFieldState;
	Status: PXFieldState;
	EmployeeID: PXFieldState;
	GrossAmount: PXFieldState;
	DedAmount: PXFieldState;
	TaxAmount: PXFieldState;
}
