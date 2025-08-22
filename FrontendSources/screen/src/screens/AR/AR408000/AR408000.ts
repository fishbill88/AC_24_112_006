import {
	createCollection, createSingle,
	PXScreen, PXView, PXFieldState, PXActionState,
	graphInfo, gridConfig, columnConfig,
	GridColumnType, PXFieldOptions
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.AR.ARDunningLetterByCustomerEnq", primaryView: "Filter",
	hideFilesIndicator: true, hideNotesIndicator: true
})
export class AR408000 extends PXScreen {
	Filter = createSingle(DLByCustomerFilter);
	EnqResults = createCollection(ARDunningLetter);
}

export class DLByCustomerFilter extends PXView  {
	BAccountID : PXFieldState<PXFieldOptions.CommitChanges>;
	BeginDate : PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true
})
export class ARDunningLetter extends PXView  {
	ViewDocument : PXActionState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BAccountID: PXFieldState;

	Customer__AcctName : PXFieldState;
	DunningLetterLevel : PXFieldState;
	Status : PXFieldState;
	ARDunningLetterDetail__OverdueBal : PXFieldState;
	DunningLetterDate : PXFieldState;
	DetailsCount : PXFieldState;
}
