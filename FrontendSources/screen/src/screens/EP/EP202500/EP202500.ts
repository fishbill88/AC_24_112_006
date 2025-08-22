import {
	createCollection,
	createSingle,
	PXScreen,
	PXView,
	graphInfo,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	gridConfig,
	GridPreset,
} from "client-controls";

@graphInfo({ graphType: "PX.EP.EPLoginTypeMaint", primaryView: "LoginType" })
export class EP202500 extends PXScreen {
	LoginType = createSingle(EPLoginType);
	AllowedRoles = createCollection(EPLoginTypeAllowsRole);
	Users = createCollection(Users);
	ManagedLoginTypes = createCollection(EPManagedLoginType);
}

export class EPLoginType extends PXView {
	LoginTypeName: PXFieldState;
	Entity: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	EmailAsLogin: PXFieldState<PXFieldOptions.CommitChanges>;
	ResetPasswordOnLogin: PXFieldState;
	RequireLoginActivation: PXFieldState;
	AllowedLoginType: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowedSessions: PXFieldState;
	DisableTwoFactorAuth: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	fastFilterByAllFields: false,
})
export class EPLoginTypeAllowsRole extends PXView {
	UpdateUsers: PXActionState;
	IsDefault: PXFieldState<PXFieldOptions.CommitChanges>;
	Rolename: PXFieldState;
	Roles__Descr: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class Users extends PXView {
	Username: PXFieldState;
	State: PXFieldState;
	DisplayName: PXFieldState;
	Comment: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class EPManagedLoginType extends PXView {
	LoginTypeID: PXFieldState;
	EPLoginType__Description: PXFieldState;
}
