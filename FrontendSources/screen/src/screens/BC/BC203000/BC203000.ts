import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
} from "client-controls";

import {
	MasterView,
	OptionMappings
} from './views';

@graphInfo({ graphType: 'PX.Commerce.Objects.BCMatrixOptionsMappingMaint', primaryView: 'MasterView' })
export class BC203000 extends PXScreen {
	MasterView = createSingle(MasterView);

	OptionMappings = createCollection(OptionMappings);
}
