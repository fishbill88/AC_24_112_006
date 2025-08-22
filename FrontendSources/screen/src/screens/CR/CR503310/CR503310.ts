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
	graphType: "PX.Objects.CR.AssignBAccountMassProcess",
	primaryView: "Items",
})
export class CR503310 extends PXScreen {
	Items = createCollection(BAccount);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	showFilterBar: GridFilterBarVisibility.OnDemand,
	quickFilterFields: ["AcctName"],
	suppressNoteFiles: true,
})
export class BAccount extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	Type: PXFieldState;
	AcctCD: PXFieldState;
	AcctName: PXFieldState;
	Status: PXFieldState;
	@columnConfig({ hideViewLink: true }) ClassID: PXFieldState;
	BAccountParent__AcctCD: PXFieldState<PXFieldOptions.Hidden>;
	BAccountParent__AcctName: PXFieldState<PXFieldOptions.Hidden>;
	State__name: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true })
	Address__CountryID: PXFieldState<PXFieldOptions.Hidden>;
	Address__City: PXFieldState<PXFieldOptions.Hidden>;
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
	@columnConfig({ hideViewLink: true }) OwnerID: PXFieldState;
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedByID_Modifier_Username: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	SalesTerritoryID: PXFieldState<PXFieldOptions.Hidden>;
}
