// TODO: Cancel() action is not called by the UI (https://jira.acumatica.com/browse/AC-290465)
// TODO: PXSelector columns order and default sort are different from Old UI (https://jira.acumatica.com/browse/AC-290463)
// TODO: missing PXSelector.DisplayMode and PXSelector.TextField (https://jira.acumatica.com/browse/AC-285843)
// TODO: disable FastFilter in the grid (https://jira.acumatica.com/browse/AC-290459)
// TODO: hide Import from Excel button (https://jira.acumatica.com/browse/AC-290457)

import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXFieldOptions,
	PXFieldState,
	PXView,
} from "client-controls";

@graphInfo({ graphType: "PX.SM.RelationEntities", primaryView: "HeaderEntity" })
export class SM201040 extends PXScreen {
	HeaderEntity = createSingle(HeaderEntity);
	DetailsEntity = createCollection(DetailsEntity);
}

export class HeaderEntity extends PXView {
	EntityTypeName: PXFieldState<PXFieldOptions.CommitChanges>;
	Entity: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class DetailsEntity extends PXView {
	Selected: PXFieldState;

	GroupName: PXFieldState;

	Description: PXFieldState;

	Active: PXFieldState;

	GroupType: PXFieldState;
}
