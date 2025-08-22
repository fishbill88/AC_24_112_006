import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	PXPageLoadBehavior,
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	columnConfig,
	GridPreset
} from 'client-controls';

@graphInfo({graphType: 'PX.Objects.IN.InventoryTranSumEnq', primaryView: 'Filter', pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues})
export class IN406000 extends PXScreen {

	@viewInfo({containerName: 'Selection'})
	Filter = createSingle(InventoryTranSumEnqFilter);
	@viewInfo({containerName: 'Transaction Details'})
	ResultRecords = createCollection(INItemSiteHist);
}

export class InventoryTranSumEnqFilter extends PXView  {
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	ByFinancialPeriod: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemCD: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowItemsWithoutMovement: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemDetails: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteDetails: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationDetails: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry
})
export class INItemSiteHist extends PXView  {
	@columnConfig({ hideViewLink: true })
	FinPeriodID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	InventoryID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SiteID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	LocationID: PXFieldState;
	TranBegQty: PXFieldState;
	TranPtdQtyIssued: PXFieldState;
	TranPtdQtyReceived: PXFieldState;
	TranPtdQtySales: PXFieldState;
	TranPtdQtyCreditMemos: PXFieldState;
	TranPtdQtyDropShipSales: PXFieldState;
	TranPtdQtyTransferIn: PXFieldState;
	TranPtdQtyTransferOut: PXFieldState;
	TranPtdQtyAssemblyIn: PXFieldState;
	TranPtdQtyAssemblyOut: PXFieldState;
	TranPtdQtyAdjusted: PXFieldState;
	TranYtdQty: PXFieldState;
	InventoryID_InventoryItem_Descr: PXFieldState;
}