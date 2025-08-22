import {
	createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, gridConfig, columnConfig
} from 'client-controls';

@graphInfo({
	graphType: 'PX.Objects.GL.GLAccessBudget', primaryView: 'Group',
	hideFilesIndicator: true, hideNotesIndicator: true
})
export class GL105030 extends PXScreen {

	Group = createSingle(RelationGroup);
	Users = createCollection(Users);
	BudgetTree = createCollection(GLBudgetTree);

}

export class RelationGroup extends PXView {

	GroupName: PXFieldState;
	Description: PXFieldState;
	GroupType: PXFieldState;
	Active: PXFieldState;
}

@gridConfig({
	allowDelete: false, allowInsert: false, allowUpdate: false, adjustPageSize: true, suppressNoteFiles: true,
	quickFilterFields: ["Username", "FullName"]
})
export class Users extends PXView {

	@columnConfig({ allowUpdate: false, allowCheckAll: true })
	Included: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	Username: PXFieldState;
	@columnConfig({ allowUpdate: false })
	FullName: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Comment: PXFieldState;
}

@gridConfig({
	allowDelete: false, allowUpdate: false, adjustPageSize: true,
	quickFilterFields: ["AccountID", "SubID", "GroupID"]
})
export class GLBudgetTree extends PXView {

	@columnConfig({ allowUpdate: false, allowCheckAll: true })
	Included: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true, width: 200 })
	GroupID: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	AccountID: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	SubID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AccountMask: PXFieldState;
	@columnConfig({ allowUpdate: false })
	SubMask: PXFieldState;
	@columnConfig({ allowUpdate: false })
	IsGroup: PXFieldState;
}
