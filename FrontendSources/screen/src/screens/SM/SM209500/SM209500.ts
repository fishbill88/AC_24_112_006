import { PXView, PXFieldState, gridConfig, columnConfig, createCollection, PXScreen, graphInfo } from 'client-controls';

@graphInfo({graphType: 'PX.Objects.SM.FullTextIndexRebuild', primaryView: 'Items' })
export class SM209500 extends PXScreen {
	Items = createCollection(Items);
}

// Views

@gridConfig({adjustPageSize: true, mergeToolbarWith: "ScreenToolbar"})
export class Items extends PXView {
    @columnConfig({allowUpdate: false, allowCheckAll: true})
    Selected: PXFieldState;
    @columnConfig({allowUpdate: false})
    DisplayName: PXFieldState;
    @columnConfig({allowUpdate: false})
    Name: PXFieldState;
}