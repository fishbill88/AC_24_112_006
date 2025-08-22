import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, PXActionState } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.TX.ReportTaxReview', primaryView: 'Period_Header', pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues })
export class TX502000 extends PXScreen {

	ViewDocument: PXActionState;
	CheckDocument: PXActionState;

	Period_Header = createSingle(TaxPeriodFilter);
	Period_Details = createCollection(TaxReportLine);
	APDocuments = createCollection(APInvoice);

}

export class TaxPeriodFilter extends PXView {

	OrganizationID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisionId: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowDifference: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState;
	EndDate: PXFieldState;

}

export class TaxReportLine extends PXView {

	LineNbr: PXFieldState;
	SortOrder: PXFieldState;
	Descr: PXFieldState;

	@linkCommand('ViewDocument')
	TaxHistory__ReportFiledAmt: PXFieldState;

	TaxReportLine__TaxReportRevisionID: PXFieldState;

}

@gridConfig({ syncPosition: true })
export class APInvoice extends PXView {

	BranchID: PXFieldState;
	DocType: PXFieldState;

	@linkCommand('CheckDocument')
	RefNbr: PXFieldState;

	DocDate: PXFieldState;
	FinPeriodID: PXFieldState;
	VendorID: PXFieldState;
	CuryID: PXFieldState;
	CuryOrigDocAmt: PXFieldState;
	CuryDocBal: PXFieldState;
	Status: PXFieldState;

}
