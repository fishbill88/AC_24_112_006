import { EP203000 } from "../EP203000";
import {
	PXView,
	PXFieldState,
	linkCommand,
	createCollection,
	PXFieldOptions,
	PXActionState,
	gridConfig,
} from "client-controls";

export interface EP203000_Activities extends EP203000 {}
export class EP203000_Activities {
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

	IsPinned: PXFieldState;
	IsCompleteIcon: PXFieldState;
	PriorityIcon: PXFieldState;
	CRReminder__ReminderIcon: PXFieldState;
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
