import {
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	viewInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
} from "client-controls";

@graphInfo({
	graphType: "PX.SM.PreferencesSecurityMaint",
	primaryView: "Prefs",
	hideFilesIndicator: true,
	hideNotesIndicator: true,
})
export class SM201060 extends PXScreen {
	GenerateCodes: PXActionState;

	@viewInfo({ containerName: "General Settings" })
	Prefs = createSingle(PreferencesSecurity);
	@viewInfo({ containerName: "Global Settings" })
	PrefsGlobal = createSingle(PreferencesGlobal);
	@viewInfo({ containerName: "Confirm" })
	ConfirmView = createSingle(ConfirmAccessCode);
}

export class PreferencesSecurity extends PXView {
	IsPasswordDayAge: PXFieldState<PXFieldOptions.CommitChanges>;
	PasswordDayAge: PXFieldState;
	IsPasswordMinLength: PXFieldState<PXFieldOptions.CommitChanges>;
	PasswordMinLength: PXFieldState;
	PasswordComplexity: PXFieldState;
	PasswordRegexCheck: PXFieldState;
	PasswordRegexCheckMessage: PXFieldState;
	MultiFactorAuthLevel: PXFieldState;
	EmailEnabled: PXFieldState;
	SmsEnabled: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountLockoutThreshold: PXFieldState;
	AccountLockoutDuration: PXFieldState;
	AccountLockoutReset: PXFieldState;
	DBCertificateName: PXFieldState;
	PdfCertificateName: PXFieldState;
	DefaultMenuEditorRole: PXFieldState<PXFieldOptions.CommitChanges>;
	TraceMonthsKeep: PXFieldState;
	TraceOperationLogin: PXFieldState;
	TraceOperationLoginFailed: PXFieldState;
	TraceOperationLogout: PXFieldState;
	TraceOperationAccessScreen: PXFieldState;
	TraceOperationSessionExpired: PXFieldState;
	TraceOperationLicenseExceeded: PXFieldState;
	TraceOperationSendMail: PXFieldState;
	TraceOperationSendMailFailed: PXFieldState;
	TraceOperationODataRefresh: PXFieldState;
	TraceOperationCustomizationPublished: PXFieldState;
}

export class PreferencesGlobal extends PXView {
	UsePreconfiguredTimeouts: PXFieldState<PXFieldOptions.CommitChanges>;
	LogoutTimeout: PXFieldState<PXFieldOptions.CommitChanges>;
	PreconfiguredLogoutTimeout: PXFieldState;
}

export class ConfirmAccessCode extends PXView {
	ConfirmText: PXFieldState<PXFieldOptions.Disabled>;
	AccessCode: PXFieldState<PXFieldOptions.CommitChanges>;
	BackupText: PXFieldState<PXFieldOptions.Disabled>;
	ConfigureApiText: PXFieldState<PXFieldOptions.Disabled>;
}
