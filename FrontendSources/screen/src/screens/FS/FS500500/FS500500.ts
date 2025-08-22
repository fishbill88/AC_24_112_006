import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	linkCommand,
	PXScreen,
	PXView,
	PXFieldState,
	PXActionState,
	PXFieldOptions,
	columnConfig
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.UpdateInventoryPost', primaryView: 'Filter' })
export class FS500500 extends PXScreen {
	ViewPostBatch: PXActionState;
	Filter = createSingle(UpdateInventoryFilter);
	Appointments = createCollection(FSAppointmentDet);
}

export class UpdateInventoryFilter extends PXView {
	CutOffDate: PXFieldState<PXFieldOptions.CommitChanges>;
	RouteDocumentID: PXFieldState<PXFieldOptions.CommitChanges>;
	AppointmentID: PXFieldState<PXFieldOptions.CommitChanges>;
	DocumentDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	mergeToolbarWith: "ScreenToolbar"
})
export class FSAppointmentDet extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) FSAppointment__SrvOrdType: PXFieldState;
	FSAppointment__RefNbr: PXFieldState;
	FSServiceOrder__BillCustomerID: PXFieldState;
	Customer__AcctName: PXFieldState;
	FSAppointment__SORefNbr: PXFieldState;
	SODetID: PXFieldState;
	PickupDeliveryServiceID: PXFieldState;
	ServiceType: PXFieldState;
	InventoryID: PXFieldState;
	@linkCommand("ViewPostBatch") Mem_BatchNbr: PXFieldState;
}
