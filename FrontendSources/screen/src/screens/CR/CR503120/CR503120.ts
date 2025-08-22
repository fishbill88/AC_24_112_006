import {
	PXView,
	PXFieldState,
	columnConfig,
	graphInfo,
	PXScreen,
	linkCommand,
	createCollection,
	gridConfig,
	PXFieldOptions,
	PXPageLoadBehavior,
	GridPagerMode,
	PXActionState,
	createSingle,
	GridPreset,
	GridFilterBarVisibility,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CR.UpdateOpportunityMassProcess",
	primaryView: "Filter",
	pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues,
})
export class CR503120 extends PXScreen {
	wizardNext: PXActionState;

	Filter = createSingle(CRWorkflowMassActionFilter);
	Items = createCollection(CROpportunity);
}

export class CRWorkflowMassActionFilter extends PXView {
	Operation: PXFieldState<PXFieldOptions.CommitChanges>;
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	showFilterBar: GridFilterBarVisibility.OnDemand,
	quickFilterFields: ["OpportunityID", "Subject"],
	allowUpdate: false,
	pagerMode: GridPagerMode.Numeric,
	suppressNoteFiles: true,
})
export class CROpportunity extends PXView {
	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;

	@columnConfig({ allowCheckAll: true, width: 35 })
	Selected: PXFieldState;
	@linkCommand("Items_ViewDetails")
	OpportunityID: PXFieldState;
	Subject: PXFieldState;
	Status: PXFieldState;
	Resolution: PXFieldState<PXFieldOptions.Hidden>;
	StageID: PXFieldState;
	CROpportunityProbability__Probability: PXFieldState<PXFieldOptions.Hidden>;
	CRActivityStatistics__LastIncomingActivityDate: PXFieldState<PXFieldOptions.Hidden>;
	CRActivityStatistics__LastOutgoingActivityDate: PXFieldState<PXFieldOptions.Hidden>;
	CRActivityStatistics__LastActivityDate: PXFieldState<PXFieldOptions.Hidden>;
	CloseDate: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
	CuryProductsAmount: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ClassID: PXFieldState;
	Source: PXFieldState<PXFieldOptions.Hidden>;
	BAccount__AcctCD: PXFieldState;
	BAccount__AcctName: PXFieldState;
	@linkCommand("Items_BAccountParent_ViewDetails")
	BAccountParent__AcctCD: PXFieldState<PXFieldOptions.Hidden>;
	BAccountParent__AcctName: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true })
	WorkgroupID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true })
	OwnerID: PXFieldState;
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedByID_Modifier_Username: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	SalesTerritoryID: PXFieldState<PXFieldOptions.Hidden>;
}
