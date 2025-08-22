import { CR304000 } from "../CR304000";
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

export interface CR304000_Activities extends CR304000 {}
export class CR304000_Activities {
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
	@linkCommand("<stub>")
	BAccountID: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("<stub>")
	ContactID: PXFieldState<PXFieldOptions.Hidden>;
	ProjectID: PXFieldState<PXFieldOptions.Hidden>;
	ProjectTaskID: PXFieldState<PXFieldOptions.Hidden>;

	Body: PXFieldState<PXFieldOptions.Hidden>;
}
