import {
	PXView,
	PXFieldState,
	columnConfig,
	graphInfo,
	PXScreen,
	linkCommand,
	createCollection,
	gridConfig,
	PXFieldOptions,
	PXPageLoadBehavior,
	GridPagerMode,
	PXActionState,
	createSingle,
	GridPreset,
} from "client-controls";

@graphInfo({ graphType: "PX.SM.SMAccess", primaryView: "Group" })
export class SM201050 extends PXScreen {
	Group = createSingle(RelationGroup);
	Users = createCollection(User);
	Account = createCollection(Account);
}

export class RelationGroup extends PXView {
	GroupName: PXFieldState;
	Description: PXFieldState;
	GroupType: PXFieldState;
	Active: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowDelete: false,
	suppressNoteFiles: true,
	quickFilterFields: ["EmailAccountId", "LoginName"],
})
export class Account extends PXView {
	EmailAccountId: PXFieldState;
	Description: PXFieldState;
	LoginName: PXFieldState;
	Included: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	quickFilterFields: ["Username", "Comment"],
	allowDelete: false,
	pagerMode: GridPagerMode.Numeric,
})
export class User extends PXView {
	@columnConfig({ allowCheckAll: true })
	Included: PXFieldState;
	Username: PXFieldState;
	FullName: PXFieldState;
	Comment: PXFieldState;
}
