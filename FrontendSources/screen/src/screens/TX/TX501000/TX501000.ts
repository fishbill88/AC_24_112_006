import {
	createCollection, createSingle, PXScreen, graphInfo, PXPageLoadBehavior, PXView, PXFieldState, PXFieldOptions, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.TX.ReportTax', primaryView: 'Period_Header', pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues })
export class TX501000 extends PXScreen {

	Period_Header = createSingle(TaxPeriodFilter);
	Period_Details = createCollection(TaxReportLine);

}

export class TaxPeriodFilter extends PXView {

	OrganizationID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState;
	EndDate: PXFieldState;

}

@gridConfig({
	mergeToolbarWith: 'ScreenToolbar',
})
export class TaxReportLine extends PXView {

	LineNbr: PXFieldState;
	SortOrder: PXFieldState;
	Descr: PXFieldState;
	TaxHistory__ReportUnfiledAmt: PXFieldState;
	TaxReportLine__TaxReportRevisionID: PXFieldState;

}
