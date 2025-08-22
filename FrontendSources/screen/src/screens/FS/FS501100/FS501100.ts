import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.ServiceOrderProcess', primaryView: 'Filter' })
export class FS501100 extends PXScreen {
	Filter = createSingle(ServiceOrderFilter);
	ServiceOrderRecords = createCollection(FSServiceOrder);
}

export class ServiceOrderFilter extends PXView {
	SOAction: PXFieldState<PXFieldOptions.CommitChanges>;
	SrvOrdType: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	ServiceContractID: PXFieldState<PXFieldOptions.CommitChanges>;
	FromDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ToDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	mergeToolbarWith: "ScreenToolbar"
})
export class FSServiceOrder extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	BranchID: PXFieldState;
	@columnConfig({ hideViewLink: true }) BranchLocationID: PXFieldState;
	@columnConfig({ hideViewLink: true }) SrvOrdType: PXFieldState;
	RefNbr: PXFieldState;
	DocDesc: PXFieldState;
	CustomerID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LocationID: PXFieldState;
	OrderDate: PXFieldState;
	Status: PXFieldState;
	WFStageID: PXFieldState;
	ServiceContractID: PXFieldState;
	ScheduleID: PXFieldState;
}
