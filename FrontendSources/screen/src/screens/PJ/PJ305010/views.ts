import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	GridColumnType,
	GridColumnShowHideMode,
	columnConfig,
	gridConfig
} from "client-controls";

export class Photos extends PXView {
	PhotoLogId: PXFieldState<PXFieldOptions.CommitChanges>;
	PhotoCd: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		type: GridColumnType.Icon
	})
	ImageUrl: PXFieldState;
	Name: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Multiline>;
	UploadedDate: PXFieldState;
	UploadedById: PXFieldState;
	IsMainPhoto: PXFieldState<PXFieldOptions.CommitChanges>;
	FileId: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	fastFilterByAllFields: false,
	allowInsert: false,
	allowDelete: false
})
export class Attributes extends PXView {
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False
	})
	AttributeID: PXFieldState;
	isRequired: PXFieldState;
	@columnConfig({
		allowSort: false,
		allowShowHide: GridColumnShowHideMode.False
	})
	Value: PXFieldState;
}

