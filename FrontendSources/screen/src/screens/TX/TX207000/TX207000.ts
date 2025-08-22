import { createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, PXActionState } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.TX.TaxYearMaint', primaryView: 'TaxYearFilterSelectView', showUDFIndicator: true })
export class TX207000 extends PXScreen {

	ViewTaxPeriodDetails: PXActionState;

	TaxYearFilterSelectView = createSingle(TaxYearFilter);
	TaxPeriodExSelectView = createCollection(TaxPeriod);

}

export class TaxYearFilter extends PXView {

	OrganizationID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	Year: PXFieldState<PXFieldOptions.CommitChanges>;
	ShortTaxYear: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxPeriodType: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ syncPosition: true, allowInsert: false, allowDelete: false })
export class TaxPeriod extends PXView {

	TaxPeriodID: PXFieldState;
	StartDateUI: PXFieldState;
	EndDateUI: PXFieldState;
	Status: PXFieldState;
	@linkCommand('ViewTaxPeriodDetails')
	NetTaxAmt: PXFieldState;

	// Actions

	AddPeriod: PXActionState;
	DeletePeriod: PXActionState;

}
