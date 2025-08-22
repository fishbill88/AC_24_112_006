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

@graphInfo({ graphType: 'PX.Objects.SO.SOCreateShipment', primaryView: 'Filter' })
export class SO501000 extends PXScreen {

	ViewDocument: PXActionState;

	@viewInfo({ containerName: "Process Orders Filter" })
	Filter = createSingle(Filter);
	@viewInfo({ containerName: "Orders" })
	Orders = createCollection(Orders);
}

export class Filter extends PXView {
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	DateSel: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipmentDate: PXFieldState;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	SchedOrderDate: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CarrierPluginID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipVia: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
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
	@columnConfig({ hideViewLink: true }) OrderType: PXFieldState;
	@linkCommand('ViewDocument')
	OrderNbr: PXFieldState;
	OrderDesc: PXFieldState;
	CustomerOrderNbr: PXFieldState;
	Status: PXFieldState;
	MinSchedOrderDate: PXFieldState;
	ExpireDate: PXFieldState;
	CuryPaymentTotal: PXFieldState;
	CuryUnpaidBalance: PXFieldState;
	@columnConfig({ hideViewLink: true }) TermsID: PXFieldState;
	RequestDate: PXFieldState;
	ShipDate: PXFieldState;
	ShipSeparately: PXFieldState;
	@columnConfig({ hideViewLink: true }) CustomerID: PXFieldState;
	CustomerID_BAccountR_acctName: PXFieldState;
	@columnConfig({ hideViewLink: true }) CustomerLocationID: PXFieldState;
	CustomerLocationID_Location_descr: PXFieldState;
	@columnConfig({ hideViewLink: true }) DefaultSiteID: PXFieldState;
	DefaultSiteID_INSite_descr: PXFieldState;
	@columnConfig({ hideViewLink: true }) ShipVia: PXFieldState;
	ShipVia_Carrier_description: PXFieldState;
	@columnConfig({ hideViewLink: true }) ShipZoneID: PXFieldState;
	@columnConfig({ hideViewLink: true }) WorkgroupID: PXFieldState;
	@columnConfig({ hideViewLink: true }) OwnerID: PXFieldState;
	OrderWeight: PXFieldState;
	OrderVolume: PXFieldState;
	OrderQty: PXFieldState;
	@columnConfig({ hideViewLink: true }) CuryID: PXFieldState;
	CuryOrderTotal: PXFieldState;
}