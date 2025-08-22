import {
	PXActionState,
	PXScreen,
	createSingle,
	graphInfo
} from 'client-controls';

import { Messages as SysMessages } from 'client-controls/services/messages';

import {
	Locations,
	Address,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PR.WorkLocationsMaint',
	primaryView: 'Locations'
})
export class PR101040 extends PXScreen {
	SysMessages = SysMessages;

	AddressLookup: PXActionState;
	ViewOnMap: PXActionState;

	Locations = createSingle(Locations);
	Address = createSingle(Address);
}

