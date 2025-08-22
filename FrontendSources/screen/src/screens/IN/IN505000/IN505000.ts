import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	viewInfo,
	PXPageLoadBehavior
} from 'client-controls';

import {
	INRecalculateInventoryFilter,
	InventoryItemCommon,
	InventoryLinkFilter
} from './views';

@graphInfo({graphType: 'PX.Objects.IN.INIntegrityCheck', primaryView: 'Filter', pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues, hideFilesIndicator: true, hideNotesIndicator: true})
export class IN505000 extends PXScreen {
	SelectItems: PXActionState;

	@viewInfo({containerName: 'Selection'})
	Filter = createSingle(INRecalculateInventoryFilter);
	@viewInfo({containerName: 'Details'})
	INItemList = createCollection(InventoryItemCommon);

	@viewInfo({containerName: 'Inventory Item List'})
	SelectedItems = createCollection(InventoryLinkFilter);
}
