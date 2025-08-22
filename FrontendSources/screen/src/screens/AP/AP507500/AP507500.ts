import { PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, createCollection, createSingle, PXScreen, graphInfo, PXActionState } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AP.MISC1099EFileProcessing', primaryView: 'Filter' })
export class AP507500 extends PXScreen {

	Filter = createSingle(MISC1099EFileFilter);
	Records = createCollection(MISC1099EFileProcessingInfo);

}

export class MISC1099EFileFilter extends PXView {
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	FinYear: PXFieldState<PXFieldOptions.CommitChanges>;
	FileFormat: PXFieldState<PXFieldOptions.CommitChanges>;
	Include: PXFieldState<PXFieldOptions.CommitChanges>;
	Box7: PXFieldState<PXFieldOptions.CommitChanges>;
	IsPriorYear: PXFieldState;
	IsCorrectionReturn: PXFieldState;
	IsLastFiling: PXFieldState;
	ReportingDirectSalesOnly: PXFieldState<PXFieldOptions.CommitChanges>;
	IsTestMode: PXFieldState;
}

@gridConfig({ syncPosition: true, adjustPageSize: true })
export class MISC1099EFileProcessingInfo extends PXView {

	view1099Summary: PXActionState;
	
	@columnConfig({ allowUpdate: false, allowCheckAll: true })
	Selected: PXFieldState;
	@columnConfig({ allowUpdate: false })
	VAcctCD: PXFieldState;
	@columnConfig({ allowUpdate: false })
	VAcctName: PXFieldState;
	@columnConfig({ allowUpdate: false })
	HistAmt: PXFieldState;
	@columnConfig({ allowUpdate: false })
	LTaxRegistrationID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	PayerBAccountID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	State: PXFieldState;
}