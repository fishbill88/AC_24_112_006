import { createCollection, createSingle, PXScreen, graphInfo,
	PXActionState, viewInfo, PXPageLoadBehavior, PXView,
	PXFieldState, PXFieldOptions, gridConfig, columnConfig,
	TextAlign, GridColumnType, treeConfig, GridPreset } from "client-controls";

@graphInfo({graphType: "PX.Objects.EP.EPAssignmentMaint", primaryView: "AssigmentMap", pageLoadBehavior: PXPageLoadBehavior.InsertRecord})
export class EP205000 extends PXScreen {

   	@viewInfo({containerName: "Assignment Rules Summary"})
	AssigmentMap = createSingle(EPAssignmentMap);
   	@viewInfo({containerName: "Tree"})
	Nodes = createCollection(EPAssignmentRoute);
   	@viewInfo({containerName: "Rules"})
	Items = createCollection(EPAssignmentRoute2);
   	@viewInfo({containerName: "Rules"})
	CurrentItem = createSingle(EPAssignmentRoute3);
   	@viewInfo({containerName: "Conditions"})
	Rules = createCollection(EPAssignmentRule);
}

export class EPAssignmentMap extends PXView  {
	AssignmentMapID : PXFieldState;
	Name : PXFieldState<PXFieldOptions.CommitChanges>;
	GraphType : PXFieldState<PXFieldOptions.CommitChanges>;
}

@treeConfig({
	dynamic: true,
	hideRootNode: true,
	dataMember: 'Nodes',
	idParent: 'Key',
	idName: 'AssignmentRouteID',
	description: 'Name',
	modifiable: false,
	mode: 'single',
	singleClickSelect: true,
	syncPosition: true
})
export class EPAssignmentRoute extends PXView  {
	Key: PXFieldState;
	AssignmentRouteID: PXFieldState;
	Name: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
	autoRepaint: ['Rules'],
	fastFilterByAllFields: false,
	topBarItems: {
		Up: {index: 0, config: {commandName: "Up", text: "Up", images: { normal: 'main@ArrowUp'}}},
		Down: {index: 1, config: {commandName: "Down", text: "Down", images: { normal: 'main@ArrowDown'}}},
	},
	actionsConfig: {
		exportToExcel: { hidden: true }
	},
})
export class EPAssignmentRoute2 extends PXView  {
	Up : PXActionState;
	Down : PXActionState;
	@columnConfig({textAlign: TextAlign.Right})	Sequence : PXFieldState;
	RouterType : PXFieldState;
	Name : PXFieldState;
	@columnConfig({textAlign: TextAlign.Left})	RouteID : PXFieldState;
	@columnConfig({
		textAlign: TextAlign.Left,
		editorType: 'qp-tree-selector',
		hideViewLink: true,
		editorConfig: {
			treeSelectorId: 'tree_selector_edWorkgroupID',
			treeConfig: {
				idName: 'WorkgroupID',
				dataMember: '_EPCompanyTree_Tree_',
				description: 'Description',
				idParent: 'Key',
				iconField: 'Icon',
				mode: 'single',
				hideRootNode: true
			},
		}
	})
	WorkgroupID : PXFieldState;
	WaitTime : PXFieldState;
	OwnerID : PXFieldState;
	EPEmployee__acctName : PXFieldState<PXFieldOptions.Hidden>;
	EPEmployee__departmentID : PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({
		hideViewLink: true,
		editorType: 'qp-tree-selector',
		editorConfig: {
			treeSelectorId: 'tree_selector_edownersource',
			treeConfig: {
				idName: 'Path',
				dataMember: 'EntityItems',
				description: 'Name',
				idParent: 'Key',
				iconField: 'Icon',
				mode: 'single',
				hideRootNode: true
			},
		},
	})
	OwnerSource : PXFieldState;
	@columnConfig({type: GridColumnType.CheckBox})	UseWorkgroupByOwner : PXFieldState;
}

export class EPAssignmentRoute3 extends PXView  {
	RuleType : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
	fastFilterByAllFields: false,
	actionsConfig: {
		exportToExcel: { hidden: true }
	},
})
export class EPAssignmentRule extends PXView  {
	Entity : PXFieldState;
	FieldName : PXFieldState;
	Condition : PXFieldState;
	FieldValue : PXFieldState;
}
