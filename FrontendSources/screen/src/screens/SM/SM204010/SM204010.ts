import {
	createCollection, createSingle, PXScreen, graphInfo, PXView,
	PXFieldState, PXFieldOptions, gridConfig, columnConfig, GridPreset
} from "client-controls";

@graphInfo({ graphType: 'PX.SM.EMailSyncPolicyMaint', primaryView: 'SyncPolicy' })
export class SM204010 extends PXScreen {
	SyncPolicy = createSingle(EMailSyncPolicy);
	Preferences = createCollection(EMailSyncAccountPreferences);
}

class EMailSyncPolicy extends PXView {
	PolicyName: PXFieldState
	Description: PXFieldState;

	Category: PXFieldState<PXFieldOptions.CommitChanges>;
	LinkTemplate: PXFieldState;
	Priority: PXFieldState<PXFieldOptions.CommitChanges>;
	Color: PXFieldState<PXFieldOptions.CommitChanges>;
	SkipError: PXFieldState<PXFieldOptions.CommitChanges>;

	ContactsSync: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactsSeparated: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactsMerge: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactsSkipCategory: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactsGenerateLink: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactsDirection: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactsFolder: PXFieldState;
	ContactsFilter: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactsClass: PXFieldState<PXFieldOptions.CommitChanges>;

	EmailsSync: PXFieldState<PXFieldOptions.CommitChanges>;
	EmailsAttachments: PXFieldState;
	EmailsFolder: PXFieldState;

	TasksSync: PXFieldState<PXFieldOptions.CommitChanges>;
	TasksSeparated: PXFieldState<PXFieldOptions.CommitChanges>;
	TasksSkipCategory: PXFieldState<PXFieldOptions.CommitChanges>;
	TasksDirection: PXFieldState;
	TasksFolder: PXFieldState;

	EventsSync: PXFieldState<PXFieldOptions.CommitChanges>;
	EventsSeparated: PXFieldState<PXFieldOptions.CommitChanges>;
	EventsSkipCategory: PXFieldState<PXFieldOptions.CommitChanges>;
	EventsDirection: PXFieldState;
	EventsFolder: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	suppressNoteFiles: true
})
class EMailSyncAccountPreferences extends PXView {
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ width: 150 }) EmployeeCD: PXFieldState;
	Address: PXFieldState<PXFieldOptions.Readonly>;
}
