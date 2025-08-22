import { PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, createCollection, createSingle, PXScreen, graphInfo } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CT.ContractItemMaint', primaryView: 'ContractItems' })
export class CT201000 extends PXScreen {
	ContractItems = createSingle(ContractItem);
	CurrentContractItem = createSingle(ContractItem);

	// we split DAC to allow grids to show different fields
	CurrentTemplates = createCollection(ContractTemplate);
	CurrentContracts = createCollection(Contract);
}

export class ContractItem extends PXView {
	ContractItemCD: PXFieldState;
	MaxQty: PXFieldState<PXFieldOptions.CommitChanges>;
	MinQty: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultQty: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	BaseItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	BasePriceOption: PXFieldState<PXFieldOptions.CommitChanges>;
	BasePrice: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainRate: PXFieldState<PXFieldOptions.CommitChanges>;
	Deposit: PXFieldState<PXFieldOptions.CommitChanges>;
	Refundable: PXFieldState;
	ProrateSetup: PXFieldState;
	RenewalItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	CollectRenewFeeOnActivation: PXFieldState;
	RenewalPriceOption: PXFieldState<PXFieldOptions.CommitChanges>;
	RenewalPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	BasePriceVal: PXFieldState<PXFieldOptions.Disabled>;
	FixedRecurringPriceVal: PXFieldState<PXFieldOptions.Disabled>;
	UsagePriceVal: PXFieldState<PXFieldOptions.Disabled>;
	RenewalPriceVal: PXFieldState<PXFieldOptions.Disabled>;
	RecurringType: PXFieldState<PXFieldOptions.CommitChanges>;
	RecurringItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	ResetUsageOnBilling: PXFieldState<PXFieldOptions.CommitChanges>;
	FixedRecurringPriceOption: PXFieldState<PXFieldOptions.CommitChanges>;
	FixedRecurringPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	UsagePriceOption: PXFieldState<PXFieldOptions.CommitChanges>;
	UsagePrice: PXFieldState<PXFieldOptions.CommitChanges>;
	DepositItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
}

@gridConfig({ adjustPageSize: true })
export class ContractTemplate extends PXView {
	@columnConfig({ allowUpdate: false })
	ContractCD: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Description: PXFieldState;
}

@gridConfig({ adjustPageSize: true })
export class Contract extends PXView {
	@columnConfig({ allowUpdate: false })
	ContractCD: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CustomerID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Status: PXFieldState;
	@columnConfig({ allowUpdate: false })
	StartDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	ExpireDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Description: PXFieldState;
}
