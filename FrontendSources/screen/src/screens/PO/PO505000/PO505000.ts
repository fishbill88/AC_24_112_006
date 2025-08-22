import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, GridPreset } from "client-controls";

@graphInfo({graphType: "PX.Objects.PO.POCreate", primaryView: "Filter"})
export class PO505000 extends PXScreen {
	ViewDocument: PXActionState;
	ViewServiceOrderDocument: PXActionState;
	ViewProdDocument: PXActionState;

	@viewInfo({containerName: "Selection"})
	Filter = createSingle(POCreateFilter);
   	@viewInfo({containerName: "Details"})
	FixedDemand = createCollection(INItemPlan);
}

// Views

export class POCreateFilter extends PXView  {
	BranchID : PXFieldState<PXFieldOptions.CommitChanges>;
	PurchDate : PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID : PXFieldState<PXFieldOptions.CommitChanges>;
	MyOwner : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	WorkGroupID : PXFieldState<PXFieldOptions.CommitChanges>;
	MyWorkGroup : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	ItemClassCD : PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID : PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID : PXFieldState<PXFieldOptions.CommitChanges>;
	RequestedOnDate : PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID : PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID : PXFieldState<PXFieldOptions.CommitChanges>;
	OrderType : PXFieldState<PXFieldOptions.CommitChanges>;
	OrderNbr : PXFieldState<PXFieldOptions.CommitChanges>;
	SrvOrdType : PXFieldState<PXFieldOptions.CommitChanges>;
	serviceOrderRefNbr : PXFieldState<PXFieldOptions.CommitChanges>;
	AMOrderType : PXFieldState<PXFieldOptions.CommitChanges>;
	ProdOrdID : PXFieldState<PXFieldOptions.CommitChanges>;
	OrderTotal : PXFieldState<PXFieldOptions.Disabled>;
	OrderWeight : PXFieldState<PXFieldOptions.Disabled>;
	OrderVolume : PXFieldState<PXFieldOptions.Disabled>;
	OrderQty : PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	quickFilterFields: ['InventoryID', 'POSiteID', 'VendorID', 'SOOrder__CustomerID']
})
export class INItemPlan extends PXView  {
	@columnConfig({allowNull: false, allowCheckAll: true})	Selected : PXFieldState;
	@columnConfig({allowUpdate: false})	LocalizedPlanDescr : PXFieldState;
	@columnConfig({allowUpdate: false})	InventoryID : PXFieldState;
	@columnConfig({allowUpdate: false})	InventoryID_InventoryItem_descr : PXFieldState;
	@columnConfig({allowUpdate: false, hideViewLink: true})	SubItemID : PXFieldState;
	@columnConfig({allowUpdate: false})	POSiteID : PXFieldState;
	@columnConfig({allowUpdate: false})	POSiteID_description : PXFieldState;
	@columnConfig({allowUpdate: false})	SourceSiteID : PXFieldState;
	@columnConfig({allowUpdate: false})	SourceSiteDescr : PXFieldState;
	@columnConfig({allowUpdate: false, hideViewLink: true})	DemandUOM : PXFieldState;
	@columnConfig({allowUpdate: false})	OrderQty : PXFieldState;
	@columnConfig({allowUpdate: false})	RequestedDate : PXFieldState;
	@columnConfig({allowUpdate: false})	VendorID : PXFieldState;
	@columnConfig({allowUpdate: false})	VendorID_Vendor_acctName : PXFieldState;
	@columnConfig({allowUpdate: false, hideViewLink: true})	VendorLocationID : PXFieldState;
	@columnConfig({allowUpdate: false})	Location__vLeadTime : PXFieldState;
	@columnConfig({allowUpdate: false})	AddLeadTimeDays : PXFieldState;
	@columnConfig({allowUpdate: false, hideViewLink: true})	Vendor__TermsID : PXFieldState;
	@columnConfig({allowUpdate: false, hideViewLink: true})	Location__vCarrierID : PXFieldState;
	@columnConfig({allowUpdate: false})	effPrice : PXFieldState;
	@columnConfig({allowUpdate: false})	ExtCost : PXFieldState;
	@columnConfig({allowUpdate: false, hideViewLink: true})	CuryID : PXFieldState;
	@columnConfig({allowUpdate: false})	SOOrder__CustomerID : PXFieldState;
	@columnConfig({allowUpdate: false})	SOOrder__CustomerID_BAccountR_acctName : PXFieldState;
	@columnConfig({allowUpdate: false, hideViewLink: true})	SOOrder__CustomerLocationID : PXFieldState;
	@columnConfig({allowUpdate: false})	SOLine__UnitPrice : PXFieldState;
	@columnConfig({allowUpdate: false, hideViewLink: true})	SOLine__UOM : PXFieldState;
	@linkCommand("viewDocument")
	SOOrder__OrderNbr : PXFieldState;
	AMProdMatlSplitPlan__OrderType: PXFieldState;
	@linkCommand("viewProdDocument")
	AMProdMatlSplitPlan__ProdOrdID: PXFieldState;
	@linkCommand("viewServiceOrderDocument")
	FSRefNbr : PXFieldState;
	@columnConfig({allowUpdate: false, allowNull: false})	ExtWeight : PXFieldState;
	@columnConfig({allowUpdate: false, allowNull: false})	ExtVolume : PXFieldState;
}
