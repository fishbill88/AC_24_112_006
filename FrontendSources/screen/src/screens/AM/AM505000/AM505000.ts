import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	columnConfig,
	gridConfig,
	GridPreset,
} from 'client-controls';

export class MrpProcessingSetup extends PXView {
	LastMrpRegenCompletedDateTime: PXFieldState;
	LastMrpRegenCompletedByID: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
})
export class AMRPAuditTable extends PXView {
	Recno: PXFieldState;
	CreatedDateTime: PXFieldState;
	@columnConfig({ width: 1000 }) MsgText: PXFieldState;
	CreatedByScreenID: PXFieldState;
	CreatedByID: PXFieldState;
	ProcessID: PXFieldState;
	MsgType: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.Fullregen', primaryView: 'MrpProcessing' })
export class AM505000 extends PXScreen {
	MrpProcessing = createSingle(MrpProcessingSetup);
	AuditDetailRecs = createCollection(AMRPAuditTable);
}
