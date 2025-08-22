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

@graphInfo({ graphType: 'PX.Objects.FS.EquipmentSetupMaint', primaryView: 'EquipmentSetupRecord' })
export class FS100300 extends PXScreen {
	EquipmentSetupRecord = createSingle(FSEquipmentSetup);
	Notifications = createCollection(FSCTNotification);
	Recipients = createCollection(NotificationSetupRecipient);
}

export class FSEquipmentSetup extends PXView {
	EquipmentNumberingID: PXFieldState;
	ServiceContractNumberingID: PXFieldState;
	ScheduleNumberingID: PXFieldState;
	EnableAllTargetEquipment: PXFieldState;
	EnableSeasonScheduleContract: PXFieldState;
	EquipmentCalculateWarrantyFrom: PXFieldState;
	ContractPostTo: PXFieldState<PXFieldOptions.CommitChanges>;
	ContractPostOrderType: PXFieldState;
	DfltContractTermIDARSO: PXFieldState;
	ContractSalesAcctSource: PXFieldState<PXFieldOptions.CommitChanges>;
	ContractCombineSubFrom: PXFieldState<PXFieldOptions.CommitChanges>;
	EnableContractPeriodWhenInvoice: PXFieldState;
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
