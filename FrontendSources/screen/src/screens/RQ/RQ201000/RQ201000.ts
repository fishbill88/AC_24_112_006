import { Messages as SysMessages } from 'client-controls/services/messages';
import { createCollection, createSingle, PXScreen, graphInfo,
	viewInfo, PXView, PXFieldState, PXFieldOptions } from 'client-controls';

@graphInfo({graphType: 'PX.Objects.RQ.RQRequestClassMaint', primaryView: 'Classes' })
export class RQ201000 extends PXScreen {
	@viewInfo({ containerName: 'Request Class' })
	Classes = createSingle(RQRequestClassHeader);
	CurrentClass = createSingle(RQRequestClass);
	@viewInfo({ containerName: 'Item List' })
	ClassItems = createCollection(RQRequestClassItem);
}

export class RQRequestClassHeader extends PXView {
	ReqClassID: PXFieldState;
	Descr: PXFieldState;
	PromisedLeadTime: PXFieldState;
	CustomerRequest: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorNotRequest: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorMultiply: PXFieldState;
	IssueRequestor: PXFieldState;
	RestrictItemList: PXFieldState<PXFieldOptions.CommitChanges>;
	HideInventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class RQRequestClass extends PXView {
	BudgetValidation: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseAccountDefault: PXFieldState;
	ExpenseSubMask: PXFieldState;
	ExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseSubID: PXFieldState;
}

export class RQRequestClassItem extends PXView {
	InventoryID: PXFieldState;
	RQInventoryItem__Descr: PXFieldState;
	RQInventoryItem__ItemClassID: PXFieldState;
	RQInventoryItem__ItemStatus: PXFieldState;
	RQInventoryItem__ItemType: PXFieldState;
}
