import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, PXFieldOptions
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.DR.DRSetupMaint', primaryView: 'DRSetupRecord' })
export class DR101000 extends PXScreen {
	DRSetupRecord = createSingle(DRSetup);
}

export class DRSetup extends PXView {
	ScheduleNumberingID: PXFieldState;
	SuspenseAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SuspenseSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	UseFairValuePricesInBaseCurrency: PXFieldState;
	RecognizeAdjustmentsInPreviousPeriods: PXFieldState;
}
