import {
	graphInfo,
	createSingle,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.RoutesOptimizationProcess', primaryView: 'Filter' })
export class FS501400 extends PXScreen {
	Filter = createSingle(FSAppointmentFilter);
	AppointmentList = createCollection(FSAppointmentFSServiceOrder);
	StaffMemberFilter = createCollection(FSAppointmentStaffMember);
}

export class FSAppointmentStaffMember extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	Type: PXFieldState;
	AcctCD: PXFieldState;
	AcctName: PXFieldState;
}

export class FSAppointmentFSServiceOrder extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	@columnConfig({ hideViewLink: true }) SrvOrdType: PXFieldState;
	RefNbr: PXFieldState;
	PrimaryDriver: PXFieldState;
	CustomerID: PXFieldState;
	ScheduledDateTimeBegin_Time: PXFieldState;
	Confirmed: PXFieldState;
	ScheduledDuration: PXFieldState;
	EstimatedDurationTotal: PXFieldState;
	ProjectID: PXFieldState;
	AddressLine1: PXFieldState;
	State: PXFieldState;
	City: PXFieldState;
}

export class FSAppointmentFilter extends PXView {
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsiderSkills: PXFieldState<PXFieldOptions.CommitChanges>;
}
