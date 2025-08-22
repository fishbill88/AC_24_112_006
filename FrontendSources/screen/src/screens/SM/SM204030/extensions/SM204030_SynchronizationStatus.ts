import {
	SM204030
} from '../SM204030';

import {
	PXView,
	createCollection,
	createSingle,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	gridConfig,
	GridPreset
} from 'client-controls';

export interface SM204030_SynchronizationStatus extends SM204030 { }

export class SM204030_SynchronizationStatus {
	ResetContacts: PXActionState;
	ResetTasks: PXActionState;
	ResetEvents: PXActionState;
	ResetEmails: PXActionState;

	CurrentItem = createSingle(EMailSyncAccountFilter);
	OperationLog = createCollection(EMailSyncLog);
}

export class EMailSyncAccountFilter extends PXView {
	ServerID: PXFieldState<PXFieldOptions.Disabled>;
	Address: PXFieldState<PXFieldOptions.Disabled>;
	ContactsExportDate_Date: PXFieldState;
	ContactsExportDate_Time: PXFieldState;
	ContactsImportDate_Date: PXFieldState<PXFieldOptions.Disabled>;
	ContactsImportDate_Time: PXFieldState<PXFieldOptions.Disabled>;
	TasksExportDate_Date: PXFieldState;
	TasksExportDate_Time: PXFieldState;
	TasksImportDate_Date: PXFieldState<PXFieldOptions.Disabled>;
	TasksImportDate_Time: PXFieldState<PXFieldOptions.Disabled>;
	EventsExportDate_Date: PXFieldState;
	EventsExportDate_Time: PXFieldState;
	EventsImportDate_Date: PXFieldState<PXFieldOptions.Disabled>;
	EventsImportDate_Time: PXFieldState<PXFieldOptions.Disabled>;
	EmailsExportDate_Date: PXFieldState;
	EmailsExportDate_Time: PXFieldState;
	EmailsImportDate_Date: PXFieldState<PXFieldOptions.Disabled>;
	EmailsImportDate_Time: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Details,
})
class EMailSyncLog extends PXView {
	ClearLog: PXActionState;
	ResetWarning: PXActionState;

	ServerID: PXFieldState;
	Address: PXFieldState;
	Level: PXFieldState;
	Date: PXFieldState;
	Message: PXFieldState;
}
