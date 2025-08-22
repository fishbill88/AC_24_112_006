import {
	createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, PXActionState
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CT.ContractPriceUpdate', primaryView: 'Filter', hideFilesIndicator: true, hideNotesIndicator: true })
export class CT503000 extends PXScreen {

	ViewContract: PXActionState;

	Filter = createSingle(ContractFilter);
	Items = createCollection(ContractDetail);
	SelectedContractItem = createSingle(ContractDetail);
}

export class ContractFilter extends PXView {

	ContractItemID: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ syncPosition: true, adjustPageSize: true })
export class ContractDetail extends PXView {

	@columnConfig({ allowUpdate: false, allowCheckAll: true })
	Selected: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Contract__StrIsTemplate: PXFieldState;

	@linkCommand('ViewContract')
	@columnConfig({ allowUpdate: false })
	Contract__ContractCD: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Contract__Status: PXFieldState;

	@columnConfig({ allowUpdate: false })
	BasePriceOption: PXFieldState;

	@columnConfig({ allowUpdate: false })
	BasePrice: PXFieldState;

	@columnConfig({ allowUpdate: false })
	BasePriceVal: PXFieldState;

	@columnConfig({ allowUpdate: false })
	RenewalPriceOption: PXFieldState;

	@columnConfig({ allowUpdate: false })
	RenewalPrice: PXFieldState;

	@columnConfig({ allowUpdate: false })
	RenewalPriceVal: PXFieldState;

	@columnConfig({ allowUpdate: false })
	FixedRecurringPriceOption: PXFieldState;

	@columnConfig({ allowUpdate: false })
	FixedRecurringPrice: PXFieldState;

	@columnConfig({ allowUpdate: false })
	FixedRecurringPriceVal: PXFieldState;

	@columnConfig({ allowUpdate: false })
	UsagePriceOption: PXFieldState;

	@columnConfig({ allowUpdate: false })
	UsagePrice: PXFieldState;

	@columnConfig({ allowUpdate: false })
	UsagePriceVal: PXFieldState;

}
