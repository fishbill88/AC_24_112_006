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
	gridConfig,
	viewInfo,
	localizable,
	GridColumnShowHideMode,
	GridPreset
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.IN.INTransferEntry', primaryView: 'transfer', bpEventsIndicator: true, showUDFIndicator: true })
export class IN304000 extends PXScreen {

	LineSplittingExtension_GenerateNumbers: PXActionState; //Generate button on Line Details panel
	AddSelectedItems: PXActionState; //Add button on Inventory Lookup panel

	@viewInfo({containerName: "Transfer Header"})
	transfer = createSingle(INRegister);

	@viewInfo({containerName: "Transfer"})
	CurrentDocument = createSingle(INRegister);

	@viewInfo({containerName: "Details"})
	transactions = createCollection(INTran);

	@viewInfo({containerName: "Line Details Header"})
	LineSplittingExtension_LotSerOptions = createSingle(LineSplittingHeader);

	@viewInfo({containerName: "Line Details"})
	splits = createCollection(INTranSplit);

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
	TransferType: PXFieldState<PXFieldOptions.CommitChanges>;
	TranDate: PXFieldState<PXFieldOptions.CommitChanges>;

	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;

	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ToSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExtRefNbr: PXFieldState;
	TranDesc: PXFieldState;

	TotalQty: PXFieldState;
	ControlQty: PXFieldState;
	//Financial
	BatchNbr: PXFieldState;
	BranchID: PXFieldState;
	BranchBaseCuryID: PXFieldState;
	POReceiptNbr: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
	statusField: "Availability"
})
export class INTran extends PXView {
	LineSplittingExtension_ShowSplits: PXActionState; //Line Details button on Details tab
	ShowItems: PXActionState; //Add Items button on Details tab
	InventorySummary: PXActionState; //Inventory Summary button on Details tab

	@columnConfig({
		visible: false,
		allowShowHide: GridColumnShowHideMode.False
	})
	Availability: PXFieldState;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true,
		nullText: NullTextValues.Split
	})
	SubItemID: PXFieldState;
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
	@columnConfig({
		hideViewLink: true,
		nullText: NullTextValues.Split
	})
	ToLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	ToCostLayerType: PXFieldState;
	ToSpecialOrderCostCenterID: PXFieldState;
	@columnConfig({hideViewLink: true})
	ToProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	ToTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	ToCostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState;
	ReceiptedQty: PXFieldState;
	INTransitQty: PXFieldState;
	@columnConfig({
		hideViewLink: true,
		nullText: NullTextValues.Split
	})
	LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpireDate: PXFieldState;
	@columnConfig({hideViewLink: true})
	ReasonCode: PXFieldState<PXFieldOptions.CommitChanges>;
	TranDesc: PXFieldState;
	LineNbr: PXFieldState;
}

export class INSiteStatusFilter extends PXView {
	Inventory: PXFieldState<PXFieldOptions.CommitChanges>;
	BarCode: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClass: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItem: PXFieldState<PXFieldOptions.CommitChanges>;
	ReceiptNbr: PXFieldState<PXFieldOptions.CommitChanges>;

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

export class LineSplittingHeader extends PXView {
	UnassignedQty: PXFieldState<PXFieldOptions.Disabled>;
	Qty: PXFieldState;
	StartNumVal: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	initNewRow: true
})
export class INTranSplit extends PXView {
	@columnConfig({hideViewLink: true})
	InventoryID: PXFieldState;
	@columnConfig({hideViewLink: true})
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState;
	@columnConfig({hideViewLink: true})
	UOM: PXFieldState;
}