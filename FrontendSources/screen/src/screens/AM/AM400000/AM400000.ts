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
	linkCommand,
} from 'client-controls';

export class MRPDisplayFilters extends PXView {
	RefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	ParentRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	ProductRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry
})
export class AMRPDetail extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	InventoryID: PXFieldState;
	InventoryID_InventoryItem_descr: PXFieldState;
	SubItemID: PXFieldState;
	ReplenishmentSource: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState;
	BaseQty: PXFieldState;
	PromiseDate: PXFieldState;
	ActionDate: PXFieldState;
	Type: PXFieldState;
	ProductManagerID: PXFieldState;
	PreferredVendorID: PXFieldState;
	Vendor__AcctName: PXFieldState;
	ParentInventoryID: PXFieldState;
	ParentSubItemID: PXFieldState;
	ProductInventoryID: PXFieldState;
	ProductSubItemID: PXFieldState;
	SDFlag: PXFieldState;
	RefType: PXFieldState;
	ActionLeadTime: PXFieldState;
	CreatedDateTime: PXFieldState;
	BOMID: PXFieldState;
	BOMRevisionID: PXFieldState;
	@columnConfig({ hideViewLink: true }) BaseUOM: PXFieldState;
	RecordID: PXFieldState;
	@linkCommand("AMRPDetail$RefNbr$Link") RefNbr: PXFieldState;
	@linkCommand("AMRPDetail$ParentRefNbr$Link") ParentRefNbr: PXFieldState;
	@linkCommand("AMRPDetail$ProductRefNbr$Link") ProductRefNbr: PXFieldState;
	@columnConfig({ hideViewLink: true }) ItemClassID: PXFieldState;
	@columnConfig({ hideViewLink: true }) BranchID: PXFieldState;
	TransferSiteID: PXFieldState;
	KitRevisionID: PXFieldState;
}

export class PlanPurchaseFilter extends PXView {
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
})
export class AMOrderCrossRef extends PXView {
	@columnConfig({ hideViewLink: true }) InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	Source: PXFieldState;
	@columnConfig({ hideViewLink: true }) SiteID: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	PlanDate: PXFieldState;
	GroupNumber: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	ProdOrdID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class PlanTransferFilter extends PXView {
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	FromSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@graphInfo({ graphType: 'PX.Objects.AM.MRPDisplay', primaryView: 'Detailrecs' })
export class AM400000 extends PXScreen {
	// to remove the buttons from the screen toolbar
	AMRPDetail$RefNbr$Link: PXActionState;
	AMRPDetail$ParentRefNbr$Link: PXActionState;
	AMRPDetail$ProductRefNbr$Link: PXActionState;

	MRPDisplayFilters = createSingle(MRPDisplayFilters);
	Detailrecs = createCollection(AMRPDetail);
	PlanPurchaseOrderFilter = createSingle(PlanPurchaseFilter);
	ProcessRecords = createCollection(AMOrderCrossRef);
	PlanTransferOrderFilter = createSingle(PlanTransferFilter);
}
