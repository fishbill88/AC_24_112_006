import { 
	createCollection,
	PXScreen,
	graphInfo,
	viewInfo,
	PXView,
	PXFieldState,
	gridConfig
} from 'client-controls';

@graphInfo({graphType: 'PX.Objects.IN.INSubItemMaint', primaryView: 'SubItemRecords'})
export class IN205000 extends PXScreen {

	@viewInfo({containerName: 'Subitems'})
	SubItemRecords = createCollection(INSubItem);
}

@gridConfig({
	adjustPageSize: true,
	mergeToolbarWith: 'ScreenToolbar'
})
export class INSubItem extends PXView  {
	SubItemID: PXFieldState;
	SubItemCD: PXFieldState;
	Descr: PXFieldState;
}