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
	graphType: "PX.Objects.CR.UpdateBAccountMassProcess",
	primaryView: "Items",
	pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues,
})
export class CR503320 extends PXScreen {
	Items = createCollection(BAccount);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	showFilterBar: GridFilterBarVisibility.OnDemand,
	allowUpdate: false,
	pagerMode: GridPagerMode.Numeric,
	suppressNoteFiles: true,
})
export class BAccount extends PXView {
	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;

	@columnConfig({ allowCheckAll: true, width: 35 })
	Selected: PXFieldState;
	Type: PXFieldState;
	AcctCD: PXFieldState;
	AcctName: PXFieldState;
	Status: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ClassID: PXFieldState;
	@linkCommand("Items_BAccountParent_ViewDetails")
	BAccountParent__AcctCD: PXFieldState<PXFieldOptions.Hidden>;
	BAccountParent__AcctName: PXFieldState<PXFieldOptions.Hidden>;
	State__name: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true })
	Address__CountryID: PXFieldState<PXFieldOptions.Hidden>;
	Address__City: PXFieldState<PXFieldOptions.Hidden>;
	Address__PostalCode: PXFieldState<PXFieldOptions.Hidden>;
	Address__AddressLine1: PXFieldState<PXFieldOptions.Hidden>;
	Address__AddressLine2: PXFieldState<PXFieldOptions.Hidden>;
	Contact__EMail: PXFieldState;
	Contact__Phone1: PXFieldState;
	Contact__Phone2: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Phone3: PXFieldState<PXFieldOptions.Hidden>;
	Contact__Fax: PXFieldState<PXFieldOptions.Hidden>;
	Contact__WebSite: PXFieldState<PXFieldOptions.Hidden>;
	Contact__DuplicateStatus: PXFieldState<PXFieldOptions.Hidden>;
	TaxZoneID: PXFieldState<PXFieldOptions.Hidden>;
	Location__CCarrierID: PXFieldState<PXFieldOptions.Hidden>;
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
