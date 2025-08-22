import { CR301000 } from "../CR301000";
import {
	PXView,
	PXFieldState,
	linkCommand,
	createCollection,
	PXFieldOptions,
	PXActionState,
	gridConfig,
	columnConfig,
} from "client-controls";

export interface CR301000_Activities extends CR301000 {}
export class CR301000_Activities {
	Activities = createCollection(Activities);
}

@gridConfig({
	syncPosition: true,
	allowInsert: false,
	allowDelete: false,
})
export class Activities extends PXView {
	NewTask: PXActionState;
	NewEvent: PXActionState;
	NewMailActivity: PXActionState;
	NewActivity: PXActionState;
	TogglePinActivity: PXActionState;

	@columnConfig({ width: 35 })
	IsPinned: PXFieldState;
	@columnConfig({ width: 35 })
	IsCompleteIcon: PXFieldState;
	@columnConfig({ width: 35 })
	PriorityIcon: PXFieldState;
	@columnConfig({ width: 35 })
	CRReminder__ReminderIcon: PXFieldState;
	@columnConfig({ width: 35 })
	ClassIcon: PXFieldState;
	ClassInfo: PXFieldState;

	@linkCommand("ViewActivity")
	Subject: PXFieldState;

	UIStatus: PXFieldState;
	Released: PXFieldState;
	StartDate: PXFieldState;
	CreatedDateTime: PXFieldState;
	TimeSpent: PXFieldState;
	CreatedByID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;
	WorkgroupID: PXFieldState;

	@linkCommand("OpenActivityOwner")
	OwnerID: PXFieldState;

	Source: PXFieldState<PXFieldOptions.Hidden>;
	BAccountID: PXFieldState<PXFieldOptions.Hidden>;
	ContactID: PXFieldState<PXFieldOptions.Hidden>;
	ProjectID: PXFieldState<PXFieldOptions.Hidden>;
	ProjectTaskID: PXFieldState<PXFieldOptions.Hidden>;

	Body: PXFieldState<PXFieldOptions.Hidden>;
}
