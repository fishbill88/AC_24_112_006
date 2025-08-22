import {
	PXView, PXFieldState, graphInfo, PXScreen, createSingle, PXFieldOptions
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.CR.CRSMEmailMaint', primaryView: 'Email' })
export class CR306035 extends PXScreen {
	Email = createSingle(Email);
}

export class Email extends PXView {
	MailFrom: PXFieldState<PXFieldOptions.Disabled>;
	MailTo: PXFieldState<PXFieldOptions.Disabled>;
	mailCc: PXFieldState<PXFieldOptions.Disabled>;
	mailBcc: PXFieldState<PXFieldOptions.Disabled>;
	Subject: PXFieldState<PXFieldOptions.Disabled>;
	MPStatus: PXFieldState<PXFieldOptions.Disabled>;
	RedException: PXFieldState<PXFieldOptions.Disabled>;
	Body: PXFieldState<PXFieldOptions.Disabled>;
}
