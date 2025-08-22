import {
	createCollection,
	PXScreen,
	PXView,
	graphInfo,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	columnConfig,
	linkCommand,
	GridPreset,
	GridFilterBarVisibility,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CR.AssignContactMassProcess",
	primaryView: "Items",
})
export class CR503011 extends PXScreen {
	Items = createCollection(Contact);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	showFilterBar: GridFilterBarVisibility.OnDemand,
	suppressNoteFiles: true,
})
export class Contact extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	@linkCommand("Items_ViewDetails") DisplayName: PXFieldState;
	Title: PXFieldState<PXFieldOptions.Hidden>;
	FirstName: PXFieldState<PXFieldOptions.Hidden>;
	LastName: PXFieldState<PXFieldOptions.Hidden>;
	Salutation: PXFieldState;
	DuplicateStatus: PXFieldState<PXFieldOptions.Hidden>;
	BAccount__AcctCD: PXFieldState;
	FullName: PXFieldState;
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
