import {
	PXView, PXFieldState, gridConfig, PXFieldOptions,
	linkCommand, columnConfig, GridColumnType, commitChanges, TextAlign, PXActionState, localizable
} from "client-controls";

@localizable
class Message {
	static NEWPlaceholder = "<NEW>";
}

export class ActionExecution extends PXView {
	@columnConfig({nullText: Message.NEWPlaceholder})
	@commitChanges ExecutionID: PXFieldState;
	@columnConfig({allowUpdate: false})
	Name: PXFieldState;
	ActionScreenID: PXFieldState<PXFieldOptions.CommitChanges>;
	ActionName: PXFieldState;
	ScreenID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowCreatedByEventsTabExpr: PXFieldState;
}

@gridConfig({syncPosition: true, allowDelete: false, allowInsert: false, allowUpdate: true, fastFilterByAllFields: false})
export class ActionExecutionMapping extends PXView {
	DisplayFieldName: PXFieldState;
	@columnConfig({type: GridColumnType.CheckBox, textAlign: TextAlign.Center, allowNull: false })
	FromSchema: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({key: "valueMapping", allowSort: false, editorConfig: {allowCustomItems: true}})
	Value: PXFieldState;
}

@gridConfig({initNewRow: true, syncPosition: true, allowInsert: true, allowUpdate: true, allowDelete: true, fastFilterByAllFields: false})
export class ActionExecutionParameter extends PXView {
	ObjectName: PXFieldState<PXFieldOptions.CommitChanges>;
	FieldName: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowNull: false, textAlign: TextAlign.Center, type: GridColumnType.CheckBox})
	FromSchema: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({key: "valueParameter", allowSort: false, editorConfig: {allowCustomItems: true}})
	Value: PXFieldState;
}

@gridConfig({syncPosition: true, allowUpdate: false, fastFilterByAllFields: false})
export class BPEvent extends PXView {
	createBusinessEvent: PXActionState;

	@linkCommand('ViewBusinessEvent')
	Name: PXFieldState;
	Description: PXFieldState;
	@columnConfig({type: GridColumnType.CheckBox})
	Active: PXFieldState;
	Type: PXFieldState;
}

export class BPEventData extends PXView {
	Name: PXFieldState<PXFieldOptions.CommitChanges>;
}
