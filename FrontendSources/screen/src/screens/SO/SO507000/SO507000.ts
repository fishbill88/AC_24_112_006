import {
	PXScreen,
	createSingle,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	columnConfig,
	PXFieldOptions,
	viewInfo,
	PXActionState,
	linkCommand,
	gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.SO.SOPaymentProcess', primaryView: 'Filter' })
export class SO507000 extends PXScreen {

	ViewRelatedDocument: PXActionState;
	ViewDocument: PXActionState;

	@viewInfo({ containerName: "CC Processing Filter" })
	Filter = createSingle(Filter);
	@viewInfo({ containerName: "Payments" })
	Payments = createCollection(Payments);
}

export class Filter extends PXView {
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	adjustPageSize: true,
	syncPosition: true,
	mergeToolbarWith: 'ScreenToolbar'
})
export class Payments extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	@columnConfig({ hideViewLink: true })
	DocType: PXFieldState;
	@linkCommand('ViewDocument')
	RefNbr: PXFieldState;
	DocDate: PXFieldState;
	@columnConfig({ hideViewLink: true })
	FinPeriodID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CustomerID: PXFieldState;
	CustomerID_BAccountR_acctName: PXFieldState;
	FundHoldExpDate: PXFieldState;
	Status: PXFieldState;
	RelatedTranProcessingStatus: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
	CuryOrigDocAmt: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ProcessingCenterID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	PaymentMethodID: PXFieldState;
	RelatedDocument: PXFieldState;
	@linkCommand('ViewRelatedDocument')
	RelatedDocumentNumber: PXFieldState;
	RelatedDocumentStatus: PXFieldState;
	CuryRelatedDocumentAppliedAmount: PXFieldState;
	@columnConfig({ hideViewLink: true })
	relatedDocumentCreditTerms: PXFieldState;
	ErrorDescription: PXFieldState;
}