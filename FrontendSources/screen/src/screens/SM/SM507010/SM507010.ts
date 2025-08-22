import {
	graphInfo,
	PXView,
	PXFieldState,
	gridConfig,
	PXScreen,
	createSingle,
	createCollection,
	PXFieldOptions,
	columnConfig,
	GridColumnShowHideMode,
	linkCommand,
	GridPreset,
} from "client-controls";

@graphInfo({ graphType: "PX.SM.EmailSendReceiveMaint", primaryView: "Filter" })
export class SM507010 extends PXScreen {
	Filter = createSingle(EmailProcessingFilter);
	FilteredItems = createCollection(EMailAccount);
}
@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	allowUpdate: false,
	fastFilterByAllFields: false,
	suppressNoteFiles: true,
})
export class EMailAccount extends PXView {
	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	@linkCommand("ViewDetails") Description: PXFieldState;
	Address: PXFieldState;
	InboxCount: PXFieldState;
	LastReceiveDateTime: PXFieldState;
	OutboxCount: PXFieldState;
	LastSendDateTime: PXFieldState;
}

export class EmailProcessingFilter extends PXView {
	Operation: PXFieldState<PXFieldOptions.CommitChanges>;
}
