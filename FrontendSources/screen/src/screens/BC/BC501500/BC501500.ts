import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
} from "client-controls";

import {
	ProcessFilter,
	Statuses
} from './views';

@graphInfo({ graphType: 'PX.Commerce.Core.BCProcessData', primaryView: 'ProcessFilter' })
export class BC501500 extends PXScreen {
	ProcessFilter = createSingle(ProcessFilter);

	Statuses = createCollection(Statuses);
}
