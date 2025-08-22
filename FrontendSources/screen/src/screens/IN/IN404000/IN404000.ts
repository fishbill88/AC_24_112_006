import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	PXPageLoadBehavior,
	PXFieldState,
	PXFieldOptions,
	PXView,
	gridConfig,
	columnConfig,
	GridColumnShowHideMode,
	GridPreset
} from 'client-controls';

@graphInfo({graphType: 'PX.Objects.IN.InventoryTranDetEnq', primaryView: 'Filter', pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues})
export class IN404000 extends PXScreen {

	@viewInfo({containerName: 'Selection'})
	Filter = createSingle(InventoryTranDetEnqFilter);
	@viewInfo({containerName: 'Transaction Details'})
	ResultRecords = createCollection(InventoryTranDetEnqResult);
}

export class InventoryTranDetEnqFilter extends PXView  {
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	ByFinancialPeriod: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemCD: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodStartDate: PXFieldState;
	PeriodEndDateInclusive: PXFieldState;
	SummaryByDay: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeUnreleased: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	batchUpdate: true,
	quickFilterFields: ["LocationID", "LotSerialNbr"]
})
export class InventoryTranDetEnqResult extends PXView  {
	GridLineNbr: PXFieldState;
	@columnConfig({hideViewLink: true})
	INTran__InventoryID: PXFieldState;
	TranDate: PXFieldState;
	TranType: PXFieldState;
	RefNbr: PXFieldState;
	DocType: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({hideViewLink: true})
	SubItemID: PXFieldState;
	@columnConfig({hideViewLink: true})
	INTran__SiteID: PXFieldState;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.Server,
		hideViewLink: true
	})
	LocationID: PXFieldState;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.Server,
		hideViewLink: true
	})
	LotSerialNbr: PXFieldState;
	INTran__FinPeriodID: PXFieldState;
	INTran__TranPeriodID: PXFieldState;
	Released: PXFieldState;
	INTran__ReleasedDateTime: PXFieldState;
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
	BegBalance: PXFieldState;
	ExtCostIn: PXFieldState;
	ExtCostOut: PXFieldState;
	EndBalance: PXFieldState;
	UnitCost: PXFieldState;
	@columnConfig({hideViewLink: true})
	INTran__SOOrderType: PXFieldState;
	INTran__SOOrderNbr: PXFieldState;
	INTran__POReceiptNbr: PXFieldState;
}