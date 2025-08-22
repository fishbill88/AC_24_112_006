import {
	createSingle,
	PXScreen,
	PXView,
	graphInfo,
	PXFieldState,
	PXFieldOptions,
	createCollection,
	gridConfig,
	GridPreset
} from 'client-controls';

@graphInfo({ graphType: 'PX.SM.EMailAccountMaint', primaryView: 'EMailAccounts' })
export class SM204002 extends PXScreen {
	EMailAccounts = createSingle(EMailAccount);
	Details = createCollection(EMailAccountPluginDetail);
}

export class EMailAccount extends PXView {
	EmailAccountID: PXFieldState;
	UserId: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.CommitChanges>;
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	Address: PXFieldState;
	ReplyAddress: PXFieldState;
	PluginTypeName: PXFieldState<PXFieldOptions.CommitChanges>;
	IsOfPluginType: PXFieldState<PXFieldOptions.Hidden>;
	IncomingHostProtocol: PXFieldState<PXFieldOptions.CommitChanges>;
	ImapRootFolder: PXFieldState;
	IncomingHostName: PXFieldState;
	OutcomingHostName: PXFieldState;
	SendGroupMails: PXFieldState;
	AuthenticationMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	OAuthApplicationID: PXFieldState<PXFieldOptions.CommitChanges>;
	AzureTenantID: PXFieldState;
	LoginName: PXFieldState;
	Password: PXFieldState;
	OutcomingAuthenticationRequest: PXFieldState<PXFieldOptions.CommitChanges>;
	OutcomingAuthenticationDifferent: PXFieldState<PXFieldOptions.CommitChanges>;
	OutcomingLoginName: PXFieldState;
	OutcomingPassword: PXFieldState;
	ValidateFrom: PXFieldState<PXFieldOptions.CommitChanges>;
	IncomingPort: PXFieldState;
	IncomingSSLRequest: PXFieldState<PXFieldOptions.CommitChanges>;
	OutcomingPort: PXFieldState;
	OutcomingSSLRequest: PXFieldState<PXFieldOptions.CommitChanges>;
	Timeout: PXFieldState<PXFieldOptions.CommitChanges>;
	FetchingBehavior: PXFieldState<PXFieldOptions.CommitChanges>;
	SenderDisplayNameSource: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountDisplayName: PXFieldState;
	IncomingProcessing: PXFieldState<PXFieldOptions.CommitChanges>;
	AddIncomingProcessingTags: PXFieldState;
	ConfirmReceipt: PXFieldState;
	ConfirmReceiptNotificationID: PXFieldState;
	CreateCase: PXFieldState<PXFieldOptions.CommitChanges>;
	CreateCaseClassID: PXFieldState;
	RouteEmployeeEmails: PXFieldState<PXFieldOptions.CommitChanges>;
	CreateActivity: PXFieldState;
	CreateLead: PXFieldState<PXFieldOptions.CommitChanges>;
	CreateLeadClassID: PXFieldState;
	ProcessUnassigned: PXFieldState<PXFieldOptions.CommitChanges>;
	ResponseNotificationID: PXFieldState;
	SubmitToIncomingAPDocuments: PXFieldState<PXFieldOptions.CommitChanges>;
	DeleteUnProcessed: PXFieldState<PXFieldOptions.CommitChanges>;
	TypeDelete: PXFieldState;
	AddUpInformation: PXFieldState;
	IncomingDelSuccess: PXFieldState;
	IncomingAttachmentType: PXFieldState;
	DefaultEmailAssignmentMapID: PXFieldState;
	DefaultWorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultOwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class EMailAccountPluginDetail extends PXView {
	SettingID: PXFieldState;
	Description: PXFieldState;
	Value: PXFieldState;
}
