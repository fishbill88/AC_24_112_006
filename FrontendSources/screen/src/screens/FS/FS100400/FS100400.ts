import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.RouteSetupMaint', primaryView: 'RouteSetupRecord' })
export class FS100400 extends PXScreen {
	SetupRecord = createSingle(FSSetup);
	RouteSetupRecord = createSingle(FSRouteSetup);
	Notifications = createCollection(FSCTNotification);
	Recipients = createCollection(NotificationSetupRecipient);
}

export class FSSetup extends PXView {
	EquipmentNumberingID: PXFieldState;
	ServiceContractNumberingID: PXFieldState;
	ScheduleNumberingID: PXFieldState;
	ContractPostTo: PXFieldState<PXFieldOptions.CommitChanges>;
	ContractPostOrderType: PXFieldState;
	DfltContractTermIDARSO: PXFieldState;
	ContractSalesAcctSource: PXFieldState<PXFieldOptions.CommitChanges>;
	ContractCombineSubFrom: PXFieldState<PXFieldOptions.CommitChanges>;
	EnableContractPeriodWhenInvoice: PXFieldState;
}

export class FSRouteSetup extends PXView {
	RouteNumberingID: PXFieldState;
	EnableSeasonScheduleContract: PXFieldState;
	DfltSrvOrdType: PXFieldState;
	AutoCalculateRouteStats: PXFieldState;
	GroupINDocumentsByPostingProcess: PXFieldState<PXFieldOptions.CommitChanges>;
	SetFirstManualAppointment: PXFieldState;
	TrackRouteLocation: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	autoRepaint: ['Recipients']
})
export class FSCTNotification extends PXView {
	Active: PXFieldState;
	NotificationCD: PXFieldState;
	EMailAccountID: PXFieldState;
	ReportID: PXFieldState;
	NotificationID: PXFieldState;
	Format: PXFieldState;
	RecipientsBehavior: PXFieldState;
}

export class NotificationSetupRecipient extends PXView {
	Active: PXFieldState;
	ContactType: PXFieldState;
	ContactID: PXFieldState;
	Format: PXFieldState;
	AddTo: PXFieldState;
}
