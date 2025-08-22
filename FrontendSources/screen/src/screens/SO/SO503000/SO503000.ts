import {
	PXScreen,
	createSingle,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	columnConfig,
	linkCommand,
	PXFieldOptions,
	PXActionState,
	viewInfo,
	gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.SO.SOInvoiceShipment', primaryView: 'Filter' })
export class SO503000 extends PXScreen {

	ViewDocument: PXActionState;

	@viewInfo({ containerName: "Process Shipments Filter" })
	Filter = createSingle(Filter);
	@viewInfo({ containerName: "Shipments" })
	Orders = createCollection(Orders);
}

export class Filter extends PXView {
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoiceDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowPrinted: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CarrierPluginID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipVia: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	PackagingType: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintWithDeviceHub: PXFieldState<PXFieldOptions.CommitChanges>;
	DefinePrinterManually: PXFieldState<PXFieldOptions.CommitChanges>;
	PrinterID: PXFieldState<PXFieldOptions.CommitChanges>;
	NumberOfCopies: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	adjustPageSize: true,
	syncPosition: true,
	mergeToolbarWith: 'ScreenToolbar',
	batchUpdate: true
})
export class Orders extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	@linkCommand('ViewDocument')
	ShipmentNbr: PXFieldState;
	Status: PXFieldState;
	ShipDate: PXFieldState;
	@columnConfig({ hideViewLink: true })CustomerID: PXFieldState;
	CustomerID_BAccountR_acctName: PXFieldState;
	@columnConfig({ hideViewLink: true })CustomerLocationID: PXFieldState;
	CustomerLocationID_Location_descr: PXFieldState;
	CustomerOrderNbr: PXFieldState;
	BillingInOrders: PXFieldState;
	BillSeparately: PXFieldState;
	@columnConfig({ hideViewLink: true })SiteID: PXFieldState;
	SiteID_INSite_descr: PXFieldState;
	@columnConfig({ hideViewLink: true })WorkgroupID: PXFieldState;
	@columnConfig({ hideViewLink: true })OwnerID: PXFieldState;
	ShipmentQty: PXFieldState;
	@columnConfig({ hideViewLink: true })ShipVia: PXFieldState;
	ShipVia_Carrier_description: PXFieldState;
	ShipmentWeight: PXFieldState;
	ShipmentVolume: PXFieldState;
	LabelsPrinted: PXFieldState;
}