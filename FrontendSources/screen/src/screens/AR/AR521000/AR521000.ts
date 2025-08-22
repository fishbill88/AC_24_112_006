import {
	createCollection, createSingle,
	PXScreen, PXActionState, PXView, PXFieldState,
	graphInfo, viewInfo,
	gridConfig, columnConfig,
	PXFieldOptions, GridColumnType
} from "client-controls";


@graphInfo({
	graphType: "PX.Objects.AR.ARDunningLetterProcess", primaryView: "Filter",
	hideFilesIndicator: true, hideNotesIndicator: true
})
export class AR521000 extends PXScreen {
   	@viewInfo({containerName: "Selection"})
    Filter = createSingle(ARDunningLetterRecordsParameters);

   	@viewInfo({containerName: "Customers"})
	DunningLetterList = createCollection(ARDunningLetterList);
}

export class ARDunningLetterRecordsParameters extends PXView {
	OrganizationID: PXFieldState<PXFieldOptions.CommitChanges>;
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	DocDate: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeNonOverdueDunning: PXFieldState<PXFieldOptions.CommitChanges>;
	AddOpenPaymentsAndCreditMemos: PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	IncludeType: PXFieldState<PXFieldOptions.CommitChanges>;
	LevelFrom: PXFieldState<PXFieldOptions.CommitChanges>;
	LevelTo: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	quickFilterFields: ["CustomerClassID", "BAccountID", "BAccountID_BAccountR_acctName"]
})
export class ARDunningLetterList extends PXView {
	@columnConfig({ allowSort: false, allowNull: false, type: GridColumnType.CheckBox })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;

	CustomerClassID: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	BAccountID: PXFieldState;

	BAccountID_BAccountR_acctName: PXFieldState;
	DueDate: PXFieldState;

	@columnConfig({ allowNull: false, hideViewLink: true })
	BranchID: PXFieldState;

	@columnConfig({ allowNull: false })
	DocBal: PXFieldState;

	@columnConfig({ allowNull: false })
	NumberOfOverdueDocuments: PXFieldState;

	@columnConfig({ allowNull: false })
	OrigDocAmt: PXFieldState;

	@columnConfig({ allowNull: false })
	NumberOfDocuments: PXFieldState;

	@columnConfig({ allowNull: false })
	DunningLetterLevel: PXFieldState;

	@columnConfig({ allowNull: false, hideViewLink: true })
	CuryID: PXFieldState;

	LastDunningLetterDate: PXFieldState;
}
