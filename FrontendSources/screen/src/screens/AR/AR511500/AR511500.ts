import {
	createCollection, createSingle,
	PXScreen, PXView, PXFieldState, PXActionState,
	graphInfo, viewInfo, gridConfig, columnConfig, linkCommand,
	PXFieldOptions
} from 'client-controls';

@graphInfo({
	graphType: 'PX.Objects.AR.ARPaymentsAutoProcessing', primaryView: 'Filter',
	hideFilesIndicator: true, hideNotesIndicator: true
})
export class AR511500 extends PXScreen {
	EditDetail: PXActionState;

	@viewInfo({ containerName: 'Selection' })
	Filter = createSingle(PaymentFilter);

	@viewInfo({ containerName: 'Payment Details' })
	ARDocumentList = createCollection(ARPaymentInfo);
}

export class PaymentFilter extends PXView {
	PayDate: PXFieldState<PXFieldOptions.CommitChanges>;
	StatementCycleId: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	PaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProcessingCenterID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	initNewRow: true, syncPosition: true, mergeToolbarWith: 'ScreenToolbar',
	quickFilterFields: ['RefNbr', 'CustomerID', 'CustomerID_BAccountR_acctName']})
export class ARPaymentInfo extends PXView {

	@columnConfig({ allowNull: false, allowCheckAll: true })
	Selected: PXFieldState;

	BranchID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	DocType: PXFieldState;

	@linkCommand('EditDetail')
	@columnConfig({ allowUpdate: false })
	RefNbr: PXFieldState;

	@columnConfig({ allowUpdate: false })
	DocDate: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	FinPeriodID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CustomerID: PXFieldState;

	CustomerID_BAccountR_acctName: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	CustomerLocationID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	DocDesc: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Status: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	CuryID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CuryOrigDocAmt: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CuryDocBal: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CuryDiscBal: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CCPaymentStateDescr: PXFieldState;

	@columnConfig({ allowUpdate: false })
	PMInstanceDescr: PXFieldState;

	IsCCExpired: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	ProcessingCenterID: PXFieldState;

	ProcessingCenterID_CCProcessingCenter_Name: PXFieldState;
	CCTranDescr: PXFieldState;
}
