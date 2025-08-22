import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
} from "client-controls";

import {
	ProcessFilter,
	Entities
} from './views';

@graphInfo({ graphType: 'PX.Commerce.Core.BCPrepareData', primaryView: 'ProcessFilter' })
export class BC501000 extends PXScreen {
	ProcessFilter = createSingle(ProcessFilter);

	Entities = createCollection(Entities);
}
