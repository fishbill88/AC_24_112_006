import {
	createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, PXActionState} from "client-controls";

@graphInfo({ graphType: "PX.Objects.AP.InvoiceRecognition.RecognizedRecordProcess", primaryView: "Filter", pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues })
export class AP501100 extends PXScreen {

	viewDocument: PXActionState;

	@viewInfo({ containerName: "Filter" })
	Filter = createSingle(RecognizedRecordFilter);

	@viewInfo({ containerName: "Outdated Recognition Results" })
	Records = createCollection(RecognizedRecord);

}

export class RecognizedRecordFilter extends PXView {

	CreatedBefore: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowUnprocessedRecords: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ mergeToolbarWith: "ScreenToolbar" })
export class RecognizedRecord extends PXView {

	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;

	EntityType: PXFieldState;
	Status: PXFieldState;

	@linkCommand("viewDocument")
	DocumentLink: PXFieldState;

	CreatedDateTime: PXFieldState;
	MailFrom: PXFieldState;
	Subject: PXFieldState;
	Owner: PXFieldState;

}
