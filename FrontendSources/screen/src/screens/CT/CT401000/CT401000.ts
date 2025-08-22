import {
	createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, PXActionState
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CT.ExpiringContractsEng', primaryView: 'Filter' })
export class CT401000 extends PXScreen {

	viewContract: PXActionState;

	Filter = createSingle(ExpiringContractFilter);
	Contracts = createCollection(Contract);
}

export class ExpiringContractFilter extends PXView {
	BeginDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowAutoRenewable: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	TemplateID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ syncPosition: true, mergeToolbarWith: 'ScreenToolbar' })
export class Contract extends PXView {

	@linkCommand('viewContract')
	ContractCD: PXFieldState;

	Description: PXFieldState;

	@columnConfig({hideViewLink: true})
	TemplateID: PXFieldState;

	Type: PXFieldState;

	@columnConfig({hideViewLink: true})
	CustomerID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Customer__AcctName: PXFieldState;

	Status: PXFieldState;

	StartDate: PXFieldState;
	ExpireDate: PXFieldState;
	AutoRenew: PXFieldState;

	@columnConfig({ allowUpdate: false })
	ContractBillingSchedule__LastDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	ContractBillingSchedule__NextDate: PXFieldState;
}
