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

@graphInfo({graphType: "PX.Objects.RQ.RQRequisitionProcess", primaryView: "Filter", pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues})
export class RQ505000 extends PXScreen {

   	@viewInfo({containerName: "Selection"})
	Filter = createSingle(RQRequisitionSelection);
   	@viewInfo({containerName: "Requisitions"})
	Records = createCollection(RQRequisition);
}

export class RQRequisitionSelection extends PXView  {
	Action : PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID : PXFieldState<PXFieldOptions.CommitChanges>;
	MyOwner : PXFieldState<PXFieldOptions.CommitChanges>;
	WorkGroupID : PXFieldState<PXFieldOptions.CommitChanges>;
	MyWorkGroup : PXFieldState<PXFieldOptions.CommitChanges>;
	MyEscalated : PXFieldState<PXFieldOptions.CommitChanges>;
	Description : PXFieldState<PXFieldOptions.CommitChanges>;
	SelectedPriority : PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeID : PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	mergeToolbarWith: "ScreenToolbar"
})
export class RQRequisition extends PXView  {
	@columnConfig({allowCheckAll: true})
	Selected : PXFieldState;
	Priority : PXFieldState;
	ReqNbr : PXFieldState;
	OrderDate : PXFieldState;
	Status : PXFieldState;
	Description : PXFieldState;
	@columnConfig({hideViewLink: true})
	EmployeeID : PXFieldState;
	@columnConfig({hideViewLink: true})
	VendorID : PXFieldState;
	@columnConfig({hideViewLink: true})
	VendorLocationID : PXFieldState;
	VendorRefNbr : PXFieldState;
	@columnConfig({hideViewLink: true})
	CuryID : PXFieldState;
	CuryEstExtCostTotal : PXFieldState;
}