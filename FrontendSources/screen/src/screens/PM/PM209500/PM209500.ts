import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo
} from 'client-controls';

import {
	ChangeIDDialog,
	Items,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PM.CostCodeMaint',
	primaryView: 'Items',
	hideFilesIndicator: true,
	hideNotesIndicator: true
})
export class PM209500 extends PXScreen {
	Save: PXActionState;
	Cancel: PXActionState;

	Items = createCollection(Items);
	ChangeIDDialog = createSingle(ChangeIDDialog);
}

