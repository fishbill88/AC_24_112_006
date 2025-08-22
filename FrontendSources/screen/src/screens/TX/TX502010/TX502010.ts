import { createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, linkCommand, PXActionState } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.TX.ReportTaxDetail', primaryView: 'History_Header', })
export class TX502010 extends PXScreen {

	ViewDocument: PXActionState;

	History_Header = createSingle(TaxHistoryMaster);
	History_Detail = createCollection(TaxTran);

}

export class TaxHistoryMaster extends PXView {

	OrganizationID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDateUI: PXFieldState;
	EndDateInclusive: PXFieldState;
	LineNbr: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ syncPosition: true })
export class TaxTran extends PXView {

	ViewBatch: PXActionState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;

	Module: PXFieldState;
	TranTypeInvoiceDiscriminated: PXFieldState;

	@linkCommand('ViewDocument')
	RefNbr: PXFieldState;

	TranDate: PXFieldState;

	@columnConfig({ hideViewLink: true })
	TaxID: PXFieldState;

	Description: PXFieldState;
	TaxRate: PXFieldState;
	NonDeductibleTaxRate: PXFieldState;
	ReportTaxableAmt: PXFieldState;
	ReportExemptedAmt: PXFieldState;
	ReportTaxAmt: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BAccount__AcctCD: PXFieldState;

	BAccount__AcctName: PXFieldState;

	@columnConfig({ hideViewLink: true })
	TaxZoneID: PXFieldState;

}
