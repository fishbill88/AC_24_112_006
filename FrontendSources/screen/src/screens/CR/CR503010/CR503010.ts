import {
	PXScreen,
	graphInfo,
	PXView,
	createCollection,
	PXFieldState,
	gridConfig,
	PXActionState,
	GridPagerMode,
	GridFilterBarVisibility,
	PXFieldOptions,
	columnConfig,
	linkCommand,
	GridPreset,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CR.AssignLeadMassProcess",
	primaryView: "Items",
})
export class CR503010 extends PXScreen {
	Items = createCollection(CRLead);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	showFilterBar: GridFilterBarVisibility.OnDemand,
	quickFilterFields: ["DisplayName", "FullName"],
	suppressNoteFiles: true,
})
export class CRLead extends PXView {
	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	@linkCommand("Items_ViewDetails") MemberName: PXFieldState;
	Title: PXFieldState<PXFieldOptions.Hidden>;
	FirstName: PXFieldState<PXFieldOptions.Hidden>;
	LastName: PXFieldState<PXFieldOptions.Hidden>;
	Salutation: PXFieldState;
	DuplicateStatus: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("Items_BAccount_ViewDetails") BAccount__AcctCD: PXFieldState;
	FullName: PXFieldState;
	@linkCommand("Items_BAccountParent_ViewDetails")
	BAccountParent__AcctCD: PXFieldState<PXFieldOptions.Hidden>;
	BAccountParent__AcctName: PXFieldState<PXFieldOptions.Hidden>;
	Status: PXFieldState;
	Resolution: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true }) ClassID: PXFieldState;
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
	@columnConfig({ hideViewLink: true }) OwnerID: PXFieldState;
	CRActivityStatistics__LastIncomingActivityDate: PXFieldState<PXFieldOptions.Hidden>;
	CRActivityStatistics__LastOutgoingActivityDate: PXFieldState<PXFieldOptions.Hidden>;
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedByID_Modifier_Username: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	SalesTerritoryID: PXFieldState<PXFieldOptions.Hidden>;
}
