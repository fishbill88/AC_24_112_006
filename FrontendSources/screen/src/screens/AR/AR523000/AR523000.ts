import {
	createCollection, createSingle,
	PXScreen, PXActionState, PXView, PXFieldState, PXFieldOptions,
	graphInfo, viewInfo, gridConfig, columnConfig, linkCommand
} from 'client-controls';

@graphInfo({
	graphType: 'PX.Objects.AR.ARCustomerCreditHoldProcess', primaryView: 'Filter',
	hideFilesIndicator: true, hideNotesIndicator: true
})
export class AR523000 extends PXScreen {
	ViewDocument: PXActionState;

	@viewInfo({containerName: 'Selection'})
	Filter = createSingle(CreditHoldParameters);

	@viewInfo({containerName: 'Customers'})
	Details = createCollection(DetailsResult);

}

export class CreditHoldParameters extends PXView {
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	BeginDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowAll: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	mergeToolbarWith: 'ScreenToolbar',
	quickFilterFields: ['CustomerId', 'CustomerId_description']
})
export class DetailsResult extends PXView {
	@columnConfig({ allowNull: false, allowCheckAll: true })
	Selected: PXFieldState;

	@linkCommand('ViewDocument')
	CustomerId: PXFieldState;

	CustomerId_description: PXFieldState;
	DunningLetterDate: PXFieldState;
	DocBal: PXFieldState;
	InvBal: PXFieldState;
	Status: PXFieldState;
}
