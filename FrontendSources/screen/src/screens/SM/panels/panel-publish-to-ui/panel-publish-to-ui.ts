import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	createSingle,
	viewInfo,
} from "client-controls";

export abstract class PanelPublishToUiBase {
	@viewInfo({ containerName: "Publish to the UI" })
	PublishToUIDialog = createSingle(SiteMapWithAccessRights);
}

export class SiteMapWithAccessRights extends PXView {
	Title: PXFieldState<PXFieldOptions.CommitChanges>;
	Workspace: PXFieldState<PXFieldOptions.CommitChanges>;
	Category: PXFieldState<PXFieldOptions.CommitChanges>;
	ScreenID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccessRights: PXFieldState<PXFieldOptions.CommitChanges>;
	CopyFromScreenID: PXFieldState<PXFieldOptions.CommitChanges>;
}
