import {
	PXScreen, createSingle, graphInfo, viewInfo, PXView, PXFieldState, PXFieldOptions
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.DR.DeferredCodeMaint', primaryView: 'deferredcode' })
export class DR202000 extends PXScreen {
	@viewInfo({ containerName: 'Deferral Code' })
	DeferredCode = createSingle(DRDeferredCode);
}

export class DRDeferredCode extends PXView {
	DeferredCodeID: PXFieldState;
	Description: PXFieldState;
	Active: PXFieldState;
	MultiDeliverableArrangement: PXFieldState<PXFieldOptions.CommitChanges>;
	Method: PXFieldState<PXFieldOptions.CommitChanges>;
	RecognizeInPastPeriods: PXFieldState;
	ReconNowPct: PXFieldState;
	StartOffset: PXFieldState;
	Occurrences: PXFieldState;
	AccountType: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountSource: PXFieldState<PXFieldOptions.CommitChanges>;
	DeferralSubMaskAR: PXFieldState;
	DeferralSubMaskAP: PXFieldState;
	CopySubFromSourceTran: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;
	Frequency: PXFieldState;
	Periods: PXFieldState<PXFieldOptions.Disabled>;
	ScheduleOption: PXFieldState<PXFieldOptions.CommitChanges>;
	FixedDay: PXFieldState;
}
