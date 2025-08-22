import {
	PXView,
	PXFieldState,
	gridConfig,
	PXFieldOptions,
	columnConfig
} from 'client-controls';

export class Carrier extends PXView  {
	CarrierID: PXFieldState;
	Description: PXFieldState;
	IsExternal: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class Carrier2 extends PXView  {
	CalendarID: PXFieldState;
	CarrierPluginID: PXFieldState<PXFieldOptions.CommitChanges>;
	CalcMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	PluginMethod: PXFieldState;
	BaseRate: PXFieldState;
	IsCommonCarrier: PXFieldState;
	CalcFreightOnReturn: PXFieldState<PXFieldOptions.CommitChanges>;
	ConfirmationRequired: PXFieldState;
	PackageRequired: PXFieldState;
	ReturnLabel: PXFieldState;
	TaxCategoryID: PXFieldState;
	FreightSalesAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	FreightSalesSubID: PXFieldState;
	FreightExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	FreightExpenseSubID: PXFieldState;
	ValidatePackedQty: PXFieldState;
	IsExternalShippingApplication: PXFieldState<PXFieldOptions.CommitChanges>;
	ShippingApplicationType: PXFieldState;
}

export class FreightRate extends PXView  {
	Weight: PXFieldState;
	Volume: PXFieldState;
	ZoneID: PXFieldState;
	Rate: PXFieldState;
}

@gridConfig({
	adjustPageSize: true
})
export class CarrierPackage extends PXView  {
	@columnConfig({ hideViewLink: true })
	BoxID: PXFieldState;
	CSBox__Description: PXFieldState;
	CSBox__BoxWeight: PXFieldState;
	CSBox__MaxWeight: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CommonSetup__WeightUOM: PXFieldState;
	CSBox__MaxVolume: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CommonSetup__VolumeUOM: PXFieldState;
	CSBox__Length: PXFieldState;
	CSBox__Width: PXFieldState;
	CSBox__Height: PXFieldState;

}