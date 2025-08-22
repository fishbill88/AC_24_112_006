import { graphInfo, PXScreen, PXView, PXFieldState, commitChanges, createSingle } from "client-controls";
import refreshHub from "./refresh-hub";

@graphInfo({ graphType: 'PX.OAuthClient.ApplicationMaint', primaryView: 'Applications', bpEventsIndicator: false })
export class SM301000 extends PXScreen {
	Applications = createSingle(Applications);

	constructor() {
		super();
		refreshHub.subscribeOnHub(this.Applications, () => this.screenService.update());
	}
}

export class Applications extends PXView {
	ApplicationID: PXFieldState;
	@commitChanges Type: PXFieldState;
	ApplicationName: PXFieldState;
	ClientID: PXFieldState;
	@commitChanges ClientSecret: PXFieldState;
	OAuthToken: OAuthToken;
	ReturnUrl: PXFieldState;
}

export class OAuthToken {
	Bearer: PXFieldState;
	UtcExpiredOn: PXFieldState;
}

