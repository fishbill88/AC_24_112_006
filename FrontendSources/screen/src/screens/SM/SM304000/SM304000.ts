import { graphInfo, PXScreen, PXView, PXFieldState, commitChanges, gridConfig, createSingle, createCollection, PXFieldOptions, PXActionState } from "client-controls";
import { Messages as SysMessages } from "client-controls/services/messages";
@graphInfo({ graphType: 'PX.Api.Webhooks.Graph.WebhookMaint', primaryView: 'Webhook', bpEventsIndicator: false })
export class SM304000 extends PXScreen {
	Webhook = createSingle(Webhook);
	WebhookRequest = createCollection(WebhookRequest);
	WebhookRequestCurrent = createSingle(WebhookRequestCurrent);
}

export class Webhook extends PXView {
	Name: PXFieldState;
	Handler: PXFieldState;
	Url: PXFieldState<PXFieldOptions.Disabled>;
	IsActive: PXFieldState;
	IsSystem: PXFieldState<PXFieldOptions.Disabled>;;
	RequestLogLevel: PXFieldState;
	RequestRetainCount: PXFieldState;
}

@gridConfig({
	adjustPageSize: true,
	syncPosition: true,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false
})
export class WebhookRequest extends PXView {
	Type: PXFieldState;
	ReceivedFrom: PXFieldState;
	ReceiveDate: PXFieldState;
	ResponseStatus: PXFieldState;
	ShowRequestDetails: PXActionState;
	ClearRequestsLog: PXActionState;
}

export class WebhookRequestCurrent extends PXView {
	Type: PXFieldState<PXFieldOptions.Disabled>;
	Request: PXFieldState<PXFieldOptions.Disabled>;
	ResponseStatus: PXFieldState<PXFieldOptions.Disabled>;
	ProcessingTime: PXFieldState<PXFieldOptions.Disabled>;
	Response: PXFieldState<PXFieldOptions.Disabled>;
	Error: PXFieldState<PXFieldOptions.Disabled>;
}

