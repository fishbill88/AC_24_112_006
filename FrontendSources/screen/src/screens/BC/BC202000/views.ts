import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
} from "client-controls";

export class EntityFilter extends PXView {
	ConnectorType: PXFieldState<PXFieldOptions.CommitChanges>;
	BindingID: PXFieldState<PXFieldOptions.CommitChanges>;
	EntityType: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CurrentEntity extends PXView {
	IsActive: PXFieldState<PXFieldOptions.Disabled>;
	Direction: PXFieldState<PXFieldOptions.CommitChanges>;
	PrimarySystem: PXFieldState<PXFieldOptions.CommitChanges>;
	MaxAttemptCount: PXFieldState;
	AutoMergeDuplicates: PXFieldState;
	ParallelProcessing: PXFieldState;
	ImportRealTimeStatus: PXFieldState;
	ExportRealTimeStatus: PXFieldState;
	RealTimeMode: PXFieldState;
	RealTimeBaseURL: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	initNewRow: true,
	allowImport: true
})
export class ImportMappings extends PXView {
	SortOrder: PXFieldState;
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	TargetObject: PXFieldState<PXFieldOptions.CommitChanges>;
	TargetField: PXFieldState<PXFieldOptions.CommitChanges>;
	SourceObject: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		editorType: 'qp-formula-editor',
		editorConfig: { comboBox: true }
	})
	SourceField: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	initNewRow: true,
	allowImport: true
})
export class ExportMappings extends PXView {
	SortOrder: PXFieldState;
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	TargetObject: PXFieldState<PXFieldOptions.CommitChanges>;
	TargetField: PXFieldState<PXFieldOptions.CommitChanges>;
	SourceObject: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		editorType: 'qp-formula-editor',
		editorConfig: { comboBox: true }
	})
	SourceField: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	initNewRow: true,
	allowImport: true
})
export class ExportFilters extends PXView {
	SortOrder: PXFieldState;
	@columnConfig({ allowNull: false })
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowNull: false })
	OpenBrackets: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ editorType: "qp-drop-down" })
	FieldName: PXFieldState;
	@columnConfig({ allowNull: false })
	Condition: PXFieldState;
	@columnConfig({ allowNull: false })
	IsRelative: PXFieldState;
	Value: PXFieldState<PXFieldOptions.CommitChanges>;
	Value2: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowNull: false })
	CloseBrackets: PXFieldState;
	@columnConfig({ allowNull: false })
	Operator: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	initNewRow: true,
	allowImport: true
})
export class ImportFilters extends PXView {
	SortOrder: PXFieldState;
	@columnConfig({ allowNull: false })
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowNull: false })
	OpenBrackets: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ editorType: "qp-drop-down" })
	FieldName: PXFieldState;
	@columnConfig({ allowNull: false })
	Condition: PXFieldState;
	@columnConfig({ allowNull: false })
	IsRelative: PXFieldState;
	Value: PXFieldState<PXFieldOptions.CommitChanges>;
	Value2: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowNull: false })
	CloseBrackets: PXFieldState;
	@columnConfig({ allowNull: false })
	Operator: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	initNewRow: true,
})
export class DeleteConfirmationPanel extends PXView {
	EntityName: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	initNewRow: true,
})
export class StartRealTimePanel extends PXView {
	RealTimeURL: PXFieldState<PXFieldOptions.CommitChanges>;
}
