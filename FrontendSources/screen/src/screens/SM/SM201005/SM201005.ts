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
	graphType: "PX.SM.RoleAccess",
	primaryView: "Roles",
	hideFilesIndicator: true,
	hideNotesIndicator: true,
})
export class SM201005 extends PXScreen {
	@viewInfo({ containerName: "Role Information" })
	Roles = createSingle(Roles);

	@viewInfo({ containerName: "Membership" })
	UsersByRole = createCollection(UsersInRoles);

	@viewInfo({ containerName: "Active Directory" })
	ActiveDirectoryMap = createCollection(RoleActiveDirectory);

	@viewInfo({ containerName: "Claims" })
	ClaimsMap = createCollection(RoleClaims);
}

// Views

export class Roles extends PXView {
	Rolename: PXFieldState;
	Descr: PXFieldState;
	Guest: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	adjustPageSize: true,
	quickFilterFields: ["Username", "DisplayName", "Comment"],
})
export class UsersInRoles extends PXView {
	@columnConfig({ hideViewLink: true })
	Username: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({
		allowUpdate: false,
		allowShowHide: GridColumnShowHideMode.False,
	})
	ApplicationName: PXFieldState;

	@columnConfig({
		allowUpdate: false,
		allowShowHide: GridColumnShowHideMode.False,
	})
	Rolename: PXFieldState;

	@columnConfig({ allowUpdate: false })
	DisplayName: PXFieldState;

	@columnConfig({ allowUpdate: false })
	State: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Domain: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Comment: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Inherited: PXFieldState;
}

@gridConfig({
	adjustPageSize: true,
	quickFilterFields: ["Name", "Description"],
})
export class RoleActiveDirectory extends PXView {
	GroupID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false })
	GroupDomain: PXFieldState;

	@columnConfig({ allowUpdate: false })
	GroupDescription: PXFieldState;
}

@gridConfig({ adjustPageSize: true })
export class RoleClaims extends PXView {
	GroupID: PXFieldState;
}
