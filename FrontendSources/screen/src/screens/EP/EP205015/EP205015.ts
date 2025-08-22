import { createCollection, createSingle, PXScreen, graphInfo,
	PXActionState, viewInfo, PXPageLoadBehavior, treeConfig,
	PXFieldState, PXFieldOptions, PXView, gridConfig,
	TextAlign, GridColumnType, columnConfig, localizable, GridColumnDisplayMode, GridPreset, } from "client-controls";

@localizable
class Labels {
	static Insert = "Insert";
}

@graphInfo({
	graphType: "PX.Objects.EP.EPApprovalMapMaint",
	primaryView: "AssigmentMap",
	pageLoadBehavior: PXPageLoadBehavior.InsertRecord,
})
export class EP205015 extends PXScreen {
	Up: PXActionState;
	Down: PXActionState;
	DeleteRoute: PXActionState;

	@viewInfo({ containerName: "Assignment Rules Summary" })
	AssigmentMap = createSingle(EPAssignmentMap);
	@viewInfo({ containerName: "Steps" })
	NodesTree = createCollection(EPRule);
	CurrentNode = createSingle(EPRule2);
	@viewInfo({ containerName: "Conditions" })
	Rules = createCollection(EPRuleCondition);
	@viewInfo({ containerName: "Rule Actions" })
	EmployeeCondition = createCollection(EPRuleEmployeeCondition);
	RuleApprovers = createCollection(EPRuleApprover);
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
	topBarItems: {
		AddStep: { config: { commandName: "AddStep", text: "Add Step" }},
		AddRule: { config: { commandName: "AddRule", images: {normal: "main@RecordAdd"} } },
		up: { config: { commandName: 'Up', text: '', images: { normal: 'main@ArrowUp' } } },
		down: { config: { commandName: 'Down', text: '', images: { normal: 'main@ArrowDown' } } },
		DeleteRoute: {config: {commandName: 'DeleteRoute', images: {normal: 'main@RecordDel' } } }
	}
})
export class EPRule extends PXView {
	AddStep: PXActionState;
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
	EmptyStepType: PXFieldState<PXFieldOptions.CommitChanges>;
	ExecuteStep: PXFieldState;
	RuleType: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerSource: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowReassignment: PXFieldState<PXFieldOptions.CommitChanges>;
	WaitTime: PXFieldState;
	ApproveType: PXFieldState<PXFieldOptions.CommitChanges>;
	ReasonForApprove: PXFieldState;
	ReasonForReject: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
	fastFilterByAllFields: false,
	adjustPageSize: true,
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
	@columnConfig({ allowSort: false })
	Entity: PXFieldState;
	@columnConfig({ allowSort: false })
	FieldName: PXFieldState;
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

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
	fastFilterByAllFields: false,
})
export class EPRuleEmployeeCondition extends PXView {
	@columnConfig({ allowNull: false })
	OpenBrackets: PXFieldState;
	Entity: PXFieldState;
	FieldName: PXFieldState;
	@columnConfig({ allowNull: false })
	Condition: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	IsField: PXFieldState;
	Value: PXFieldState;
	Value2: PXFieldState;
	@columnConfig({ allowNull: false })
	CloseBrackets: PXFieldState;
	@columnConfig({ allowNull: false })
	Operator: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class EPRuleApprover extends PXView {
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CREmployee__AcctCD: PXFieldState;
	Contact__Salutation: PXFieldState;
	CREmployee__DepartmentID: PXFieldState;
}
