import {
	createCollection, createSingle,
	PXScreen, PXView, PXFieldState, PXActionState,
	graphInfo, viewInfo, gridConfig, linkCommand,
	PXFieldOptions, PXPageLoadBehavior
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.AR.ARSPCommissionDocEnq", primaryView: "Filter", 
	pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues,
	hideFilesIndicator: true, hideNotesIndicator: true
})
export class AR403000 extends PXScreen {
	viewDocument: PXActionState;
	viewOrigDocument: PXActionState;

   	@viewInfo({containerName: "Selection"})
	Filter = createSingle(SPDocFilter);
   	@viewInfo({containerName: "Documents"})
	SPDocs = createCollection(ARSPCommnDocResult);
}

export class SPDocFilter extends PXView  {
	SalesPersonID : PXFieldState<PXFieldOptions.CommitChanges>;
	CommnPeriod : PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID : PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	mergeToolbarWith: "ScreenToolbar"
})
export class ARSPCommnDocResult extends PXView  {
	BranchID : PXFieldState;
	DocType : PXFieldState;
	
	@linkCommand("viewDocument")
	RefNbr : PXFieldState;
	
	AdjdDocType : PXFieldState;
	
	@linkCommand("viewOrigDocument")
	AdjdRefNbr : PXFieldState;
	
	OrigDocAmt : PXFieldState;	
	CommnblAmt : PXFieldState;
	CommnPct : PXFieldState;
	CommnAmt : PXFieldState;
	BaseCuryID : PXFieldState;
	CustomerID : PXFieldState;
	CustomerID_BAccountR_acctName : PXFieldState;
	CustomerLocationID : PXFieldState;
	CustomerLocationID_Location_descr : PXFieldState;
}
