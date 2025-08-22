import {
	PXScreen, createSingle, createCollection, graphInfo, PXView, PXFieldState, gridConfig, columnConfig, PXFieldOptions
} from 'client-controls';

@gridConfig({ adjustPageSize: true, syncPosition: true, mergeToolbarWith: 'ScreenToolbar' })
export class FADocumentList extends PXView {
	BranchID: PXFieldState;

	@columnConfig({ allowCheckAll: true, allowNull: false, allowUpdate: false })
	Selected: PXFieldState;

	@columnConfig({ allowUpdate: false })
	RefNbr: PXFieldState;

	@columnConfig({ allowNull: false, allowUpdate: false })
	Origin: PXFieldState;

	@columnConfig({ allowNull: false, allowUpdate: false })
	Status: PXFieldState;

	@columnConfig({ allowUpdate: false })
	DocDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	FinPeriodID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	DocDesc: PXFieldState;
}

export class Filter extends PXView {
	Origin: PXFieldState<PXFieldOptions.CommitChanges>;
}

@graphInfo({ graphType: 'PX.Objects.FA.AssetTranRelease', primaryView: 'Filter' })
export class FA503000 extends PXScreen {
	Filter = createSingle(Filter);
	FADocumentList = createCollection(FADocumentList);
}
