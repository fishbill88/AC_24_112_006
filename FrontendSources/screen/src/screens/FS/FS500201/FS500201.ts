import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.CloneAppointmentProcess', primaryView: 'Filter' })
export class FS500201 extends PXScreen {
	Filter = createSingle(FSCloneAppointmentFilter);
	ServiceOrderRelated = createSingle(FSServiceOrder);
	AppointmentSelected = createSingle(FSAppointment);
	AppointmentClones = createCollection(FSAppointmentFSServiceOrder);
}

export class FSCloneAppointmentFilter extends PXView {
	CloningType: PXFieldState<PXFieldOptions.CommitChanges>;
	SingleGenerationDate: PXFieldState<PXFieldOptions.CommitChanges>;
	MultGenerationFromDate: PXFieldState<PXFieldOptions.CommitChanges>;
	MultGenerationToDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduledStartTime_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	ApptDuration: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideApptDuration: PXFieldState<PXFieldOptions.CommitChanges>;
	ActiveOnSunday: PXFieldState<PXFieldOptions.CommitChanges>;
	ActiveOnMonday: PXFieldState<PXFieldOptions.CommitChanges>;
	ActiveOnTuesday: PXFieldState<PXFieldOptions.CommitChanges>;
	ActiveOnWednesday: PXFieldState<PXFieldOptions.CommitChanges>;
	ActiveOnThursday: PXFieldState<PXFieldOptions.CommitChanges>;
	ActiveOnFriday: PXFieldState<PXFieldOptions.CommitChanges>;
	ActiveOnSaturday: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	mergeToolbarWith: "ScreenToolbar"
})
export class FSAppointmentFSServiceOrder extends PXView {
	@columnConfig({ hideViewLink: true }) SrvOrdType: PXFieldState;
	SORefNbr: PXFieldState;
	RefNbr: PXFieldState;
	ScheduledDateTimeBegin: PXFieldState;
	ScheduledDateTimeBegin_Time: PXFieldState;
	ScheduledDateTimeEnd: PXFieldState;
	ScheduledDateTimeEnd_Time: PXFieldState;
	Status: PXFieldState;
}

export class FSAppointment extends PXView {
	SrvOrdType: PXFieldState;
	RefNbr: PXFieldState;
	SORefNbr: PXFieldState;
}

export class FSServiceOrder extends PXView {
	CustomerID: PXFieldState<PXFieldOptions.Readonly>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Readonly>;
	BranchLocationID: PXFieldState<PXFieldOptions.Readonly>;
}
