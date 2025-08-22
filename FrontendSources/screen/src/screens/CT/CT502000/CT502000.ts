import {
	createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, PXActionState
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CT.RenewContracts', primaryView: 'Filter', })
export class CT502000 extends PXScreen {

	editDetail: PXActionState;

	Filter = createSingle(RenewalContractFilter);
	Items = createCollection(ContractsList);
}

export class RenewalContractFilter extends PXView {

	RenewalDate: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	TemplateID: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ syncPosition: true, mergeToolbarWith: 'ScreenToolbar' })
export class ContractsList extends PXView {

	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;

	@linkCommand('editDetail')
	@columnConfig({ allowUpdate: false })
	ContractID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Description: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	TemplateID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Type: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	CustomerID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CustomerName: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Status: PXFieldState;

	@columnConfig({ allowUpdate: false })
	StartDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	ExpireDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	LastDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	NextDate: PXFieldState;

}
