import { CR302000 } from "../CR302000";
import {
	PXView,
	PXFieldState,
	PXActionState,
	linkCommand,
	createCollection,
	createSingle,
	PXFieldOptions,
	gridConfig,
	viewInfo,
	columnConfig,
	GridPagerMode,
	GridPreset,
} from "client-controls";

export interface CR302000_UserInfo extends CR302000 {}
export class CR302000_UserInfo {
	@viewInfo({ containerName: "User Info" })
	User = createSingle(Users);
	Roles = createCollection(EPLoginTypeAllowsRole);
	@viewInfo({ containerName: "Locations" })
	RoleAssignments = createCollection(BCRoleAssignment);
}

export class Users extends PXView {
	ResetPasswordOK: PXActionState;
	ResetPassword: PXActionState;
	ActivateLogin: PXActionState;
	EnableLogin: PXActionState;
	DisableLogin: PXActionState;
	UnlockLogin: PXActionState;

	State: PXFieldState<PXFieldOptions.Disabled>;
	LoginTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Username: PXFieldState<PXFieldOptions.CommitChanges>;
	Password: PXFieldState;
	GeneratePassword: PXFieldState<PXFieldOptions.CommitChanges>;
	NewPassword: PXFieldState;
	ConfirmPassword: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	fastFilterByAllFields: false,
	pagerMode: GridPagerMode.InfiniteScroll,
	autoAdjustColumns: true,
})
export class EPLoginTypeAllowsRole extends PXView {
	Selected: PXFieldState;
	@columnConfig({ hideViewLink: true })
	Rolename: PXFieldState<PXFieldOptions.Disabled>;
	Rolename_Roles_descr: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true
})
export class BCRoleAssignment extends PXView {
	LocationID: PXFieldState<PXFieldOptions.Disabled>;
	Role: PXFieldState<PXFieldOptions.Disabled>;
}
