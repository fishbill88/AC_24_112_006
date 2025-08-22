import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo
} from 'client-controls';

import {Carrier,
	Carrier2,
	FreightRate,
	CarrierPackage
} from './views';

@graphInfo({graphType: 'PX.Objects.CS.CarrierMaint', primaryView: 'Carrier'})
export class CS207500 extends PXScreen {

	@viewInfo({containerName: 'Ship Via Summary'})
	Carrier = createSingle(Carrier);

	@viewInfo({containerName: 'Details'})
	CarrierCurrent = createSingle(Carrier2);

	@viewInfo({containerName: 'Freight Rates'})
	FreightRates = createCollection(FreightRate);

	@viewInfo({containerName: 'Packages'})
	CarrierPackages = createCollection(CarrierPackage);
}