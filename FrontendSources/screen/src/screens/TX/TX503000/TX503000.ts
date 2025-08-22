import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, PXActionState, columnConfig } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.TX.ProcessOutputSVAT', primaryView: 'Filter', })
export class TX503000 extends PXScreen {

	ViewBatch: PXActionState;
	ViewDocument: PXActionState;
	Filter = createSingle(SVATTaxFilter);

	@viewInfo({ containerName: 'Transactions' })
	SVATDocuments = createCollection(SVATConversionHist);

}

export class SVATTaxFilter extends PXView {

	Date: PXFieldState<PXFieldOptions.CommitChanges>;
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxAgencyID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxID: PXFieldState<PXFieldOptions.CommitChanges>;
	ReversalMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	TotalTaxAmount: PXFieldState<PXFieldOptions.Disabled>;

}

@gridConfig({ syncPosition: true, mergeToolbarWith: 'ScreenToolbar' })
export class SVATConversionHist extends PXView {

	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;

	Module: PXFieldState;
	DisplayDocType: PXFieldState;

	@linkCommand('ViewDocument')
	AdjdRefNbr: PXFieldState;

	AdjdDocDate: PXFieldState;
	DisplayStatus: PXFieldState;
	TaxID: PXFieldState;
	TaxRate: PXFieldState;
	TaxableAmt: PXFieldState;
	TaxAmt: PXFieldState;
	TaxInvoiceNbr: PXFieldState;
	TaxInvoiceDate: PXFieldState;
	DisplayCounterPartyID: PXFieldState;
	DisplayDescription: PXFieldState;
	DisplayDocRef: PXFieldState;

	@linkCommand('ViewBatch')
	AdjBatchNbr: PXFieldState;

}
