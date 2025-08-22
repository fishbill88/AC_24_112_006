import {
	RQ302000
} from '../RQ302000';

import {
	PXView,
	createCollection,
	PXFieldState,
	PXFieldOptions,
	createSingle,
	gridConfig,
	columnConfig,
	viewInfo,
	GridPreset
} from 'client-controls';

export interface RQ302000_SelectRequestedItems extends RQ302000 { }
export class RQ302000_SelectRequestedItems {
	@viewInfo({containerName: "Select Requested Items"})
	RequestFilter = createSingle(RQRequestSelection);
   	@viewInfo({containerName: "Select Requested Items"})
	SourceRequests = createCollection(RQRequestLineSelect);
}

export class RQRequestSelection extends PXView  {
	OwnerID : PXFieldState<PXFieldOptions.CommitChanges>;
	MyOwner : PXFieldState<PXFieldOptions.CommitChanges>;
	WorkGroupID : PXFieldState<PXFieldOptions.CommitChanges>;
	MyWorkGroup : PXFieldState<PXFieldOptions.CommitChanges>;
	MyEscalated : PXFieldState<PXFieldOptions.CommitChanges>;
	AddExists : PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID : PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID : PXFieldState<PXFieldOptions.CommitChanges>;
	ReqClassID : PXFieldState<PXFieldOptions.CommitChanges>;
	SelectedPriority : PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeID : PXFieldState<PXFieldOptions.CommitChanges>;
	DepartmentID : PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID : PXFieldState<PXFieldOptions.CommitChanges>;
	Description : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry
})
export class RQRequestLineSelect extends PXView  {
	@columnConfig({allowCheckAll: true}) Selected : PXFieldState;
	@columnConfig({allowNull: false}) RQRequest__Priority : PXFieldState;
	@columnConfig({hideViewLink: true}) RQRequest__ReqClassID : PXFieldState;
	@columnConfig({hideViewLink: true}) RQRequest__OrderNbr : PXFieldState;
	@columnConfig({allowUpdate: false, hideViewLink: true}) RQRequest__EmployeeID : PXFieldState;
	@columnConfig({visible: false}) LineNbr : PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({hideViewLink: true}) InventoryID : PXFieldState;
	@columnConfig({hideViewLink: true}) SubItemID : PXFieldState;
	Description : PXFieldState;
	@columnConfig({hideViewLink: true}) UOM : PXFieldState;
	@columnConfig({allowNull: false}) SelectQty : PXFieldState;
	@columnConfig({allowUpdate: false, allowNull: false}) OpenQty : PXFieldState;
	@columnConfig({allowUpdate: false, allowNull: false}) ReqQty : PXFieldState;
	@columnConfig({hideViewLink: true}) VendorID : PXFieldState;
	@columnConfig({hideViewLink: true}) VendorLocationID : PXFieldState;
	VendorName : PXFieldState;
	VendorRefNbr : PXFieldState;
	VendorDescription : PXFieldState;
	AlternateID : PXFieldState;
	RequestedDate : PXFieldState;
	PromisedDate : PXFieldState;
}
