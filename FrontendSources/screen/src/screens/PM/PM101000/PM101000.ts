import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo
} from 'client-controls';

import {
	Setup,
	Markups,
	Notifications,
	Recipients,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PM.SetupMaint',
	primaryView: 'Setup'
})
export class PM101000 extends PXScreen {
	Refresh: PXActionState;

	Setup = createSingle(Setup);
	Markups = createCollection(Markups);
	Notifications = createCollection(Notifications);
	Recipients = createCollection(Recipients);
}

