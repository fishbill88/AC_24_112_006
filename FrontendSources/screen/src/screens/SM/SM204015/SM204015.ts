import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	columnConfig,
	GridPreset,
} from "client-controls";

@graphInfo({
	graphType: "PX.SM.EMailSyncServerMaint",
	primaryView: "Servers",
})
export class SM204015 extends PXScreen {
	Servers = createSingle(EMailSyncServer);
	SyncAccounts = createCollection(EMailSyncAccount);
}

class EMailSyncServer extends PXView {
	AccountCD: PXFieldState;
	Address: PXFieldState;
	AuthenticationMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	AzureTenantID: PXFieldState;
	OAuthApplicationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Password: PXFieldState;
	IsActive: PXFieldState;
	DefaultPolicyName: PXFieldState;
	ServerUrl: PXFieldState;
	LoggingLevel: PXFieldState;
	ConnectionMode: PXFieldState;
	SyncProcBatch: PXFieldState;
	SyncUpdateBatch: PXFieldState;
	SyncSelectBatch: PXFieldState;
	SyncAttachmentSize: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	suppressNoteFiles: true,
})
class EMailSyncAccount extends PXView {
	@columnConfig({ allowUpdate: false })
	SyncAccount: PXFieldState;
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowUpdate: false, width: 150 })
	EmployeeCD: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Address: PXFieldState;
	@columnConfig({ allowUpdate: false })
	PolicyName: PXFieldState;
}
