import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	viewInfo,
	createSingle,
} from "client-controls";

export abstract class PanelNotificationFilterBase {
	@viewInfo({ containerName: "Notification Template" })
	NotificationInfo = createSingle(NotificationFilter);
}

export class NotificationFilter extends PXView {
	NotificationName: PXFieldState<PXFieldOptions.CommitChanges>;
	ReplaceEmailContents: PXFieldState<PXFieldOptions.CommitChanges>;
}
