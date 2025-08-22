import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, handleEvent,
	CustomEventType, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState,
	gridConfig, PXFieldOptions, linkCommand, columnConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.GL.Reclassification.UI.ReclassificationHistoryInq',
	primaryView: 'TransView' })
export class GL405000 extends PXScreen {

	ViewOrigBatch: PXActionState;
	ViewBatch: PXActionState;

	TransView = createCollection(GLTran);
}

@gridConfig({ syncPosition: true, allowDelete: false, allowInsert: false, mergeToolbarWith: 'ScreenToolbar' })
export class GLTran extends PXView {

	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("ViewOrigBatch")
	OrigBatchNbr: PXFieldState;

	ActionDesc: PXFieldState;

	SplitIcon: PXFieldState;

	@linkCommand("ViewBatch")
	BatchNbr: PXFieldState;

	LineNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState;

	AccountID_Account_description: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ProjectID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	TaskID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState;

	CuryDebitAmt: PXFieldState;
	CuryCreditAmt: PXFieldState;
	CuryReclassRemainingAmt: PXFieldState;
	TranDesc: PXFieldState;
	TranDate: PXFieldState;
	FinPeriodID: PXFieldState;
	ReclassSeqNbr: PXFieldState;
}
