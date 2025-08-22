import { createCollection, PXScreen, graphInfo, PXView, PXFieldState, gridConfig, columnConfig } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CM.TranslationRelease', primaryView: 'TranslationReleaseList', })
export class CM502000 extends PXScreen {

	TranslationReleaseList = createCollection(TranslationHistory);

}

@gridConfig({ syncPosition: true, mergeToolbarWith: 'ScreenToolbar' })
export class TranslationHistory extends PXView {

	Selected: PXFieldState;
	ReferenceNbr: PXFieldState;
	Description: PXFieldState;
	TranslDefId: PXFieldState;
	BranchID: PXFieldState;
	LedgerID: PXFieldState;
	DateEntered: PXFieldState;
	FinPeriodID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Status: PXFieldState;

}
