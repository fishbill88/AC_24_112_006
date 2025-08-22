import {
	PXScreen,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXPageLoadBehavior,
} from 'client-controls';

export class ManufacturingDiagramFilter extends PXView {
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	ProdOrdId: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderStatus: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduleStatus: PXFieldState<PXFieldOptions.CommitChanges>;
	ProductWorkgroupId: PXFieldState<PXFieldOptions.CommitChanges>;
	ProductManagerId: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeOnHold: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SoOrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	SoNumber: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	DateFrom: PXFieldState<PXFieldOptions.CommitChanges>;
	DateTo: PXFieldState<PXFieldOptions.CommitChanges>;
	ColorCodingOrders: PXFieldState<PXFieldOptions.CommitChanges>;
}

@graphInfo({ graphType: 'PX.Objects.AM.ManufacturingDiagram', primaryView: 'Filter', pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues })
export class AM215555 extends PXScreen {
	Filter = createSingle(ManufacturingDiagramFilter);
}
