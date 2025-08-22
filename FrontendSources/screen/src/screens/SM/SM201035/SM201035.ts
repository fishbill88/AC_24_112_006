import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	PXView,
	PXFieldState,
	gridConfig,
	columnConfig,
} from "client-controls";

@graphInfo({
	graphType: "PX.SM.UserAccess",
	primaryView: "User"
})
export class SM201035 extends PXScreen {
	@viewInfo({ containerName: "User" })
	User = createSingle(Users);

	@viewInfo({ containerName: "Restriction Groups" })
	Groups = createCollection(RelationGroup);
}

// Views

export class Users extends PXView {
	Username: PXFieldState;
	FirstName: PXFieldState;
	LastName: PXFieldState;
	Comment: PXFieldState;
}

@gridConfig({
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
	adjustPageSize: true
})
export class RelationGroup extends PXView {
	@columnConfig({ allowCheckAll: true })
	Included: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	GroupName: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Description: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Active: PXFieldState;

	@columnConfig({ allowUpdate: false })
	GroupType: PXFieldState;
}
