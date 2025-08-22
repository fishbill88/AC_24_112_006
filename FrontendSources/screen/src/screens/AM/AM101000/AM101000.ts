import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	GridPreset,
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AM.BOMSetup', primaryView: 'AMBSetupRecord' })
export class AM101000 extends PXScreen {
	AMBSetupRecord = createSingle(AMBSetupRecord);
	ECRSetupApproval = createCollection(ECRSetupApproval);
	ECOSetupApproval = createCollection(ECOSetupApproval);
}

export class AMBSetupRecord extends PXView {
	BOMNumberingID: PXFieldState;
	ECRNumberingID: PXFieldState;
	ECONumberingID: PXFieldState;
	DefaultRevisionID: PXFieldState;
	DuplicateItemOnBOM: PXFieldState;
	DuplicateItemOnOper: PXFieldState;
	WcID: PXFieldState;
	OperationTimeFormat: PXFieldState;
	ProductionTimeFormat: PXFieldState;
	AllowEmptyBOMSubItemID: PXFieldState;
	ForceECR: PXFieldState;
	RequireECRBeforeECO: PXFieldState;
	BOMHoldRevisionsOnEntry: PXFieldState;
	AllowArchiveWithoutUpdatePending: PXFieldState;
	AutoArchiveWhenUpdatePending: PXFieldState;
	DefaultQueueTime: PXFieldState;
	DefaultFinishTime: PXFieldState;
	DefaultMoveTime: PXFieldState;
	ECRRequestApproval: PXFieldState<PXFieldOptions.CommitChanges>;
	ECORequestApproval: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class ECRSetupApproval extends PXView {
	AssignmentMapID: PXFieldState<PXFieldOptions.CommitChanges>;
	AssignmentNotificationID: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class ECOSetupApproval extends PXView {
	AssignmentMapID: PXFieldState<PXFieldOptions.CommitChanges>;
	AssignmentNotificationID: PXFieldState;
}
