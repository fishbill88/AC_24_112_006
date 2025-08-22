import {
	RQ302000
} from '../RQ302000';

import {
	PXView,
	createCollection,
	PXFieldState,
	gridConfig,
	columnConfig,
	viewInfo,
	PXActionState,
	GridPreset
} from 'client-controls';

export interface RQ302000_RequestDetails extends RQ302000 { }
export class RQ302000_RequestDetails {
	@viewInfo({containerName: "Request Details"})
	Contents = createCollection(RQRequisitionContent);
}

@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false
})
export class RQRequisitionContent extends PXView  {
	viewRequest : PXActionState;
	addRequestContent : PXActionState;

	RQRequest__Priority : PXFieldState;
	RQRequest__OrderNbr : PXFieldState;
	RQRequest__OrderDate : PXFieldState;
	RQRequest__EmployeeID : PXFieldState;
	RQRequest__DepartmentID : PXFieldState;
	@columnConfig({allowUpdate: false, hideViewLink: true}) RQRequestLine__InventoryID : PXFieldState;
	@columnConfig({allowUpdate: false, hideViewLink: true}) RQRequestLine__UOM : PXFieldState;
	@columnConfig({allowUpdate: false}) RQRequestLine__Description : PXFieldState;
	@columnConfig({allowNull: false}) ItemQty : PXFieldState;
	@columnConfig({allowNull: false}) RQRequestLine__OpenQty : PXFieldState;
}
