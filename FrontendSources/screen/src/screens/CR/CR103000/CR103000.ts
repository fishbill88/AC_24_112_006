import {
	PXView,
	PXFieldState,
	graphInfo,
	PXScreen,
	createCollection,
	createSingle,
	PXFieldOptions,
	treeConfig,
	PXPageLoadBehavior,
	PXActionState,
	gridConfig,
	GridPreset,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CR.CRDuplicateValidationSetupMaint",
	primaryView: "Setup",
	pageLoadBehavior: PXPageLoadBehavior.InsertRecord,
})
export class CR103000 extends PXScreen {
	CurrentNode = createSingle(Node);
	ValidationRules = createCollection(ValidationRules);
	Nodes = createCollection(Nodes);
}

@treeConfig({
	dynamic: true,
	hideRootNode: true,
	dataMember: 'Nodes',
	idParent: 'Key',
	idName: 'ID',
	description: 'Description',
	modifiable: false,
	mode: 'single',
	singleClickSelect: true,
	selectFirstNode: true,
	syncPosition: true,
	hideToolbarSearch: true,
})
export class Nodes extends PXView {
	Key: PXFieldState;
	ID: PXFieldState;
	Description: PXFieldState;
}

export class Node extends PXView {
	ValidationThreshold: PXFieldState;
	ValidateOnEntry: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	fastFilterByAllFields: false,
	suppressNoteFiles: true,
	topBarItems: {
		Copy: {
			index: 3,
			config: {
				commandName: "Copy",
				images: {
					normal: "main@Copy"
				},
			},
		},
		Paste: {
			index: 4,
			config: {
				commandName: "Paste",
				images: {
					normal: "main@Paste"
				},
			},
		},
	},
	actionsConfig: {
		exportToExcel: { visibleOnToolbar: true },
	}
})
export class ValidationRules extends PXView {
	Copy: PXActionState;
	Paste: PXActionState;

	ValidationType: PXFieldState;
	MatchingFieldUI: PXFieldState<PXFieldOptions.CommitChanges>;
	ScoreWeight: PXFieldState<PXFieldOptions.CommitChanges>;
	TransformationRule: PXFieldState<PXFieldOptions.CommitChanges>;
	CreateOnEntry: PXFieldState<PXFieldOptions.CommitChanges>;
}