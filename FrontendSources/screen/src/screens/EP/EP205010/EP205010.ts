import { PXView, PXFieldState, gridConfig,
	PXFieldOptions, columnConfig, GridColumnType,
	PXActionState, TextAlign, graphInfo,
	PXScreen, viewInfo, createSingle, localizable,
	createCollection, PXPageLoadBehavior, treeConfig, GridPreset, GridColumnDisplayMode } from "client-controls";

@localizable
class Labels {
	static Insert = "Insert";
}

@graphInfo({ graphType: "PX.Objects.EP.EPAssignmentMapMaint", primaryView: "AssigmentMap", pageLoadBehavior: PXPageLoadBehavior.InsertRecord })
export class EP205010 extends PXScreen {
	Up: PXActionState;
	Down: PXActionState;
	DeleteRoute: PXActionState;

	@viewInfo({ containerName: "Assignment Rules Summary" })
	AssigmentMap = createSingle(EPAssignmentMap);
	@viewInfo({ containerName: "Rules" })
	NodesTree = createCollection(EPRule);
	@viewInfo({ containerName: "Conditions" })
	CurrentNode = createSingle(EPRule2);
	@viewInfo({ containerName: "Conditions" })
	Rules = createCollection(EPRuleCondition);
}

export class EPAssignmentMap extends PXView {
	@columnConfig({ displayMode: GridColumnDisplayMode.Text })
	AssignmentMapID: PXFieldState<PXFieldOptions.CommitChanges>;
	Name: PXFieldState<PXFieldOptions.CommitChanges>;
	GraphType: PXFieldState<PXFieldOptions.CommitChanges>;
}

@treeConfig({
	dynamic: true,
	hideRootNode: true,
	dataMember: 'NodesTree',
	idParent: 'Key',
	idName: 'RuleID',
	description: 'ExtName',
	modifiable: false,
	mode: 'single',
	singleClickSelect: true,
	selectFirstNode: true,
	syncPosition: true,
	hideToolbarSearch: true,
})
export class EPRule extends PXView {
	AddRule: PXActionState;
	Up: PXActionState;
	Down: PXActionState;
	DeleteRoute: PXActionState;

	Key: PXFieldState;
	RuleID: PXFieldState;
	ExtName: PXFieldState;
}

export class EPRule2 extends PXView {
	Name: PXFieldState<PXFieldOptions.CommitChanges>;
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	RuleType: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerSource: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
	adjustPageSize: true,
	fastFilterByAllFields: false,
	topBarItems: {
		ConditionInsert: { config: { commandName: 'ConditionInsert', text: Labels.Insert } },
		ConditionUp: { config: { commandName: 'ConditionUp', text: '', images: { normal: 'main@ArrowUp' } } },
		ConditionDown: { config: { commandName: 'ConditionDown', text: '', images: { normal: 'main@ArrowDown' } } },
	},
	actionsConfig: {
		exportToExcel: { hidden: true },
	},
})
export class EPRuleCondition extends PXView {
	ConditionInsert: PXActionState;
	ConditionUp: PXActionState;
	ConditionDown: PXActionState;

	@columnConfig({ allowNull: false, textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	IsActive: PXFieldState;
	@columnConfig({ allowSort: false, allowNull: false })
	OpenBrackets: PXFieldState;
	@columnConfig({ allowSort: false, fullState: true })
	Entity: PXFieldState;
	@columnConfig({ allowSort: false })
	FieldName: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowSort: false, allowNull: false })
	Condition: PXFieldState;
	@columnConfig({ allowSort: false })
	Value: PXFieldState;
	@columnConfig({ allowSort: false })
	Value2: PXFieldState;
	@columnConfig({ allowSort: false, allowNull: false })
	CloseBrackets: PXFieldState;
	@columnConfig({ allowSort: false, allowNull: false })
	Operator: PXFieldState;
}
