import { graphInfo, PXScreen, PXView, PXFieldState, commitChanges, createSingle, createCollection, PXActionState, headerDescription, gridConfig} from "client-controls";

@graphInfo({ graphType: 'PX.Owin.IdentityServerIntegration.DAC.OAuthClientMaint', primaryView: 'Clients', udfTypeField: 'ClientName' })
export class SM303010 extends PXScreen {
	Clients = createSingle(Clients);

	ClientSecrets = createCollection(OAuthClientSecret);
	RedirectUris = createCollection(OAuthClientRedirectUri);
	Claims = createCollection(OAuthClientClaim);

	AddSharedSecretView = createSingle(AddSharedSecretView);
}

export class Clients extends PXView {
	ClientID: PXFieldState;
	@headerDescription ClientName: PXFieldState;
	Enabled: PXFieldState;
	@commitChanges Flow: PXFieldState;
	@commitChanges Plugin: PXFieldState;
	@commitChanges RefreshMode: PXFieldState;
	@commitChanges AbsoluteLifetimeInDays: PXFieldState;
	@commitChanges InfiniteTokenLifetime: PXFieldState;
	@commitChanges SlidingLifetimeInDays: PXFieldState;
}

@gridConfig({allowInsert: false})
export class OAuthClientSecret extends PXView {
	AddSharedSecret: PXActionState;
	Type: PXFieldState;
	Description: PXFieldState;
	ExpirationUtc: PXFieldState;
}

export class OAuthClientRedirectUri extends PXView {
	RedirectUri: PXFieldState;
}

export class OAuthClientClaim extends PXView {
	Active: PXFieldState;
	ClaimName: PXFieldState;
	Scope: PXFieldState;
	Plugin: PXFieldState;
	Description: PXFieldState;
}

export class AddSharedSecretView extends PXView {
	Description: PXFieldState;
	ExpirationUtc: PXFieldState;
	Value: PXFieldState;
}