import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, PXFieldOptions, createCollection, columnConfig, linkCommand, PXActionState, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.GL.GLSetupMaint', primaryView: 'GLSetupRecord' })
export class GL102000 extends PXScreen {

	GLSetupRecord = createSingle(GLSetupRecord);

	SetupApproval = createCollection(SetupApproval);

	ViewAssignmentMap: PXActionState;

}

export class GLSetupRecord extends PXView {

	BatchNumberingID: PXFieldState;
	TBImportNumberingID: PXFieldState;
	ScheduleNumberingID: PXFieldState;
	AllocationNumberingID: PXFieldState;
	DocBatchNumberingID: PXFieldState;
	ReuseRefNbrsInVouchers: PXFieldState;

	YtdNetIncAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	RetEarnAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	TrialBalanceSign: PXFieldState;
	COAOrder: PXFieldState;

	AutoRevOption: PXFieldState;
	AutoPostOption: PXFieldState;
	RestrictAccessToClosedPeriods: PXFieldState;
	ConsolidatedPosting: PXFieldState;
	AutoReleaseReclassBatch: PXFieldState<PXFieldOptions.CommitChanges>;

	HoldEntry: PXFieldState;
	VouchersHoldEntry: PXFieldState;
	RequireControlTotal: PXFieldState;
	RequireRefNbrForTaxEntry: PXFieldState;
	DefaultSubID: PXFieldState;

}

@gridConfig({ syncPosition: true })
export class SetupApproval extends PXView {

	IsActive: PXFieldState;
	BatchType: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("ViewAssignmentMap")
	AssignmentMapID: PXFieldState<PXFieldOptions.CommitChanges>;

	AssignmentNotificationID: PXFieldState;

}
