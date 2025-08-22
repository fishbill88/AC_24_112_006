import {
	PXView,
	PXFieldState,
	gridConfig,
	PXFieldOptions,
	columnConfig,
	PXActionState,
} from "client-controls";

// Views

export class Version extends PXView {
	CurrentVersion: PXFieldState<PXFieldOptions.Disabled>;
	Date: PXFieldState<PXFieldOptions.Disabled>;
}

export class LockoutFilter extends PXView {
	DateTime_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	DateTime_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	Reason: PXFieldState<PXFieldOptions.CommitChanges>;
	LockoutAll: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class UPLogFileFilter extends PXView {
	Text: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class VersionFilter extends PXView {
	Key: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	fastFilterByAllFields: false,
	syncPosition: true,
})
export class AvailableVersion extends PXView {
	UploadVersionCommand: PXActionState;
	DownloadVersionCommand: PXActionState;
	ApplyVersionCommand: PXActionState;
	ValidateCompatibility: PXActionState;
	@columnConfig({ allowUpdate: false }) Version: PXFieldState;
	@columnConfig({ allowUpdate: false, width: 180 }) Date: PXFieldState;
	@columnConfig({ allowUpdate: false, width: 100 }) Restricted: PXFieldState;
	@columnConfig({ allowUpdate: false, width: 150 }) Uploaded: PXFieldState;
}

@gridConfig({
	fastFilterByAllFields: false,
	syncPosition: true,
	autoRepaint: ["UpdateErrorRecords"],
})
export class UPHistory extends PXView {
	@columnConfig({ width: 120 }) UpdateID: PXFieldState;
	@columnConfig({ width: 120 })
	UPHistoryComponents__UpdateComponentID: PXFieldState;
	@columnConfig({ allowUpdate: false, width: 100 }) Host: PXFieldState;
	@columnConfig({ allowUpdate: false, width: 120 })
	UPHistoryComponents__ComponentName: PXFieldState;
	@columnConfig({ allowUpdate: false })
	UPHistoryComponents__ComponentType: PXFieldState;
	@columnConfig({ allowUpdate: false })
	UPHistoryComponents__FromVersion: PXFieldState;
	@columnConfig({ allowUpdate: false })
	UPHistoryComponents__ToVersion: PXFieldState;
	@columnConfig({ allowUpdate: false, width: 150 }) Started: PXFieldState;
	@columnConfig({ allowUpdate: false, width: 150 }) Finished: PXFieldState;
}

@gridConfig({
	autoAdjustColumns: true,
	fastFilterByAllFields: false,
})
export class UPErrors extends PXView {
	SkipErrorCommand: PXActionState;
	ShowLogFileCommand: PXActionState;
	ClearLogFileCommand: PXActionState;
	@columnConfig({ allowUpdate: false }) ErrorID: PXFieldState;
	@columnConfig({ allowUpdate: false }) Message: PXFieldState;
	@columnConfig({ allowUpdate: false }) Skip: PXFieldState;
	@columnConfig({ allowUpdate: false }) Stack: PXFieldState;
	@columnConfig({ allowUpdate: false }) Script: PXFieldState;
}
