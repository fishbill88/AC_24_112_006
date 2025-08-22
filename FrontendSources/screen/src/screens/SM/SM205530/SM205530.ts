import {
	PXScreen,
	PXView,
	PXFieldState,
	graphInfo,
	createSingle,
	createCollection,
	gridConfig,
	GridColumnGeneration,
	viewInfo,
	PXFieldOptions,
	IGridColumn,
	GridPreset
} from "client-controls";

@graphInfo({graphType: 'PX.SM.AUAuditInquire', primaryView: 'Filter'})
export class SM205530 extends PXScreen {
	@viewInfo({containerName: 'Filter'})
	Filter = createSingle(AUAuditFilter);

	@viewInfo({containerName: 'Records'})
	Keys = createCollection(AUAuditKeys);

	@viewInfo({containerName: 'Events'})
	Changes = createCollection(AUAuditValues);

	private VIRTUAL_FIELD_SUFFIX = "VIRTUAL_FIELD";
	private FORBIDDEN_COLUMNS = ["CombinedKey", "Num"];

	onFilterColumns(col: IGridColumn) {
		return col.field?.endsWith(this.VIRTUAL_FIELD_SUFFIX) || (!col.generated && !this.FORBIDDEN_COLUMNS.includes(col.field));
	}
}

export class AUAuditFilter extends PXView {
	ScreenID: PXFieldState<PXFieldOptions.CommitChanges>;
	UserID: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	TableName: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	autoRepaint: ['Changes'],
	generateColumns: GridColumnGeneration.Recreate, generateColumnsAfterSelect: true
})
export class AUAuditKeys extends PXView {
}

@gridConfig({
	preset: GridPreset.Inquiry,
	generateColumns: GridColumnGeneration.Append, generateColumnsAfterSelect: true})
export class AUAuditValues extends PXView {
	BatchID: PXFieldState;
	ChangeID: PXFieldState;
	Operation: PXFieldState;
	ChangeDate: PXFieldState;
	UserName: PXFieldState;
}
