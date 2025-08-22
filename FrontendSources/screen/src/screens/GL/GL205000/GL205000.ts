import {
	createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType,
	RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, treeConfig, headerDescription,
	ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode,
	GridColumnType
} from "client-controls";

@graphInfo({ graphType: "PX.Objects.GL.GLBudgetTreeMaint", primaryView: "Details" })
export class GL205000 extends PXScreen {

	Delete: PXActionState;

	@viewInfo({ containerName: "Budget Tree" })
	Tree = createCollection(GLBudgetTree);
	@viewInfo({ containerName: "Subarticles" })
	Details = createCollection(GLBudgetTree2);

	@viewInfo({ containerName: "Preload Accounts" })
	PreloadFilter = createSingle(AccountsPreloadFilter);
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
	openedLayers: 4,
	singleClickSelect: true,
	selectFirstNode: true,
	syncPosition: true
})
export class GLBudgetTree extends PXView {

	DeleteGroup: PXActionState;
	Left: PXActionState;
	Right: PXActionState;
	Up: PXActionState;
	Down: PXActionState;

	GroupID: PXFieldState;
	Description: PXFieldState;
	ParentGroupID: PXFieldState;
}

@gridConfig({ syncPosition: true })
export class GLBudgetTree2 extends PXView {

	configureSecurity: PXActionState;
	showPreload: PXActionState;

	IsGroup: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState;

	Description: PXFieldState;
	AccountMask: PXFieldState;
	SubMask: PXFieldState;
	Secured: PXFieldState;
}

export class AccountsPreloadFilter extends PXView {
	fromAccount: PXFieldState<PXFieldOptions.CommitChanges>;
	toAccount: PXFieldState<PXFieldOptions.CommitChanges>;
	SubIDFilter: PXFieldState;
}
