import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	GridPreset
} from "client-controls";

@gridConfig({
	preset: GridPreset.Details,
	fastFilterByAllFields: false,
})
export class NotificationSource extends PXView {
	Active: PXFieldState;
	NotificationCD: PXFieldState;
	EMailAccountID: PXFieldState;
	@columnConfig({ hideViewLink: true }) ReportID: PXFieldState;
	@columnConfig({ hideViewLink: true }) NotificationID: PXFieldState;
	NBranchID: PXFieldState;
	Format: PXFieldState;
	RecipientsBehavior: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	fastFilterByAllFields: false,
})
export class NotificationRecipient extends PXView {
	Active: PXFieldState;
	ContactType: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactID: PXFieldState;
	Format: PXFieldState;
	AddTo: PXFieldState;
}
