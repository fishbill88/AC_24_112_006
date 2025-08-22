import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, GridPreset } from "client-controls";

@graphInfo({graphType: "PX.Objects.RQ.RQRequestProcess", primaryView: "Filter", pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues})
export class RQ504000 extends PXScreen {
   	@viewInfo({containerName: "Selection"})
	Filter = createSingle(RQRequestSelection);
   	@viewInfo({containerName: "Requests"})
	Records = createCollection(RQRequestLine);
}

// Views

export class RQRequestSelection extends PXView  {
	OwnerID : PXFieldState<PXFieldOptions.CommitChanges>;
	MyOwner : PXFieldState<PXFieldOptions.CommitChanges>;
	WorkGroupID : PXFieldState<PXFieldOptions.CommitChanges>;
	MyWorkGroup : PXFieldState<PXFieldOptions.CommitChanges>;
	MyEscalated : PXFieldState<PXFieldOptions.CommitChanges>;
	ReqClassID : PXFieldState<PXFieldOptions.CommitChanges>;
	SelectedPriority : PXFieldState<PXFieldOptions.CommitChanges>;
	AddExists : PXFieldState<PXFieldOptions.CommitChanges>;
	Description : PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeID : PXFieldState<PXFieldOptions.CommitChanges>;
	DepartmentID : PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID : PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	quickFilterFields: ["VendorID"]
})
export class RQRequestLine extends PXView  {
	@columnConfig({allowCheckAll: true}) Selected : PXFieldState;
	@columnConfig({allowNull: false})	Priority : PXFieldState;
	OrderNbr : PXFieldState;
	DepartmentID : PXFieldState;
	EmployeeID : PXFieldState;
	OrderDate : PXFieldState;
	ShipDestType : PXFieldState;
	LineNbr : PXFieldState;
	InventoryID : PXFieldState;
	SubItemID : PXFieldState;
	Description : PXFieldState;
	@columnConfig({hideViewLink: true})	UOM : PXFieldState;
	@columnConfig({allowNull: false})	OrderQty : PXFieldState;
	@columnConfig({allowNull: false})	OpenQty : PXFieldState;
	@columnConfig({allowNull: false})	CuryEstUnitCost : PXFieldState;
	@columnConfig({allowNull: false})	CuryEstExtCost : PXFieldState;
	RequestedDate : PXFieldState;
	PromisedDate : PXFieldState;
	@columnConfig({hideViewLink: true})	VendorID : PXFieldState;
	VendorName : PXFieldState;
	@columnConfig({hideViewLink: true})	VendorLocationID : PXFieldState;
	VendorRefNbr : PXFieldState;
	VendorDescription : PXFieldState;
	AlternateID : PXFieldState;
	IssueStatus : PXFieldState;
	@columnConfig({hideViewLink: true})	ExpenseAcctID : PXFieldState;
	@columnConfig({hideViewLink: true})	ExpenseSubID : PXFieldState;
}
