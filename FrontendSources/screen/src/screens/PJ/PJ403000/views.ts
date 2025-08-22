import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	GridColumnShowHideMode,
	linkCommand,
	columnConfig,
	gridConfig
} from "client-controls";

export class Filter extends PXView {
	ProjectId: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskId: PXFieldState<PXFieldOptions.CommitChanges>;
	DisciplineId: PXFieldState<PXFieldOptions.CommitChanges>;
	StatusID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectIssueID: PXFieldState<PXFieldOptions.CommitChanges>;
	RFIID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsCurrentOnly: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true,
	initNewRow: true,
	allowInsert: false,
	allowImport: true,
	actionsConfig: {
		InsertDrawingLogInGrid: {
			images: {
				normal: "main@RecordAdd"
			}
		}
	}
})
export class DrawingLogs extends PXView {
	InsertDrawingLogInGrid: PXActionState;

	@columnConfig({
		allowCheckAll: true,
		allowShowHide: GridColumnShowHideMode.False
	})
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("editDrawingLog")
	@columnConfig({width: 150})
	DrawingLogCd: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewEntity")
	ProjectId: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Disabled>;
	@linkCommand("ViewEntity")
	ProjectTaskId: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	DisciplineId: PXFieldState<PXFieldOptions.CommitChanges>;
	// eslint-disable-next-line id-denylist
	Number: PXFieldState<PXFieldOptions.CommitChanges>;
	Revision: PXFieldState<PXFieldOptions.CommitChanges>;
	Sketch: PXFieldState<PXFieldOptions.CommitChanges>;
	Title: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	StatusId: PXFieldState<PXFieldOptions.CommitChanges>;
	DrawingDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ReceivedDate: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("DrawingLog$OriginalDrawingId$Link")
	OriginalDrawingId: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true, width: 120})
	OwnerId: PXFieldState<PXFieldOptions.CommitChanges>;
}

