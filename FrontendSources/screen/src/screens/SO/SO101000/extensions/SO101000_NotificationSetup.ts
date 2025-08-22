import {
	PXView,
	PXFieldState,
	PXFieldOptions,

	createCollection,

	viewInfo,
	gridConfig,
	columnConfig,
	GridPreset,
	GridColumnDisplayMode,
} from 'client-controls';

import { SO101000 } from '../SO101000';

export interface SO101000_NotificationSetup extends SO101000 { }
export class SO101000_NotificationSetup {
	@viewInfo({containerName: "Default Sources"})
	Notifications = createCollection(Notifications);

	@viewInfo({containerName: "Default Recipients"})
	Recipients = createCollection(Recipients);
}

@gridConfig({
	preset: GridPreset.Details,
	autoRepaint: ['Recipients']
})
export class Notifications extends PXView {
	Active: PXFieldState<PXFieldOptions.CommitChanges>;
	NotificationCD: PXFieldState;

	@columnConfig({ hideViewLink: true })
	NBranchID: PXFieldState;

	@columnConfig({ hideViewLink: true, displayMode: GridColumnDisplayMode.Text })
	EMailAccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DefaultPrinterID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ReportID: PXFieldState;

	NotificationID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ShipVia: PXFieldState;

	Format: PXFieldState;
	RecipientsBehavior: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ preset: GridPreset.Details })
export class Recipients extends PXView {
	Active: PXFieldState;
	ContactType: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactID: PXFieldState<PXFieldOptions.CommitChanges>;
	Format: PXFieldState;
	AddTo: PXFieldState;
}
