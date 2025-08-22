import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	viewInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig
} from "client-controls";

@graphInfo({graphType: "PX.Objects.IN.INSiteBuildingMaint", primaryView: "Buildings" })
export class IN204010 extends PXScreen {
	AddressLookup: PXActionState;

   	@viewInfo({containerName: "Warehouse Building Summary"})
	Buildings = createSingle(INSiteBuilding);
   	@viewInfo({containerName: "Warehouses"})
	Sites = createCollection(INSite);
   	@viewInfo({containerName: "Address"})
	Address = createSingle(Address);
   	@viewInfo({containerName: "Specify New ID"})
	ChangeIDDialog = createSingle(ChangeIDParam);
}

export class INSiteBuilding extends PXView  {
	BuildingCD : PXFieldState;
	BranchID : PXFieldState<PXFieldOptions.CommitChanges>;
	Descr : PXFieldState;
}

@gridConfig({
	allowInsert: false,
	allowUpdate: false
})
export class INSite extends PXView  {
	SiteCD : PXFieldState;
	Descr : PXFieldState;
	Active : PXFieldState;
}

export class Address extends PXView  {
	AddressLine1 : PXFieldState;
	AddressLine2 : PXFieldState;
	City : PXFieldState;
	CountryID : PXFieldState<PXFieldOptions.CommitChanges>;
	State : PXFieldState;
	PostalCode : PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated : PXFieldState<PXFieldOptions.Disabled>;
}

export class ChangeIDParam extends PXView  {
	CD : PXFieldState;
}