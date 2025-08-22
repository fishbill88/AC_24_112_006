import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo,
	viewInfo
} from 'client-controls';

import {
	Items,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PM.RegisterRelease',
	primaryView: 'Items'
})
export class PM501000 extends PXScreen {
	Items = createCollection(Items);
}

