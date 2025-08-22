import {
	graphInfo, PXView, PXFieldState, gridConfig, PXScreen,
	createSingle, createCollection, PXFieldOptions,
	columnConfig, GridColumnShowHideMode, linkCommand, GridPreset }
	from "client-controls";

@graphInfo({graphType: 'PX.SM.EmailProcessingMaint', primaryView: 'Filter'})
export class SM507000 extends PXScreen {
	Filter = createSingle(EmailProcessingFilter);
	FilteredItems = createCollection(SMEmail);
}
@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	allowUpdate: false,
	adjustPageSize: true
})
export class SMEmail extends PXView {
	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({allowCheckAll: true})Selected: PXFieldState;
	EMailAccount__Description: PXFieldState;
	@linkCommand('viewDetails')Subject: PXFieldState;
	MailFrom: PXFieldState;
	MailTo: PXFieldState;
	CRActivity__StartDate: PXFieldState;
	@columnConfig({hideViewLink: true})CRActivity__OwnerID: PXFieldState;
	MPStatus: PXFieldState;
}

@gridConfig({syncPosition: true,allowDelete: false,allowInsert: false,allowUpdate: false,adjustPageSize: true})
export class EmailProcessingFilter extends PXView {
	Account: PXFieldState<PXFieldOptions.CommitChanges>;
	MyOwner: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeFailed: PXFieldState;
}