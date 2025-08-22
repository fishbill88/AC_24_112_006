import { PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, PXActionState } from 'client-controls';

// Views

export class MobileNotification extends PXView {
	NotificationID: PXFieldState;
	Name: PXFieldState;
	Subject: PXFieldState;
	ScreenID: PXFieldState<PXFieldOptions.CommitChanges>;
	DeliveryType: PXFieldState<PXFieldOptions.CommitChanges>;
	NTo: PXFieldState<PXFieldOptions.CommitChanges>;
	NFrom: PXFieldState;
	ScreenIdValue: PXFieldState;
	LocaleName: PXFieldState;
	DestinationScreenID: PXFieldState<PXFieldOptions.CommitChanges>;
	DestinationEntityID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowSendByEventsTabExpr: PXFieldState;
}

export class MobileNotification2 extends PXView {
	Body: PXFieldState<PXFieldOptions.Multiline>;
}

@gridConfig({syncPosition: true, allowUpdate: false, autoAdjustColumns: true})
export class BPEvent extends PXView {
	CreateBusinessEvent: PXActionState;

	@linkCommand('ViewBusinessEvent') Name: PXFieldState;
	Description: PXFieldState;
	Active: PXFieldState;
	Type: PXFieldState;
}

export class BPEventData extends PXView {
	Name: PXFieldState<PXFieldOptions.CommitChanges>;
}