import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	PXView,
	PXFieldState,
	gridConfig,
	PXFieldOptions,
	columnConfig,
	GridColumnShowHideMode,
} from "client-controls";

@graphInfo({
	graphType: "PX.SM.RelationGroups",
	primaryView: "HeaderGroup",
	hideFilesIndicator: true,
	hideNotesIndicator: true,
})
export class SM201030 extends PXScreen {
	@viewInfo({ containerName: "Restriction Group" })
	HeaderGroup = createSingle(RelationGroup);

	@viewInfo({ containerName: "Entities" })
	DetailsGroup = createCollection(MaskedEntity);
}

export class RelationGroup extends PXView {
	GroupName: PXFieldState;
	Active: PXFieldState;
	GroupType: PXFieldState;
	Description: PXFieldState;
	EntityTypeName: PXFieldState<PXFieldOptions.CommitChanges>;
	SpecificType: PXFieldState<PXFieldOptions.CommitChanges>;
	SpecificModule: PXFieldState;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	adjustPageSize: true,
	batchUpdate: true,
})
export class MaskedEntity extends PXView {
	Selected: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Entity: PXFieldState;

	@columnConfig({
		allowUpdate: false,
		allowShowHide: GridColumnShowHideMode.False,
	})
	ID: PXFieldState<PXFieldOptions.Hidden>;
}
