import {
	PXScreen,
	createSingle,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	columnConfig,
	PXFieldOptions,
	viewInfo,
	PXActionState,
	linkCommand,
	gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.SO.SOOrderProcess', primaryView: 'Filter' })
export class SO502000 extends PXScreen {

	ViewDocument: PXActionState;

	@viewInfo({ containerName: "Print/Email Orders Filter" })
	Filter = createSingle(Filter);
	@viewInfo({ containerName: "Orders" })
	Records = createCollection(Records);
}

export class Filter extends PXView {
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	MyOwner: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	MyWorkGroup: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesPersonID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowAll: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintWithDeviceHub: PXFieldState<PXFieldOptions.CommitChanges>;
	DefinePrinterManually: PXFieldState<PXFieldOptions.CommitChanges>;
	PrinterID: PXFieldState<PXFieldOptions.CommitChanges>;
	NumberOfCopies: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	adjustPageSize: true,
	syncPosition: true,
	mergeToolbarWith: 'ScreenToolbar',
	quickFilterFields: ["CustomerID"]
})
export class Records extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OrderType: PXFieldState;
	@linkCommand('ViewDocument')
	OrderNbr: PXFieldState;
	OrderDesc: PXFieldState;
	CustomerOrderNbr: PXFieldState;
	Status: PXFieldState;
	RequestDate: PXFieldState;
	ShipDate: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CustomerID: PXFieldState;
	CustomerID_BAccountR_acctName: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CustomerLocationID: PXFieldState;
	CustomerLocationID_Location_descr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	DefaultSiteID: PXFieldState;
	DefaultSiteID_INSite_descr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ShipVia: PXFieldState;
	ShipVia_Carrier_description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ShipZoneID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	WorkgroupID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OwnerID: PXFieldState;
	OrderWeight: PXFieldState;
	OrderVolume: PXFieldState;
	OrderQty: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
	CuryOrderTotal: PXFieldState;
	Emailed: PXFieldState;
}