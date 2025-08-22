import {
	columnConfig,
	gridConfig,
	GridColumnShowHideMode,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class Setup extends PXView {
	SubcontractNumberingID: PXFieldState;
	RequireSubcontractControlTotal: PXFieldState;
	SubcontractRequestApproval: PXFieldState;
}

export class SetupApproval extends PXView {
	AssignmentMapID: PXFieldState;
	AssignmentNotificationID: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true
})
export class Notifications extends PXView {
	Active: PXFieldState;
	NotificationCD: PXFieldState;
	EMailAccountID: PXFieldState;
	ReportID: PXFieldState<PXFieldOptions.CommitChanges>;
	NotificationID: PXFieldState<PXFieldOptions.CommitChanges>;
	NBranchID: PXFieldState;
	Format: PXFieldState<PXFieldOptions.CommitChanges>;
	RecipientsBehavior: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true
})
export class Recipients extends PXView {
	Active: PXFieldState;
	ContactType: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	OriginalContactID: PXFieldState<PXFieldOptions.Hidden>;
	ContactID: PXFieldState;
	Format: PXFieldState<PXFieldOptions.CommitChanges>;
	AddTo: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class Attributes extends PXView {
	IsActive: PXFieldState;
	AttributeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	SortOrder: PXFieldState;
	Required: PXFieldState;
	ControlType: PXFieldState;
	Type: PXFieldState;
}
