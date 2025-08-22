import {
	PXScreen, createCollection, graphInfo, PXView, gridConfig,
	PXFieldState,
	columnConfig
} from 'client-controls';

@gridConfig({ adjustPageSize: true, syncPosition: true, mergeToolbarWith: 'ScreenToolbar' })
export class FARegister extends PXView {
	BranchID: PXFieldState;

	@columnConfig({ allowCheckAll: true, allowNull: false })
	Selected: PXFieldState;

	RefNbr: PXFieldState;

	DocDate: PXFieldState;

	@columnConfig({ allowNull: false })
	Origin: PXFieldState;

	DocDesc: PXFieldState;

	@columnConfig({ allowNull: false })
	Hold: PXFieldState;

	@columnConfig({ allowNull: false })
	IsEmpty: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.FA.DeleteDocsProcess', primaryView: 'Docs' })
export class FA508000 extends PXScreen {
	Docs = createCollection(FARegister);
}
