import {
	graphInfo,
	gridConfig,
	linkCommand,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXActionState,
	columnConfig
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.BillHistoryInq', primaryView: 'BillHistoryRecords' })
export class FS405000 extends PXScreen {
	FSBillHistory$ParentDocLink$Link: PXActionState;
	FSBillHistory$ChildDocLink$Link: PXActionState;
	FSBillHistory$RelatedDocument$Link: PXActionState;
	OpenPostBatch: PXActionState;
	BillHistoryRecords = createCollection(FSBillHistory);
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	mergeToolbarWith: "ScreenToolbar"
})
export class FSBillHistory extends PXView {
	@linkCommand("openPostBatch") BatchID: PXFieldState;
	RelatedDocumentType: PXFieldState;
	@linkCommand("FSBillHistory$RelatedDocument$Link") RelatedDocument: PXFieldState;
	ChildEntityType: PXFieldState;
	@linkCommand("FSBillHistory$ChildDocLink$Link") ChildDocLink: PXFieldState;
	ChildDocDate: PXFieldState;
	ChildDocStatus: PXFieldState;
	ChildAmount: PXFieldState;
	@columnConfig({ hideViewLink: true }) ServiceContractPeriodID: PXFieldState;
}
