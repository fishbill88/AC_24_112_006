import {
	PXView, PXFieldState, gridConfig, selectorSettings, PXFieldOptions,
	columnConfig, GridColumnShowHideMode, PXActionState, commitChanges, GridPagerMode
} from "client-controls";
import {TextAlign} from "client-controls/descriptors/grid-types";

export class SYInsertFrom extends PXView {
	MappingID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class SYMapping extends PXView {
	@commitChanges Name: PXFieldState;
	ScreenID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProviderID: PXFieldState<PXFieldOptions.CommitChanges>;
	@selectorSettings('Name', '')
	ProviderObject: PXFieldState<PXFieldOptions.CommitChanges>;
	SyncType: PXFieldState;
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	Visible: PXFieldState<PXFieldOptions.CommitChanges>;
	IsSimpleMapping: PXFieldState;
	SitemapTitle: PXFieldState;
	WorkspaceID: PXFieldState;
	SubcategoryID: PXFieldState;
	FormatLocale: PXFieldState;
	ProcessInParallel: PXFieldState<PXFieldOptions.CommitChanges>;
	BreakOnError: PXFieldState;
	BreakOnTarget: PXFieldState;
	DiscardResult: PXFieldState;
}

@gridConfig({
	pagerMode: GridPagerMode.InfiniteScroll,
	syncPosition: true,
	adjustPageSize: true,
	fastFilterByAllFields: false
})
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
	@columnConfig({allowNull: false, textAlign: TextAlign.Center})
	IsActive: PXFieldState;
	ObjectNameHidden: PXFieldState;
	ObjectName: PXFieldState;
	FieldNameHidden: PXFieldState;
	FieldName: PXFieldState;
	@columnConfig({allowNull: false, textAlign: TextAlign.Center})
	NeedCommit: PXFieldState;
	@columnConfig({allowNull: false, textAlign: TextAlign.Center})
	IgnoreError: PXFieldState;
	ExecuteActionBehavior: PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.False, visible: false})
	LineNbr: PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.False, visible: false})
	OrderNumber: PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.False, visible: false})
	IsVisible: PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.False, visible: false})
	ParentLineNbr: PXFieldState;
}
export class SYWhatToShow extends PXView {
	WhatToShow: PXFieldState;
}

@gridConfig({adjustPageSize: true, syncPosition: true, initNewRow: true, fastFilterByAllFields: false})
export class SYImportCondition extends PXView {
	@columnConfig({allowNull: false, textAlign: TextAlign.Center})
	IsActive: PXFieldState;
	@columnConfig({allowNull: false})
	OpenBrackets: PXFieldState;
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

@gridConfig({adjustPageSize: true, syncPosition: true, initNewRow: true, fastFilterByAllFields: false})
export class SYMappingCondition extends PXView {
	@columnConfig({allowNull: false, textAlign: TextAlign.Center})
	IsActive: PXFieldState;
	@columnConfig({allowNull: false})
	OpenBrackets: PXFieldState;
	FieldNameHidden: PXFieldState;
	FieldName: PXFieldState<PXFieldOptions.CommitChanges>;
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
