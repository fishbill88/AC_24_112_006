import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXActionState,
	columnConfig,
	PXFieldOptions,
	GridColumnShowHideMode,
	gridConfig,
	viewInfo,
	localizable,
	GridPreset
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.IN.INAdjustmentEntry', primaryView: 'adjustment', bpEventsIndicator: true, showUDFIndicator: true })
export class IN303000 extends PXScreen {

	AddSelectedItems: PXActionState; //Add button on Inventory Lookup panel

	@viewInfo({containerName: "Adjustment Header"})
	adjustment = createSingle(INRegister);

	@viewInfo({containerName: "Adjustment"})
	CurrentDocument = createSingle(INRegister);

	@viewInfo({containerName: "Details"})
	transactions = createCollection(INTran);

	@viewInfo({containerName: "Inventory Lookup Filter"})
	ItemFilter = createSingle(INSiteStatusFilter);

	@viewInfo({containerName: "Inventory Lookup Grid"})
	ItemInfo = createCollection(INSiteStatusSelected);
}

@localizable
export class NullTextValues {
	static Split = "<SPLIT>";
}

export class INRegister extends PXView {
	RefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	TranDate: PXFieldState<PXFieldOptions.CommitChanges>;

	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;

	ExtRefNbr: PXFieldState;
	PIID: PXFieldState;
	TranDesc: PXFieldState;

	TotalQty: PXFieldState;
	ControlQty: PXFieldState;
	TotalCost: PXFieldState;
	ControlCost: PXFieldState;
	//Financial
	BatchNbr: PXFieldState;
	BranchID: PXFieldState;
	BranchBaseCuryID: PXFieldState;
	IgnoreAllocationErrors: PXFieldState;
	//Manufacturing
	AMBatNbr: PXFieldState;
	AMDocType: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
	statusField: "Availability"
})
export class INTran extends PXView {
	ShowItems: PXActionState; //Add Items button on Details tab
	InventorySummary: PXActionState; //Inventory Summary button on Details tab

	@columnConfig({
		visible: false,
		allowShowHide: GridColumnShowHideMode.False
	})
	Availability: PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.Server})
	PILineNbr: PXFieldState;
	@columnConfig({hideViewLink: true})
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true,
		nullText: NullTextValues.Split
	})
	SubItemID: PXFieldState;
	@columnConfig({hideViewLink: true})
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true,
		nullText: NullTextValues.Split
	})
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostLayerType: PXFieldState;
	SpecialOrderCostCenterID: PXFieldState;
	@columnConfig({hideViewLink: true})
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState;
	@columnConfig({hideViewLink: true})
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	UnitCost: PXFieldState;
	TranCost: PXFieldState;
	ManualCost: PXFieldState;
	@columnConfig({
		hideViewLink: true,
		nullText: NullTextValues.Split
	})
	LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpireDate: PXFieldState;
	@columnConfig({hideViewLink: true})
	OrigRefNbr: PXFieldState;
	@columnConfig({hideViewLink: true})
	ReasonCode: PXFieldState<PXFieldOptions.CommitChanges>;
	TranDesc: PXFieldState;
	@columnConfig({hideViewLink: true})
	SOOrderType: PXFieldState;
	SOOrderNbr: PXFieldState;
	SOShipmentNbr: PXFieldState;
	POReceiptType: PXFieldState;
	POReceiptNbr: PXFieldState;
}

export class INSiteStatusFilter extends PXView {
	Inventory: PXFieldState<PXFieldOptions.CommitChanges>;
	BarCode: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClass: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItem: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;

	OnlyAvailable: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	allowInsert: false,
	allowDelete: false
})
export class INSiteStatusSelected extends PXView {
	@columnConfig({allowCheckAll: true})
	Selected: PXFieldState;
	QtySelected: PXFieldState;
	@columnConfig({hideViewLink: true})
	SiteID: PXFieldState;
	@columnConfig({hideViewLink: true})
	LocationID: PXFieldState;
	@columnConfig({hideViewLink: true})
	ItemClassID: PXFieldState;
	ItemClassDescription: PXFieldState;
	@columnConfig({hideViewLink: true})
	PriceClassID: PXFieldState;
	PriceClassDescription: PXFieldState;
	@columnConfig({hideViewLink: true})
	InventoryCD: PXFieldState;
	@columnConfig({hideViewLink: true})
	SubItemID: PXFieldState;
	Descr: PXFieldState;
	@columnConfig({hideViewLink: true})
	BaseUnit: PXFieldState;
	QtyAvail: PXFieldState;
	QtyOnHand: PXFieldState;
}