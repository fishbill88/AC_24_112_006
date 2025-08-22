import {
	createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent,
	CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState,
	gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand,
	columnConfig, GridColumnShowHideMode, GridColumnType
} from "client-controls";

@graphInfo({graphType: "PX.Objects.GL.Reclassification.UI.ReclassifyTransactionsProcess", primaryView: "GLTranForReclass" })
export class GL506000 extends PXScreen {

	ReloadTrans: PXActionState;
	LoadTrans: PXActionState;

	ViewReclassBatch: PXActionState;
	ViewDocument: PXActionState;

	GLTranForReclass = createCollection(GLTran);

	@viewInfo({containerName: "Load Transactions"})
	LoadOptionsView = createSingle(LoadOptions);

	@viewInfo({containerName: "Find and Replace"})
	ReplaceOptionsView = createSingle(ReplaceOptions);
}


@gridConfig({ syncPosition: true, mergeToolbarWith: "ScreenToolbar" })
export class GLTran extends PXView {

	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	SplittedIcon: PXFieldState;

	@linkCommand("ViewReclassBatch")
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	ReclassBatchNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	NewBranchID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	NewAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	NewAccountID_Account_description: PXFieldState;

	@columnConfig({ hideViewLink: true })
	NewSubID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	NewProjectID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	NewTaskID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	NewCostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;

	NewTranDate: PXFieldState<PXFieldOptions.CommitChanges>;
	NewTranDesc: PXFieldState;
	CuryNewAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDebitAmt: PXFieldState;
	CuryCreditAmt: PXFieldState;

	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;

	Module: PXFieldState;
	BatchNbr: PXFieldState;
	LineNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState;
	AccountID_Account_description: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState;

	TranDate: PXFieldState;
	FinPeriodID: PXFieldState;

	TranDesc: PXFieldState;
	ReferenceID: PXFieldState;
	@linkCommand("ViewDocument")
	RefNbr: PXFieldState;

}

export class LoadOptions extends PXView {

	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	FromAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ToAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	FromSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	ToSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	FromFinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	ToFinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	FromDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ToDate: PXFieldState<PXFieldOptions.CommitChanges>;
	Module: PXFieldState<PXFieldOptions.CommitChanges>;
	BatchNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	RefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	ReferenceID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	FromTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	ToTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	FromCostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	ToCostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	MaxTrans: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ReplaceOptions extends PXView {

	Warning: PXFieldState;
	WithBranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	WithAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	WithSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	WithProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	WithTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	WithCostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	WithDate: PXFieldState<PXFieldOptions.CommitChanges>;
	WithFinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	WithTranDescFilteringValue: PXFieldState<PXFieldOptions.CommitChanges>;
	NewBranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	NewAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	NewSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	NewProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	NewTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	NewCostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	NewDate: PXFieldState<PXFieldOptions.CommitChanges>;
	NewTranDesc: PXFieldState<PXFieldOptions.CommitChanges>;
}
