import { autoinject } from 'aurelia-framework';
import {
	commitChanges,
	createCollection,
	createSingle,
	graphInfo,
	gridConfig,
	PXActionState,
	PXFieldState,
	PXScreen,
	PXView
} from 'client-controls';

@graphInfo({
	graphType: 'PX.MSGraph.GraphSetupMaint',
	primaryView: 'GraphSetup'
})
@autoinject
export class SM220000 extends PXScreen {
	GraphSetup = createSingle(GraphSetup);

	AccessRights = createCollection(AccessRights);
}

export class GraphSetup extends PXView {
	TenantID: PXFieldState;
	ClientID: PXFieldState;
	ClientSecret: PXFieldState;
	Timeout: PXFieldState;
}

@gridConfig({
	allowDelete: false,
	allowInsert: false
})
export class AccessRights extends PXView {
	InsertRights: PXActionState;
	Enabled: PXFieldState;
	@commitChanges AccessRight: PXFieldState;
	Description: PXFieldState;
	AdminConsent: PXFieldState;
}
