import {
	createCollection, createSingle, PXScreen, graphInfo, viewInfo, handleEvent, CustomEventType,
	RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig,
	headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig,
	GridColumnShowHideMode, GridColumnType, PXActionState, treeConfig
} from "client-controls";


@graphInfo({graphType: "PX.Objects.GL.GLBudgetEntry", primaryView: "Filter", pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues, showUDFIndicator: true })
export class GL302010 extends PXScreen {
	distributeOK: PXActionState;
	manageOK: PXActionState;
	preload: PXActionState;


	@viewInfo({containerName: "Budget Filter"})
	Filter = createSingle(BudgetFilter);
	Tree = createCollection(GLBudgetLine);
	@viewInfo({containerName: "Budget Articles"})
	BudgetArticles = createCollection(GLBudgetLine2);

	@viewInfo({containerName: "Dispose Parameters"})
	DistrFilter = createSingle(BudgetDistributeFilter);
	@viewInfo({containerName: "Manage Budget"})
	ManageDialog = createSingle(ManageBudgetDialog);
	@viewInfo({containerName: "Preload Budget Articles Wizard"})
	PreloadFilter = createSingle(BudgetPreloadFilter);
}

export class BudgetFilter extends PXView {

	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	LedgerId: PXFieldState<PXFieldOptions.CommitChanges>;
	FinYear: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowTree: PXFieldState<PXFieldOptions.CommitChanges>;
	CompareToBranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	CompareToLedgerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CompareToFinYear: PXFieldState<PXFieldOptions.CommitChanges>;
	SubIDFilter: PXFieldState<PXFieldOptions.CommitChanges>;
	TreeNodeFilter: PXFieldState<PXFieldOptions.CommitChanges>;
}


@treeConfig({
	dynamic: true,
	hideRootNode: true,
	dataMember: "Tree",
	idParent: "ParentGroupID",
	idName: "GroupID",
	description: "Description",
	modifiable: false,
	mode: "single",
	singleClickSelect: true,
	selectFirstNode: true,
	syncPosition: true
})
export class GLBudgetLine extends PXView {

	GroupID: PXFieldState;
	Description: PXFieldState;
	ParentGroupID: PXFieldState;
}

@gridConfig({ syncPosition: true })
export class GLBudgetLine2 extends PXView {

	distribute: PXActionState;

	IsGroup: PXFieldState;
	Released: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState;

	@columnConfig({ allowSort: false })
	Description: PXFieldState;

	Amount: PXFieldState;
	AllocatedAmount: PXFieldState;

	@columnConfig({ hideViewLink: true, allowShowHide: GridColumnShowHideMode.True })
	CreatedByID: PXFieldState;

	@columnConfig({ hideViewLink: true, allowShowHide: GridColumnShowHideMode.True })
	LastModifiedByID: PXFieldState;
}

export class BudgetDistributeFilter extends PXView {

	Method: PXFieldState;
	ApplyToAll: PXFieldState<PXFieldOptions.CommitChanges>;
	ApplyToSubGroups: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ManageBudgetDialog extends PXView {

	Method: PXFieldState<PXFieldOptions.CommitChanges>;
	Message: PXFieldState;
}

export class BudgetPreloadFilter extends PXView {

	branchID: PXFieldState<PXFieldOptions.CommitChanges>;
	ledgerID: PXFieldState<PXFieldOptions.CommitChanges>;
	finYear: PXFieldState<PXFieldOptions.CommitChanges>;
	changePercent: PXFieldState<PXFieldOptions.CommitChanges>;
	fromAccount: PXFieldState<PXFieldOptions.CommitChanges>;
	toAccount: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountIDFilter: PXFieldState<PXFieldOptions.CommitChanges>;
	SubIDFilter: PXFieldState<PXFieldOptions.CommitChanges>;
	PreloadAction: PXFieldState<PXFieldOptions.CommitChanges>;
}
