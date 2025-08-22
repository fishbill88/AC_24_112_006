import { autoinject } from 'aurelia-framework';
import {
	PXActionState,
	PXFieldState,
	PXScreen,
	PXView,
	PXFieldOptions,
	columnConfig,
	createSingle,
	createCollection,
	graphInfo,
	gridConfig,
	viewInfo,
	GridPagerMode,
	GridPreset
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.PR.PRTaxMaintenance', primaryView: 'Filter' })
@autoinject
export class PR208000 extends PXScreen {
	@viewInfo({ containerName: "Filter" })
	Filter = createSingle(Filter);

	@viewInfo({ containerName: "Taxes" })
	Taxes = createCollection(Taxes);

	@viewInfo({ containerName: "TaxAttributes" })
	TaxAttributes = createCollection(TaxAttributes);

	@viewInfo({ containerName: "CompanyAttributes" })
	CompanyAttributes = createCollection(CompanyAttributes);

	@viewInfo({ containerName: "Employees" })
	Employees = createCollection(Employees);
}

export class Filter extends PXView {
	CountryID: PXFieldState;
	FilterStates: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	pagerMode: GridPagerMode.InfiniteScroll,
	fastFilterByAllFields: false,
	preset: GridPreset.Primary,
	autoRepaint: ["TaxAttributes"]
})
export class Taxes extends PXView {
	ViewTaxDetails: PXActionState;

	TaxID: PXFieldState;
	@columnConfig({ width: 200 })
	TaxCD: PXFieldState;
	@columnConfig({ width: 180 })
	Description: PXFieldState;
	@columnConfig({ width: 60 })
	TaxState: PXFieldState;
	@columnConfig({ width: 150 })
	TaxCategory: PXFieldState;
	@columnConfig({ width: 120 })
	BAccountID: PXFieldState;
	@columnConfig({ width: 120 })
	TaxInvDescrType: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ width: 70 })
	ExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ width: 70 })
	ExpenseSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ width: 70 })
	LiabilityAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ width: 70 })
	LiabilitySubID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	pagerMode: GridPagerMode.InfiniteScroll,
	fastFilterByAllFields: false,
	preset: GridPreset.Primary
})
export class TaxAttributes extends PXView {
	@columnConfig({ width: 200 })
	Description: PXFieldState;
	@columnConfig({ width: 250 })
	AdditionalInformation: PXFieldState;
	@columnConfig({ width: 80 })
	AllowOverride: PXFieldState;
	@columnConfig({ width: 100 })
	Value: PXFieldState<PXFieldOptions.CommitChanges>;
	Required: PXFieldState<PXFieldOptions.CommitChanges>;
	FormBox: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false,
})
export class CompanyAttributes extends PXView {
	@columnConfig({ width: 400 })
	Description: PXFieldState;
	@columnConfig({ width: 200 })
	AdditionalInformation: PXFieldState;
	@columnConfig({ width: 120 })
	State: PXFieldState;
	@columnConfig({ width: 150 })
	AllowOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ width: 200 })
	Value: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ width: 80 })
	Required: PXFieldState<PXFieldOptions.CommitChanges>;
	UsedForTaxCalculation: PXFieldState;
	UsedForGovernmentReporting: PXFieldState;
	FormBox: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false,
})
export class Employees extends PXView {
	AcctCD: PXFieldState;
}
