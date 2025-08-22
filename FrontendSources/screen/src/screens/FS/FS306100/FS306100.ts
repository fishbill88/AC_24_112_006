import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	linkCommand,
	PXScreen,
	PXView,
	PXFieldState,
	PXActionState,
	PXPageLoadBehavior
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.ContractPostBatchMaint', primaryView: 'ContractBatchRecords', pageLoadBehavior: PXPageLoadBehavior.GoFirstRecord })
export class FS306100 extends PXScreen {
	Save: PXActionState;
	OpenDocument: PXActionState;
	OpenContract: PXActionState;
	ContractBatchRecords = createSingle(FSContractPostBatch);
	ContractPostDocRecords = createCollection(ContractPostBatchDetail);
}

export class FSContractPostBatch extends PXView {
	ContractPostBatchNbr: PXFieldState;
	UpToDate: PXFieldState;
	InvoiceDate: PXFieldState;
	FinPeriodID: PXFieldState;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false
})
export class ContractPostBatchDetail extends PXView {
	PostDocType: PXFieldState;
	@linkCommand("OpenDocument") PostRefNbr: PXFieldState;
	@linkCommand("OpenContract") ContractRefNbr: PXFieldState;
	CustomerContractNbr: PXFieldState;
	BillCustomerID: PXFieldState;
	AcctName: PXFieldState;
	BillLocationID: PXFieldState;
	StartDate: PXFieldState;
	NextBillingInvoiceDate: PXFieldState;
	BranchID: PXFieldState;
	BranchLocationID: PXFieldState;
	DocDesc: PXFieldState;
}
