import {
	graphInfo,
	createSingle,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.ServiceTemplateMaint', primaryView: 'ServiceTemplateRecords' })
export class FS204900 extends PXScreen {
	ServiceTemplateRecords = createSingle(FSServiceTemplate);
	ServiceTemplateDetails = createCollection(FSServiceTemplateDet);
}

export class FSServiceTemplateDet extends PXView {
	LineType: PXFieldState;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState;
	TranDesc: PXFieldState;
}

export class FSServiceTemplate extends PXView {
	ServiceTemplateCD: PXFieldState;
	Descr: PXFieldState;
	SrvOrdType: PXFieldState<PXFieldOptions.CommitChanges>;
}
