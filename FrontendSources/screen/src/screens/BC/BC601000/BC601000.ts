import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
} from "client-controls";

import {
	Filter,
	SelectedItems
} from './views';

@graphInfo({ graphType: 'PX.Commerce.Objects.ProcessPII', primaryView: 'Filter' })
export class BC601000 extends PXScreen {
	Filter = createSingle(Filter);

	SelectedItems = createCollection(SelectedItems);
}
