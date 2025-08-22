import {
	PXScreen, graphInfo, PXView, PXFieldState, PXFieldOptions, createCollection, columnConfig, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CA.EntryTypeMaint', primaryView: 'EntryType' })
export class CA203000 extends PXScreen {
	EntryType = createCollection(CAEntryType);
}

@gridConfig({ syncPosition: true, mergeToolbarWith: 'ScreenToolbar', quickFilterFields: ['EntryTypeId', 'Descr'] })
export class CAEntryType extends PXView {
	EntryTypeId: PXFieldState;
	DrCr: PXFieldState;
	Descr: PXFieldState;
	Module: PXFieldState;
	ReferenceID: PXFieldState;
	BranchID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState;

	UseToReclassifyPayments: PXFieldState;
	CashAccountID: PXFieldState;
	Consolidate: PXFieldState;
}
