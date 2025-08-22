import { CR304000 } from "../CR304000";
import {
	PXView,
	PXFieldState,
	linkCommand,
	createCollection,
	PXFieldOptions,
	gridConfig,
	viewInfo,
	createSingle,
} from "client-controls";

export interface CR304000_DialogBoxSOAppt extends CR304000 {}
export class CR304000_DialogBoxSOAppt {
	@viewInfo({ containerName: "Document Settings" })
	DocumentSettings = createSingle(DBoxDocSettings);

	@viewInfo({ containerName: "Create Service Order" })
	CreateServiceOrderFilter = createSingle(FSCreateServiceOrderFilter);
}

export class DBoxDocSettings extends PXView {
	SrvOrdType: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderDate: PXFieldState<PXFieldOptions.CommitChanges>;
	SLAETA_Date: PXFieldState;
	SLAETA_Time: PXFieldState;
	AssignedEmpID: PXFieldState;
	ProblemID: PXFieldState;
	ContactID: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduledDateTimeBegin_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduledDateTimeBegin_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduledDateTimeEnd_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduledDateTimeEnd_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	HandleManuallyScheduleTime: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class FSCreateServiceOrderFilter extends PXView {
	SrvOrdType: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
}
