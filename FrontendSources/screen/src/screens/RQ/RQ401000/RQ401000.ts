import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	PXPageLoadBehavior,
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	columnConfig
} from "client-controls";

@graphInfo({graphType: "PX.Objects.RQ.RQRequestEnq", primaryView: "Filter", pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues})
export class RQ401000 extends PXScreen {

   	@viewInfo({containerName: "Selection"})
	Filter = createSingle(RQRequestSelection);
   	@viewInfo({containerName: "Requests"})
	Records = createCollection(RQRequestLine);
}

export class RQRequestSelection extends PXView  {
	ReqClassID : PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeID : PXFieldState<PXFieldOptions.CommitChanges>;
	DepartmentID : PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID : PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemCD : PXFieldState<PXFieldOptions.CommitChanges>;
	Description : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
	mergeToolbarWith: "ScreenToolbar"
})
export class RQRequestLine extends PXView  {
	RQRequest__OrderNbr : PXFieldState;
	@columnConfig({hideViewLink: true})
	RQRequest__ReqClassID : PXFieldState;
	RQRequest__OrderDate : PXFieldState;
	InventoryID : PXFieldState;
	@columnConfig({hideViewLink: true})
	SubItemID : PXFieldState;
	Description : PXFieldState;
	@columnConfig({hideViewLink: true})
	UOM : PXFieldState;
	@columnConfig({hideViewLink: true})
	RQRequest__CuryID : PXFieldState;
	@columnConfig({hideViewLink: true})
	CuryEstUnitCost : PXFieldState;
	OrderQty : PXFieldState;
	CuryEstExtCost : PXFieldState;
	OpenQty : PXFieldState;
	RQRequest__Status : PXFieldState;
	IssueStatus : PXFieldState;
	@columnConfig({hideViewLink: true})
	RQRequest__DepartmentID : PXFieldState;
	@columnConfig({hideViewLink: true})
	RQRequest__EmployeeID : PXFieldState;
	@columnConfig({hideViewLink: true})
	ExpenseAcctID : PXFieldState;
	@columnConfig({hideViewLink: true})
	ExpenseSubID : PXFieldState;
	@columnConfig({hideViewLink: true})
	VendorID : PXFieldState;
	@columnConfig({hideViewLink: true})
	VendorLocationID : PXFieldState;
	VendorName : PXFieldState;
	VendorRefNbr : PXFieldState;
	VendorDescription : PXFieldState;
	@columnConfig({hideViewLink: true})
	AlternateID : PXFieldState;
	RequestedDate : PXFieldState;
	PromisedDate : PXFieldState;
}