import {
	createSingle,
	PXScreen,
	PXView,
	graphInfo,
	PXFieldState,
	PXFieldOptions,
} from 'client-controls';


@graphInfo({ graphType: 'PX.Objects.EP.EPEventSetupMaint', primaryView: 'Setup' })
export class EP204070 extends PXScreen {
	Setup = createSingle(EPSetup);
}

export class EPSetup extends PXView {
	SendOnlyEventCard: PXFieldState<PXFieldOptions.CommitChanges>;
	IsSimpleNotification: PXFieldState<PXFieldOptions.CommitChanges>;
	AddContactInformation: PXFieldState;
	InvitationTemplateID: PXFieldState<PXFieldOptions.CommitChanges>;
	RescheduleTemplateID: PXFieldState<PXFieldOptions.CommitChanges>;
	CancelInvitationTemplateID: PXFieldState<PXFieldOptions.CommitChanges>;
}
