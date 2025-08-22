import {
	PXView,
	PXFieldState,
	gridConfig,
	selectorSettings,
	PXFieldOptions,
	columnConfig,
	GridPagerMode,
	GridColumnType,
	PXActionState,
	TextAlign,
} from "client-controls";

// Views

export class Users extends PXView {
	Username: PXFieldState;
	Password: PXFieldState;
	GeneratePassword: PXFieldState<PXFieldOptions.CommitChanges>;
	ForbidLoginWithPassword: PXFieldState<PXFieldOptions.CommitChanges>;
	Guest: PXFieldState<PXFieldOptions.CommitChanges>;
	LoginTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactID: PXFieldState<PXFieldOptions.CommitChanges>;
	FirstName: PXFieldState;
	LastName: PXFieldState;
	Email: PXFieldState<PXFieldOptions.CommitChanges>;
	Comment: PXFieldState;
	State: PXFieldState<PXFieldOptions.Disabled>;
	AllowPasswordRecovery: PXFieldState;
	PasswordChangeable: PXFieldState;
	PasswordNeverExpires: PXFieldState;
	PasswordChangeOnNextLogin: PXFieldState;
	AllowedSessions: PXFieldState;
	OverrideADRoles: PXFieldState<PXFieldOptions.CommitChanges>;
	Source: PXFieldState<PXFieldOptions.Hidden>;
	OverrideLocalRolesWithOidcProviderRoles: PXFieldState<PXFieldOptions.CommitChanges>;
	MultiFactorOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	MultiFactorType: PXFieldState;
	NewPassword: PXFieldState<PXFieldOptions.CommitChanges>;
	ConfirmPassword: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class Users2 extends PXView {
	CreationDate: PXFieldState;
	LastLoginDate: PXFieldState;
	LastLockedOutDate: PXFieldState;
	LastPasswordChangedDate: PXFieldState;
	FailedPasswordAttemptCount: PXFieldState;
	FailedPasswordAnswerAttemptCount: PXFieldState;
}

@gridConfig({ allowDelete: false })
export class EPLoginTypeAllowsRole extends PXView {
	@columnConfig({
		allowSort: false,
		width: 80,
		textAlign: TextAlign.Center,
		type: GridColumnType.CheckBox,
	})
	Selected: PXFieldState;
	@columnConfig({ allowUpdate: false, width: 200 }) Rolename: PXFieldState;
	@columnConfig({ allowUpdate: false, width: 400 })
	Rolename_Roles_descr: PXFieldState;
}

@gridConfig({ fastFilterByAllFields: false })
export class UserFilter extends PXView {
	@columnConfig({ width: 200 }) StartIPAddress: PXFieldState;
	@columnConfig({ width: 200 }) EndIPAddress: PXFieldState;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	fastFilterByAllFields: false,
	syncPosition: true,
	topBarItems: {
		RemoveIdentity: {
			index: 2,
			config: {
				commandName: "removeIdentity",
				images: { normal: "main@RecordDel" },
			},
		},
	},
})
export class OidcUser extends PXView {
	removeIdentity: PXActionState;
	@columnConfig({ width: 180 }) ProviderName: PXFieldState;
	@columnConfig({
		width: 90,
		textAlign: TextAlign.Center,
		type: GridColumnType.CheckBox,
	})
	Active: PXFieldState;
	@columnConfig({ width: 250 }) UserKey: PXFieldState;
	@columnConfig({ width: 250 }) UserIdentityClaimType: PXFieldState;
}

export class UserPreferences extends PXView {
	PdfCertificateName: PXFieldState;
	TimeZone: PXFieldState;
	@selectorSettings("BranchID", "")
	DefBranchID: PXFieldState;
	HomePage: PXFieldState;
	TrackLocation: PXFieldState<PXFieldOptions.CommitChanges>;
	Interval: PXFieldState;
	Distance: PXFieldState;
}

export class Contact extends PXView {
	TeamsID: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultTeamsClient: PXFieldState;
}

@gridConfig({
	allowDelete: false,
	allowInsert: false,
	fastFilterByAllFields: false,
	topBarItems: {
		AddEMailAccount: {
			index: 2,
			config: {
				commandName: "AddEMailAccount",
				images: { normal: "main@RecordAdd" },
				toolTip: "Add Email Account",
			},
		},
	},
})
export class EMailAccount extends PXView {
	AddEMailAccount: PXActionState;
	Description: PXFieldState;
	@columnConfig({ width: 200 }) Address: PXFieldState;
	@columnConfig({
		width: 80,
		textAlign: TextAlign.Center,
		type: GridColumnType.CheckBox,
	})
	IsActive: PXFieldState;
	@columnConfig({ width: 200 }) EmailAccountType: PXFieldState;
	@columnConfig({ width: 200 }) AuthenticationMethod: PXFieldState;
	LoginName: PXFieldState;
}

@gridConfig({
	allowInsert: false,
	fastFilterByAllFields: false,
})
export class MobilePushNotificationRegToken extends PXView {
	deleteDevices: PXActionState;
	disableDevices: PXActionState;
	enableDevices: PXActionState;
	@columnConfig({
		width: 160,
		textAlign: TextAlign.Center,
		type: GridColumnType.CheckBox,
	})
	Enabled: PXFieldState;
	@columnConfig({ width: 200 }) ApplicationInstanceID: PXFieldState;
	@columnConfig({ width: 200 }) DeviceName: PXFieldState;
	@columnConfig({ width: 200 }) DeviceModel: PXFieldState;
	@columnConfig({ width: 200 }) DeviceOS: PXFieldState;
	@columnConfig({
		width: 160,
		textAlign: TextAlign.Center,
		type: GridColumnType.CheckBox,
	})
	ExpiredToken: PXFieldState;
	@columnConfig({
		width: 160,
		textAlign: TextAlign.Center,
		type: GridColumnType.CheckBox,
	})
	IsConfirmation: PXFieldState;
}

@gridConfig({ pagerMode: GridPagerMode.InfiniteScroll })
export class FSGPSTrackingRequest extends PXView {
	@columnConfig({ width: 100 })
	WeekDay: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ width: 100, format: "t" }) StartTime: PXFieldState;
	@columnConfig({ width: 100, format: "t" }) EndTime: PXFieldState;
}

export class ADUserFilter extends PXView {
	Username: PXFieldState<PXFieldOptions.CommitChanges>;
}
