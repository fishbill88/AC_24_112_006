import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	columnConfig
} from 'client-controls';

@graphInfo({graphType: 'PX.Objects.RQ.RQSetupMaint', primaryView: 'Setup'})
export class RQ101000 extends PXScreen {

	@viewInfo({containerName: 'General Settings'})
	Setup = createSingle(RQSetup);

	@viewInfo({containerName: 'Approval'})
	SetupApproval = createCollection(RQSetupApproval);

	@viewInfo({containerName: 'Default Sources'})
	Notifications = createCollection(NotificationSetup);

	@viewInfo({containerName: 'Default Recipients'})
	Recipients = createCollection(NotificationSetupRecipient);
}

export class RQSetup extends PXView  {
	RequestNumberingID: PXFieldState;
	RequestAssignmentMapID: PXFieldState;
	MonthRetainRequest: PXFieldState;
	RequestApproval: PXFieldState;
	RequisitionNumberingID: PXFieldState;
	RequisitionAssignmentMapID: PXFieldState;
	MonthRetainRequisition: PXFieldState;
	RequisitionApproval: PXFieldState;
	RequisitionMergeLines: PXFieldState;
	QTOrderType: PXFieldState;
	SOOrderType: PXFieldState;
	POHold: PXFieldState;
	BudgetLedgerId: PXFieldState;
	BudgetCalculation: PXFieldState;
	DefaultReqClassID: PXFieldState;
}

export class RQSetupApproval extends PXView  {
	Type: PXFieldState;
	AssignmentMapID: PXFieldState;
	AssignmentNotificationID: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	autoRepaint: ['Recipients']
})
export class NotificationSetup extends PXView  {
	Active: PXFieldState;
	NotificationCD: PXFieldState;
	@columnConfig({ hideViewLink: true })
	NBranchID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	EMailAccountID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	DefaultPrinterID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ReportID: PXFieldState;
	NotificationID: PXFieldState;
	Format: PXFieldState;
	RecipientsBehavior: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true
})
export class NotificationSetupRecipient extends PXView  {
	Active: PXFieldState;
	ContactType: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactID: PXFieldState;
	Format: PXFieldState;
	AddTo: PXFieldState;
}