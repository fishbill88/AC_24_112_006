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
	graphType: 'PX.Objects.PM.UnionMaint',
	primaryView: 'Items',
	hideFilesIndicator: true,
	hideNotesIndicator: true,
})
export class PM209700 extends PXScreen {
	Items = createCollection(Items);
}
