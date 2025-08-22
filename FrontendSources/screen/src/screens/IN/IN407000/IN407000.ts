import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	PXPageLoadBehavior,
	PXView,
	PXFieldOptions,
	PXFieldState,
	gridConfig,
	columnConfig,
	GridColumnShowHideMode
} from 'client-controls';

@graphInfo({graphType: 'PX.Objects.IN.InventoryLotSerInq', primaryView: 'Filter', pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues})
export class IN407000 extends PXScreen {

	@viewInfo({containerName: 'Selection'})
	Filter = createSingle(INLotSerFilter);
	@viewInfo({containerName: 'Transaction Details'})
	Records = createCollection(INTranSplit);
}

export class INLotSerFilter extends PXView  {
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemCD: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowAdjUnitCost: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	allowDelete: false,
	allowInsert: false,
	mergeToolbarWith: "ScreenToolbar",
	adjustPageSize: true
})
export class INTranSplit extends PXView  {
	InventoryID: PXFieldState;
	TranDate: PXFieldState;
	TranType: PXFieldState;
	DocType: PXFieldState<PXFieldOptions.Hidden>;
	RefNbr: PXFieldState;
	SubItemID: PXFieldState;
	SiteID: PXFieldState;
	@columnConfig({hideViewLink: true, allowShowHide: GridColumnShowHideMode.Server})
	LocationID: PXFieldState;
	LotSerialNbr: PXFieldState;
	ExpireDate: PXFieldState;
	@columnConfig({hideViewLink: true})
	UOM: PXFieldState;
	InvQty: PXFieldState;
	TranUnitCost: PXFieldState;
	SOOrderType: PXFieldState;
	SOOrderNbr: PXFieldState;
	Customer__AcctCD: PXFieldState;
	Customer__AcctName: PXFieldState;
	POReceiptLine__ReceiptNbr: PXFieldState;
	POReceiptType: PXFieldState;
	Vendor__AcctCD: PXFieldState;
	Vendor__AcctName: PXFieldState;
	Released: PXFieldState;
	InventoryID_InventoryItem_Descr: PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.Server})
	TotalQty: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({allowShowHide: GridColumnShowHideMode.Server})
	TotalCost: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({allowShowHide: GridColumnShowHideMode.Server})
	AdditionalCost: PXFieldState<PXFieldOptions.Hidden>;
}