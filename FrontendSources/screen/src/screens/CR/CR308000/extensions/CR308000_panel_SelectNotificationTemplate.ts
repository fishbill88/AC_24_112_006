import { CR308000 } from "../CR308000";
import { createSingle, PXView, PXFieldState, PXFieldOptions } from "client-controls";

export interface CR308000_panel_SelectNotificationTemplate extends CR308000 {}
export class CR308000_panel_SelectNotificationTemplate {
	NotificationInfo = createSingle(NotificationFilter);
}

export class NotificationFilter extends PXView {
	NotificationName: PXFieldState<PXFieldOptions.CommitChanges>;
	ReplaceEmailContents: PXFieldState<PXFieldOptions.CommitChanges>;
}