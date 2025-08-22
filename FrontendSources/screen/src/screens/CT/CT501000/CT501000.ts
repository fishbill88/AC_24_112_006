import {
	createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, PXActionState
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CT.ContractBilling', primaryView: 'Filter' })
export class CT501000 extends PXScreen {

	editDetail: PXActionState;

	Filter = createSingle(BillingFilter);
	Items = createCollection(Contract);

}

export class BillingFilter extends PXView {

	InvoiceDate: PXFieldState<PXFieldOptions.CommitChanges>;
	TemplateID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ syncPosition: true, mergeToolbarWith: 'ScreenToolbar' })
export class Contract extends PXView {

	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;

	@linkCommand('editDetail')
	@columnConfig({ allowUpdate: false })
	ContractCD: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Description: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	CustomerID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Customer__AcctName: PXFieldState;

	@columnConfig({ allowUpdate: false })
	ContractBillingSchedule__LastDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	ContractBillingSchedule__NextDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	ExpireDate: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	TemplateID: PXFieldState;

}
