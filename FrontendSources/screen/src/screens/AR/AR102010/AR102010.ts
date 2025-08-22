import {
	createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType,
	RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo,
	disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType
} from "client-controls";

@graphInfo({graphType: "PX.Objects.AR.ARAccessDetail", primaryView: "Customer" })
export class AR102010 extends PXScreen {


	@viewInfo({containerName: "Customer"})
	Customer = createSingle(Customer);
	@viewInfo({containerName: "Restriction Groups"})
	Groups = createCollection(RelationGroup);

}

export class Customer extends PXView {
	AcctCD: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	AcctName: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({ allowDelete: false, allowInsert: false })
export class RelationGroup extends PXView {

	@columnConfig({ allowCheckAll: true })
	Included: PXFieldState;

	@columnConfig({ hideViewLink: true })
	GroupName: PXFieldState;
	Description: PXFieldState;
	Active: PXFieldState;
	GroupType: PXFieldState;
}
