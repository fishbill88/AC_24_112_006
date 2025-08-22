import {
	createCollection, createSingle,
	PXScreen, PXActionState, PXView, PXFieldState, PXFieldOptions,
	graphInfo, linkCommand, gridConfig, columnConfig
} from 'client-controls';

@graphInfo({
	graphType: 'PX.Objects.AR.ExternalTransactionValidation', primaryView: 'Filter',
	hideFilesIndicator: true, hideNotesIndicator: true
})
export class AR513000 extends PXScreen {
	ViewDocument: PXActionState;
	ViewProcessingCenter: PXActionState;

	Filter = createSingle(ExternalTransactionFilter);
	PaymentTrans = createCollection(ExternalTransaction);
}

export class ExternalTransactionFilter extends PXView {
	ProcessingCenterID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	mergeToolbarWith: 'ScreenToolbar',
	quickFilterFields: ['DocType', 'RefNbr']
})
export class ExternalTransaction extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;

	DocType: PXFieldState;

	@linkCommand('ViewDocument')
	RefNbr: PXFieldState;

	ProcStatus: PXFieldState;
	LastActivityDate: PXFieldState;
	TranNumber: PXFieldState;

	@linkCommand('ViewProcessingCenter')
	ProcessingCenterID: PXFieldState;

	SaveProfile: PXFieldState;
	NeedSync: PXFieldState;
}
