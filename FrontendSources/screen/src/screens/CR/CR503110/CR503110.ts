import {
	createCollection,
	PXScreen,
	PXView,
	graphInfo,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	columnConfig,
	GridPreset,
	GridFilterBarVisibility,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CR.AssignOpportunityMassProcess",
	primaryView: "Items",
})
export class CR503110 extends PXScreen {
	Items = createCollection(CROpportunity);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	showFilterBar: GridFilterBarVisibility.OnDemand,
	quickFilterFields: ["OpportunityID", "Subject"],
	suppressNoteFiles: true,
})
export class CROpportunity extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
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
	@columnConfig({ hideViewLink: true }) CuryID: PXFieldState;
	CuryProductsAmount: PXFieldState;
	@columnConfig({ hideViewLink: true }) ClassID: PXFieldState;
	Source: PXFieldState<PXFieldOptions.Hidden>;
	BAccount__AcctCD: PXFieldState;
	BAccount__AcctName: PXFieldState;
	BAccountParent__AcctCD: PXFieldState<PXFieldOptions.Hidden>;
	BAccountParent__AcctName: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true })
	WorkgroupID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true }) OwnerID: PXFieldState;
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedByID_Modifier_Username: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	SalesTerritoryID: PXFieldState<PXFieldOptions.Hidden>;
}
