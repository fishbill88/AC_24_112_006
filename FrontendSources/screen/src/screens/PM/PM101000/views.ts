import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	GridColumnShowHideMode,
	columnConfig,
	gridConfig
} from "client-controls";

export class Setup extends PXView {
	TranNumbering: PXFieldState;
	BatchNumberingID: PXFieldState;
	ProformaNumbering: PXFieldState;
	ChangeOrderNumbering: PXFieldState;
	ChangeRequestNumbering: PXFieldState;
	QuoteNumberingID: PXFieldState;
	ProgressWorksheetNumbering: PXFieldState;
	IsActive: PXFieldState;
	NonProjectCode: PXFieldState;
	EmptyItemCode: PXFieldState;
	EmptyItemUOM: PXFieldState;
	DefaultChangeOrderClassID: PXFieldState;
	QuoteTemplateID: PXFieldState;
	ProformaAssignmentMapID: PXFieldState;
	ProformaAssignmentNotificationID: PXFieldState;
	CutoffDate: PXFieldState;
	OverLimitErrorLevel: PXFieldState;
	RevenueBudgetUpdateMode: PXFieldState;
	CostBudgetUpdateMode: PXFieldState;
	BudgetControl: PXFieldState;
	AutoPost: PXFieldState;
	AutoReleaseAllocation: PXFieldState;
	CostCommitmentTracking: PXFieldState;
	CalculateProjectSpecificTaxes: PXFieldState<PXFieldOptions.CommitChanges>;
	MigrationMode: PXFieldState;
	VisibleInGL: PXFieldState;
	VisibleInAP: PXFieldState;
	VisibleInAR: PXFieldState;
	VisibleInSO: PXFieldState;
	VisibleInPO: PXFieldState;
	VisibleInIN: PXFieldState;
	VisibleInCA: PXFieldState;
	VisibleInCR: PXFieldState;
	VisibleInPROD: PXFieldState;
	VisibleInTA: PXFieldState;
	VisibleInEA: PXFieldState;
	RestrictProjectSelect: PXFieldState;
	ExpenseAccountSource: PXFieldState;
	ExpenseSubMask: PXFieldState;
	ExpenseAccrualAccountSource: PXFieldState;
	ExpenseAccrualSubMask: PXFieldState;
	DefaultPriceMarkupPct: PXFieldState;
	UnbilledRemainderAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	UnbilledRemainderSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	UnbilledRemainderOffsetAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	UnbilledRemainderOffsetSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	DropshipExpenseAccountSource: PXFieldState;
	DropshipExpenseSubMask: PXFieldState;
	DropshipReceiptProcessing: PXFieldState<PXFieldOptions.CommitChanges>;
	DropshipExpenseRecording: PXFieldState<PXFieldOptions.CommitChanges>;
	AssignmentMapID: PXFieldState;
	ProformaApprovalMapID: PXFieldState;
	ChangeRequestApprovalMapID: PXFieldState;
	ChangeOrderApprovalMapID: PXFieldState;
	QuoteApprovalMapID: PXFieldState;
	CostProjectionApprovalMapID: PXFieldState;
	ProgressWorksheetApprovalMapID: PXFieldState;
	AssignmentNotificationID: PXFieldState;
	ProformaApprovalNotificationID: PXFieldState;
	ChangeRequestApprovalNotificationID: PXFieldState;
	ChangeOrderApprovalNotificationID: PXFieldState;
	QuoteApprovalNotificationID: PXFieldState;
	CostProjectionApprovalNotificationID: PXFieldState;
	ProgressWorksheetApprovalNotificationID: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	suppressNoteFiles: true,
	allowInsert: true,
	allowDelete: true
})
export class Markups extends PXView {
	Type: PXFieldState;
	Description: PXFieldState;
	Value: PXFieldState;
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
	NotificationID: PXFieldState;
	NBranchID: PXFieldState;
	Format: PXFieldState<PXFieldOptions.CommitChanges>;
	RecipientsBehavior: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class Recipients extends PXView {
	Active: PXFieldState;
	ContactType: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False
	})
	OriginalContactID: PXFieldState<PXFieldOptions.Hidden>;
	ContactID: PXFieldState;
	Format: PXFieldState<PXFieldOptions.CommitChanges>;
	AddTo: PXFieldState;
}

