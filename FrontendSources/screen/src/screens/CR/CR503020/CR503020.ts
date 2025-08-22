import {
	PXView,
	PXFieldState,
	columnConfig,
	graphInfo,
	PXScreen,
	createSingle,
	createCollection,
	gridConfig,
	PXFieldOptions,
	PXPageLoadBehavior,
	GridPagerMode,
	PXActionState,
	linkCommand,
	GridPreset,
	GridFilterBarVisibility,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CR.UpdateLeadMassProcess",
	primaryView: "Filter",
	pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues,
})
export class CR503020 extends PXScreen {
	wizardNext: PXActionState;

	Filter = createSingle(CRWorkflowMassActionFilter);
	Items = createCollection(CRLead);
}

export class CRWorkflowMassActionFilter extends PXView {
	Operation: PXFieldState<PXFieldOptions.CommitChanges>;
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	showFilterBar: GridFilterBarVisibility.OnDemand,
	quickFilterFields: ["DisplayName", "FullName"],
	allowUpdate: false,
	pagerMode: GridPagerMode.Numeric,
	suppressNoteFiles: true,
})
export class CRLead extends PXView {
	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;

	@columnConfig({ allowCheckAll: true, width: 35 })
	Selected: PXFieldState;
	@linkCommand("Items_ViewDetails")
	DisplayName: PXFieldState;
	Title: PXFieldState<PXFieldOptions.Hidden>;
	FirstName: PXFieldState<PXFieldOptions.Hidden>;
	LastName: PXFieldState<PXFieldOptions.Hidden>;
	Salutation: PXFieldState;
	DuplicateStatus: PXFieldState<PXFieldOptions.Hidden>;
	BAccount__AcctCD: PXFieldState;
	FullName: PXFieldState;
	BAccountParent__AcctCD: PXFieldState<PXFieldOptions.Hidden>;
	BAccountParent__AcctName: PXFieldState<PXFieldOptions.Hidden>;
	Status: PXFieldState;
	Resolution: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true })
	ClassID: PXFieldState;
	Source: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CampaignID: PXFieldState;
	State__name: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true })
	Address__CountryID: PXFieldState<PXFieldOptions.Hidden>;
	Address__City: PXFieldState<PXFieldOptions.Hidden>;
	Address__PostalCode: PXFieldState<PXFieldOptions.Hidden>;
	Address__AddressLine1: PXFieldState<PXFieldOptions.Hidden>;
	Address__AddressLine2: PXFieldState<PXFieldOptions.Hidden>;
	EMail: PXFieldState;
	Phone1: PXFieldState;
	Phone2: PXFieldState<PXFieldOptions.Hidden>;
	Phone3: PXFieldState<PXFieldOptions.Hidden>;
	Fax: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true })
	WorkgroupID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true })
	OwnerID: PXFieldState;
	Method: PXFieldState<PXFieldOptions.Hidden>;
	CRActivityStatistics__LastIncomingActivityDate: PXFieldState<PXFieldOptions.Hidden>;
	CRActivityStatistics__LastOutgoingActivityDate: PXFieldState<PXFieldOptions.Hidden>;
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedByID_Modifier_Username: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	SalesTerritoryID: PXFieldState<PXFieldOptions.Hidden>;
}
