import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo,
	viewInfo
} from 'client-controls';

import {
	Filter,
	DocumentAddresses,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PM.ValidatePMDocumentAddressProcess',
	primaryView: 'Filter'
})
export class PM507000 extends PXScreen {
	ViewDocument: PXActionState;

	Filter = createSingle(Filter);
	DocumentAddresses = createCollection(DocumentAddresses);
}

