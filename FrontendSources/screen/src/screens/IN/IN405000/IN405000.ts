import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	PXPageLoadBehavior,
	PXFieldState,
	PXView,
	PXFieldOptions,
	gridConfig,
	columnConfig,
	GridColumnShowHideMode
} from 'client-controls';

@graphInfo({graphType: 'PX.Objects.IN.InventoryTranHistEnq', primaryView: 'Filter', pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues})
export class IN405000 extends PXScreen {

	@viewInfo({containerName: 'Selection'})
	Filter = createSingle(InventoryTranHistEnqFilter);
	@viewInfo({containerName: 'Transaction Details'})
	ResultRecords = createCollection(InventoryTranHistEnqResult);

}

export class InventoryTranHistEnqFilter extends PXView  {
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemCD: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	SummaryByDay: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeUnreleased: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowAdjUnitCost: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	mergeToolbarWith: "ScreenToolbar",
	adjustPageSize: true
})
export class InventoryTranHistEnqResult extends PXView  {
	GridLineNbr: PXFieldState;
	@columnConfig({hideViewLink: true})
	INTran__InventoryID: PXFieldState;
	INTran__ReleasedDateTime: PXFieldState;
	INTran__TranType: PXFieldState;
	RefNbr: PXFieldState;
	DocType: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({hideViewLink: true})
	INTranSplit__SubItemID: PXFieldState;
	@columnConfig({hideViewLink: true})
	INTran__SiteID: PXFieldState;
	@columnConfig({hideViewLink: true, allowShowHide: GridColumnShowHideMode.Server})
	INTranSplit__LocationID: PXFieldState;
	@columnConfig({hideViewLink: true, allowShowHide: GridColumnShowHideMode.Server})
	INTranSplit__LotSerialNbr: PXFieldState;
	INTran__FinPeriodID: PXFieldState;
	INTran__TranPeriodID: PXFieldState;
	INTran__Released: PXFieldState;
	TranDate: PXFieldState;
	@columnConfig({
		allowFilter: false,
		allowSort: false
	})
	BegQty: PXFieldState;
	QtyIn: PXFieldState;
	QtyOut: PXFieldState;
	@columnConfig({
		allowFilter: false,
		allowSort: false
	})
	EndQty: PXFieldState;
	UnitCost: PXFieldState;
	@columnConfig({hideViewLink: true})
	INTran__SOOrderType: PXFieldState;
	INTran__SOOrderNbr: PXFieldState;
	INTran__POReceiptNbr: PXFieldState;
}