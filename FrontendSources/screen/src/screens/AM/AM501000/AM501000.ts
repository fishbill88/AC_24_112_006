import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXActionState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	GridPreset,
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AM.APSRoughCutProcess', primaryView: 'OrderList' })
export class AM501000 extends PXScreen {
	orderscheduleInquiry: PXActionState;
	productionDetails: PXActionState;
	criticalMatl: PXActionState;
	inventoryAllocationDetailInq: PXActionState;
	productionScheduleBoardRedirect: PXActionState;

	OrderList = createCollection(AMSchdItem);
	Filter = createSingle(APSRoughCutProcessFilter);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
})
export class AMSchdItem extends PXView {
	@columnConfig({ allowCheckAll: true	}) Selected: PXFieldState;
	QtytoProd: PXFieldState;
	QtyRemaining: PXFieldState;
	@columnConfig({ hideViewLink: true }) AMProdItem__UOM: PXFieldState;
	@columnConfig({ hideViewLink: true }) OrderType: PXFieldState;
	ProdOrdID: PXFieldState;
	InventoryID: PXFieldState;
	InventoryID_description: PXFieldState;
	SchPriority: PXFieldState;
	ConstDate: PXFieldState;
	StartDate_Date: PXFieldState;
	EndDate_Date: PXFieldState;
	SchedulingMethod: PXFieldState;
	ScheduleStatus: PXFieldState;
	FirmSchedule: PXFieldState;
	@columnConfig({ hideViewLink: true }) SiteID: PXFieldState;
	SchdID: PXFieldState;
	AMProdItem__ProdDate: PXFieldState;
	AMProdItem__CustomerID: PXFieldState;
	AMProdItem__OrdNbr: PXFieldState;
	AMProdItem__Descr: PXFieldState;
	AMProdItem__StatusID: PXFieldState;
	StartDate_Time: PXFieldState;
	EndDate_Time: PXFieldState;
	@columnConfig({ hideViewLink: true }) AMProdItem__BranchID: PXFieldState;
}

export class APSRoughCutProcessFilter extends PXView {
	ProcessAction: PXFieldState<PXFieldOptions.CommitChanges>;
	ReleaseOrders: PXFieldState;
	ExcludePlanningOrders: PXFieldState<PXFieldOptions.CommitChanges>;
	ExcludeFirmOrders: PXFieldState<PXFieldOptions.CommitChanges>;
}
