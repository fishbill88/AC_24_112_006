import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	GridPreset,
	linkCommand,
} from 'client-controls';

export class Filter extends PXView {
	ProcessAction: PXFieldState;
	SOOrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	SOOrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultOrderType: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
})
export class ProcessingRecords extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) OrderType: PXFieldState;
	OrderNbr: PXFieldState;
	LineNbr: PXFieldState;
	SortOrder: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	TranDesc: PXFieldState;
	@columnConfig({ hideViewLink: true }) SiteID: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	@linkCommand("QtyAvailable") OpenQty: PXFieldState;
	RequestDate: PXFieldState;
	ShipDate: PXFieldState;
	AMCTPOrderType: PXFieldState;
	ProdOrdID: PXFieldState;
	CTPDate: PXFieldState;
	ManualProdOrdID: PXFieldState;
	AMCTPAccepted: PXFieldState;
	AMOrigRequestDate: PXFieldState;
}

export class QtyAvailableFilter extends PXView {
	RequestQty: PXFieldState;
	QtyAvail: PXFieldState;
	QtyHardAvail: PXFieldState;
	SupplyAvail: PXFieldState;
	ProdAvail: PXFieldState;
	TotalAvail: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.CTPProcess', primaryView: 'Filter' })
export class AM515000 extends PXScreen {
	Filter = createSingle(Filter);
	ProcessingRecords = createCollection(ProcessingRecords);
	QtyAvailableFilter = createSingle(QtyAvailableFilter);
}
