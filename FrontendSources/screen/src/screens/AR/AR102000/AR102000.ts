import {
	createCollection, createSingle, PXScreen, graphInfo, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior,
	PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, PXActionState
} from "client-controls";

@graphInfo({graphType: "PX.Objects.AR.ARAccess", primaryView: "Group", })
export class AR102000 extends PXScreen {

	@viewInfo({containerName: "Restriction Group"})
	Group = createSingle(RelationGroup);

	@viewInfo({containerName: "Users"})
	Users = createCollection(Users);

	@viewInfo({containerName: "Customers"})
	Customer = createCollection(Customer);
}

export class RelationGroup extends PXView {
	GroupName: PXFieldState;
	Description: PXFieldState;
	GroupType: PXFieldState;
	Active: PXFieldState;
}

@gridConfig({ allowDelete: false, quickFilterFields: ["FullName", "Username"] })
export class Users extends PXView {

	@columnConfig({ allowCheckAll: true })
	Included: PXFieldState;

	Username: PXFieldState;
	FullName: PXFieldState;
	Comment: PXFieldState;
}

@gridConfig({ allowDelete: false, quickFilterFields: ["AcctCD", "AcctName"] })
export class Customer extends PXView {

	@columnConfig({ allowCheckAll: true })
	Included: PXFieldState;

	AcctCD: PXFieldState;
	Status: PXFieldState;
	AcctName: PXFieldState;
}
