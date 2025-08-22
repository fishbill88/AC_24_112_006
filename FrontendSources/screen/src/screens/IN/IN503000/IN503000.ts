import { Messages as SysMessages } from 'client-controls/services/messages';
import { MassConvertStockNonStockFilter,InventoryItem,InventoryLinkFilter } from './views';
import { PXActionState, PXScreen, createCollection, createSingle, graphInfo, viewInfo } from 'client-controls';

@graphInfo({graphType: 'PX.Objects.IN.MassConvertStockNonStock', primaryView: 'Filter', bpEventsIndicator: false, udfTypeField: ''})
export class IN503000 extends PXScreen {
	SelectItems: PXActionState;

	@viewInfo({containerName: 'Selection'})
	Filter = createSingle(MassConvertStockNonStockFilter);
	@viewInfo({containerName: 'Details'})
	ItemList = createCollection(InventoryItem);
	@viewInfo({containerName: 'Inventory Item List'})
	SelectedItems = createCollection(InventoryLinkFilter);
}
