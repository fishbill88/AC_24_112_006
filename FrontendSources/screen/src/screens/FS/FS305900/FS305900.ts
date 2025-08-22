import {
	graphInfo,
	gridConfig,
	createCollection,
	createSingle,
	linkCommand,
	PXScreen,
	PXView,
	PXFieldState,
	PXActionState,
	PXPageLoadBehavior
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.InventoryPostBatchMaint', primaryView: 'BatchRecords', pageLoadBehavior: PXPageLoadBehavior.GoFirstRecord })
export class FS305900 extends PXScreen {
	OpenDocument: PXActionState;
	BatchRecords = createSingle(FSPostBatch);
	BatchDetailsInfo = createCollection(InventoryPostingBatchDetail)
}

export class FSPostBatch extends PXView {
	BatchNbr: PXFieldState;
	CutOffDate: PXFieldState;
	InvoiceDate: PXFieldState;
	FinPeriodID: PXFieldState;
	QtyDoc: PXFieldState;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false
})
export class InventoryPostingBatchDetail extends PXView {
	Mem_DocType: PXFieldState;
	@linkCommand("OpenDocument") Mem_DocNbr: PXFieldState;
	SODetID: PXFieldState;
	InventoryID: PXFieldState;
	SrvOrdType: PXFieldState;
	AppointmentID: PXFieldState;
	BillCustomerID: PXFieldState;
	AcctName: PXFieldState;
	SOID: PXFieldState;
}
