import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, PXFieldOptions, createCollection, columnConfig, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AP.APAccessDetail', primaryView: 'Vendor' })
export class AP102010 extends PXScreen {

	Vendor = createSingle(Vendor);
	Groups = createCollection(RelationGroup);

}

export class Vendor extends PXView {

	AcctCD: PXFieldState;

	@columnConfig({ allowNull: false })
	VStatus: PXFieldState;

	AcctName: PXFieldState;

}

@gridConfig({ allowInsert: false, allowDelete:false })
export class RelationGroup extends PXView {

	@columnConfig({ allowCheckAll: true })
	Included: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true }) 
	GroupName: PXFieldState;

	@columnConfig({ allowUpdate: false }) 
	Description: PXFieldState;

	@columnConfig({ allowUpdate: false }) 
	Active: PXFieldState;

	@columnConfig({ allowUpdate: false }) 
	GroupType: PXFieldState;

}
