import {
	createSingle,
	PXScreen,
	PXView,
	graphInfo,
	PXFieldState,
	PXFieldOptions,
	createCollection,
	columnConfig,
	gridConfig,
	GridFilterBarVisibility,
	GridPagerMode,
	GridPreset,
} from "client-controls";

@graphInfo({
	graphType: "PX.TokenLogin.SupportAccessMaint",
	primaryView: "UserSelect",
})
export class SM202065 extends PXScreen {
	UserSelect = createSingle(Users);
	SecurityPrefs = createSingle(PreferencesSecurity);
	FilterSelect = createSingle(Filter);
	AllowedRoles = createCollection(EPLoginTypeAllowsRole);
	GrantHistory = createCollection(SAGrantHistory);
	LoginTraces = createCollection(LoginTrace);
}

export class Users extends PXView {
	State: PXFieldState<PXFieldOptions.Disabled>;
}

export class PreferencesSecurity extends PXView {
	AllowSupportToLoginAsAnyUser: PXFieldState;
}

export class Filter extends PXView {
	AccessStatus: PXFieldState<
		PXFieldOptions.CommitChanges | PXFieldOptions.Disabled
	>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	showFilterBar: GridFilterBarVisibility.False,
	pagerMode: GridPagerMode.InfiniteScroll,
})
export class EPLoginTypeAllowsRole extends PXView {
	Selected: PXFieldState;

	@columnConfig({ hideViewLink: true })
	Rolename: PXFieldState;

	Rolename_Roles_descr: PXFieldState;
}

@gridConfig({ 
	preset: GridPreset.Inquiry,
	showFilterBar: GridFilterBarVisibility.False
})
export class LoginTrace extends PXView {
	Date: PXFieldState;
	Username: PXFieldState;
	Operation: PXFieldState;
	Host: PXFieldState;
	IPAddress: PXFieldState;
	ScreenID: PXFieldState;
	ScreenID_SiteMap_Title: PXFieldState;
	Comment: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	showFilterBar: GridFilterBarVisibility.False
})
export class SAGrantHistory extends PXView {
	Username: PXFieldState;
	Type: PXFieldState;
	CreatedDateTime: PXFieldState;
}
