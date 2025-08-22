import {
	createCollection, createSingle,
	PXScreen, PXView, PXFieldState, PXActionState,
	graphInfo, columnConfig, PXFieldOptions
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.AR.ARDunningLetterByDocumentEnq", primaryView: "Filter",
	hideFilesIndicator: true, hideNotesIndicator: true
})
export class AR408500 extends PXScreen {
	viewDocument: PXActionState;

	Filter = createSingle(DLByDocumentFilter);
	EnqResults = createCollection(ARDunningLetterDetail);
}

export class DLByDocumentFilter extends PXView  {
	BAccountID : PXFieldState<PXFieldOptions.CommitChanges>;
	BeginDate : PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate : PXFieldState<PXFieldOptions.CommitChanges>;
	LevelFrom : PXFieldState<PXFieldOptions.CommitChanges>;
	LevelTo : PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ARDunningLetterDetail extends PXView  {
	ViewLetter : PXActionState;

	@columnConfig({ hideViewLink: true })
	ARInvoice__CustomerID: PXFieldState;

	ARInvoice__DocType : PXFieldState;
	ARInvoice__RefNbr : PXFieldState;
	ARInvoice__DocBal : PXFieldState;
	ARInvoice__DueDate : PXFieldState;
	DunningLetterLevel : PXFieldState;
	ARDunningLetter__Status : PXFieldState;
	ARDunningLetter__DunningLetterDate : PXFieldState;
}
