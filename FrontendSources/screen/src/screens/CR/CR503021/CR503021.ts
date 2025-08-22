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
	linkCommand,
	GridPreset,
	GridFilterBarVisibility,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CR.UpdateContactMassProcess",
	primaryView: "Items",
	pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues,
})
export class CR503021 extends PXScreen {
	Items = createCollection(Contact);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	showFilterBar: GridFilterBarVisibility.OnDemand,
	allowUpdate: false,
	pagerMode: GridPagerMode.Numeric,
	suppressNoteFiles: true,
})
export class Contact extends PXView {
	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;

	@columnConfig({ allowCheckAll: true, width: 35 }) Selected: PXFieldState;
	@linkCommand("Items_ViewDetails") DisplayName: PXFieldState;
	Title: PXFieldState<PXFieldOptions.Hidden>;
	FirstName: PXFieldState<PXFieldOptions.Hidden>;
	LastName: PXFieldState<PXFieldOptions.Hidden>;
	Salutation: PXFieldState;
	DuplicateStatus: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("Items_BAccount_ViewDetails")
	BAccount__AcctCD: PXFieldState;
	FullName: PXFieldState;
	@linkCommand("Items_BAccountParent_ViewDetails")
	BAccountParent__AcctCD: PXFieldState<PXFieldOptions.Hidden>;
	BAccountParent__AcctName: PXFieldState<PXFieldOptions.Hidden>;
	IsActive: PXFieldState<PXFieldOptions.Hidden>;
	Status: PXFieldState<PXFieldOptions.Hidden>;
	Resolution: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true }) ClassID: PXFieldState;
	Source: PXFieldState<PXFieldOptions.Hidden>;
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
