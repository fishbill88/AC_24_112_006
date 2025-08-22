import {
	PXScreen,
	graphInfo,
	PXView,
	createCollection,
	PXFieldState,
	gridConfig,
	createSingle,
	linkCommand,
	PXFieldOptions,
	PXActionState,
	GridPreset,
} from "client-controls";

@graphInfo({ graphType: "PX.SM.PreferencesEmailMaint", primaryView: "Prefs" })
export class SM204001 extends PXScreen {
	Prefs = createSingle(Prefs);
	EMailAccounts = createCollection(EMailAccounts);
}

export class Prefs extends PXView {
	DefaultEMailAccountID: PXFieldState;
	EmailTagPrefix: PXFieldState;
	EmailTagSuffix: PXFieldState;
	ArchiveEmailsOlderThan: PXFieldState;
	RepeatOnErrorSending: PXFieldState;
	SuspendEmailProcessing: PXFieldState<PXFieldOptions.CommitChanges>;
	SendUserEmailsImmediately: PXFieldState<PXFieldOptions.CommitChanges>;
	EmailProcessingLogging: PXFieldState;
	EmailProcessingLoggingRetentionPeriod: PXFieldState;
	UserWelcomeNotificationId: PXFieldState;
	PasswordChangedNotificationId: PXFieldState;
	LoginRecoveryNotificationId: PXFieldState;
	PasswordRecoveryNotificationId: PXFieldState;
	TwoFactorNewDeviceNotificationId: PXFieldState;
	TwoFactorCodeByNotificationId: PXFieldState;
	NotificationSiteUrl: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	adjustPageSize: true,
	initNewRow: true,
	syncPosition: true,
	allowUpdate: false,
	showRowSelectors: false,
	suppressNoteFiles: true,
	fastFilterByAllFields: false,
	topBarItems: {
		EMailAccount_New: {
			index: 2,
			config: {
				commandName: "EMailAccount_New",
				images: { normal: "main@RecordAdd" },
			},
		},
		EMailAccount_Delete: {
			index: 3,
			config: {
				commandName: "EMailAccount_Delete",
				images: { normal: "main@Remove" },
			},
		},
	},
})
export class EMailAccounts extends PXView {
	EMailAccount_New: PXActionState;
	EMailAccount_Delete: PXActionState;

	@linkCommand("EMailAccount_View") Description: PXFieldState;
	@linkCommand("ViewUser") UserId: PXFieldState;
	IsActive: PXFieldState;
	Address: PXFieldState;
	LoginName: PXFieldState;
	OutcomingHostName: PXFieldState;
	IncomingHostName: PXFieldState;
}
