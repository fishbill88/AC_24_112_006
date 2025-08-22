import { createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, PXActionState } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CM.TranslationEnq', primaryView: 'Filter', })
export class CM401000 extends PXScreen {

	viewTranslatedBatch: PXActionState;
	ViewDetails: PXActionState;

	Filter = createSingle(TranslationEnqFilter);
	TranslationHistoryRecords = createCollection(TranslationHistory);

}

export class TranslationEnqFilter extends PXView {
	TranslDefId: PXFieldState<PXFieldOptions.CommitChanges>;
	Unreleased: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	Released: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ syncPosition: true, mergeToolbarWith: 'ScreenToolbar' })
export class TranslationHistory extends PXView {

	@linkCommand('ViewDetails')
	ReferenceNbr: PXFieldState;

	Status: PXFieldState;
	Description: PXFieldState;
	DateEntered: PXFieldState;
	TranslDefId: PXFieldState;
	BranchID: PXFieldState;
	LedgerID: PXFieldState;
	FinPeriodID: PXFieldState;
	CuryEffDate: PXFieldState;

	@linkCommand('viewTranslatedBatch')
	BatchNbr: PXFieldState;

}
