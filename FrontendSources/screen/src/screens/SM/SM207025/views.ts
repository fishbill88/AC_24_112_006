import {
	columnConfig, commitChanges,
	GridColumnShowHideMode,
	gridConfig, GridPagerMode, PXActionState,
	PXFieldOptions,
	PXFieldState,
	PXView,
	selectorSettings,
	TextAlign
} from "client-controls";

export class SYInsertFrom extends PXView {
	MappingID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class SYMapping extends PXView {
	@commitChanges Name: PXFieldState;
	ScreenID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProviderID: PXFieldState<PXFieldOptions.CommitChanges>;
	@selectorSettings('Name', '')
	ProviderObject: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowNull: false})
	SyncType: PXFieldState;
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	Visible: PXFieldState<PXFieldOptions.CommitChanges>;
	SitemapTitle: PXFieldState;
	WorkspaceID: PXFieldState;
	SubcategoryID: PXFieldState;
	FormatLocale: PXFieldState;
	@columnConfig({allowNull: false})
	RepeatingData: PXFieldState;
	IsExportOnlyMappingFields: PXFieldState;
	DiscardResult: PXFieldState;
}

@gridConfig({pagerMode: GridPagerMode.InfiniteScroll, autoAdjustColumns: true, adjustPageSize: true, syncPosition: true, initNewRow: true, allowUpdate: true})
export class SYMappingField extends PXView {
	rowInsert: PXActionState;
	rowUp: PXActionState;
	rowDown: PXActionState;
	insertFrom: PXActionState;
	viewSubstitutions: PXActionState;

	@columnConfig({
		editorType: 'qp-formula-editor',
		editorConfig: {
			comboBox: true,
			applicationFunctions: false,
			programFunctions: [
				'IIf( expr, truePart, falsePart )',
				'IsNull( value, nullValue )',
				'NullIf( value1, value2 )',
				'SubstituteAll( sourceField, substitutionList )',
				'SubstituteListed( sourceField, substitutionList )',
				'Switch( expr1, value1, expr2, value2, ...)'
			]
		}
	})
	Value: PXFieldState;
	@columnConfig({
		editorType: 'qp-formula-editor',
		editorConfig: {
			comboBox: true,
			applicationFunctions: false,
			programFunctions: [
				'IIf( expr, truePart, falsePart )',
				'IsNull( value, nullValue )',
				'NullIf( value1, value2 )',
				'SubstituteAll( sourceField, substitutionList )',
				'SubstituteListed( sourceField, substitutionList )',
				'Switch( expr1, value1, expr2, value2, ...)'
			]
		}
	})
	FieldName: PXFieldState;
	@columnConfig({allowNull: false, textAlign: TextAlign.Center})
	IsActive: PXFieldState;
	ObjectNameHidden: PXFieldState;
	ObjectName: PXFieldState;
	FieldNameHidden: PXFieldState;
	@columnConfig({allowNull: false, textAlign: TextAlign.Center})
	NeedCommit: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowNull: false, textAlign: TextAlign.Center})
	IgnoreError: PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.False, suppressExport: false})
	LineNbr: PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.False, suppressExport: false})
	IsVisible: PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.False, suppressExport: false})
	ParentLineNbr: PXFieldState;
}

export class SYWhatToShow extends PXView {
	WhatToShow: PXFieldState;
}

@gridConfig({ adjustPageSize: true, initNewRow: true})
export class SYMappingCondition extends PXView {
	@columnConfig({allowNull: false, textAlign: TextAlign.Center})
	IsActive: PXFieldState;
	@columnConfig({allowNull: false})
	OpenBrackets: PXFieldState;
	ObjectNameHidden: PXFieldState;
	ObjectName: PXFieldState;
	FieldNameHidden: PXFieldState;
	FieldName: PXFieldState;
	@columnConfig({allowNull: false})
	Condition: PXFieldState;
	@columnConfig({allowNull: false, textAlign: TextAlign.Center})
	IsRelative: PXFieldState;
	Value: PXFieldState;
	Value2: PXFieldState;
	@columnConfig({allowNull: false})
	CloseBrackets: PXFieldState;
	@columnConfig({allowNull: false})
	Operator: PXFieldState;
}
