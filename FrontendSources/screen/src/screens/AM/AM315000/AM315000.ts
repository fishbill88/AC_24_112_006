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
	localizable,
} from 'client-controls';

@localizable
export class NullTextValues {
	static Split = "<SPLIT>";
}

@graphInfo({ graphType: 'PX.Objects.AM.ClockEntry', primaryView: 'header' })
export class AM315000 extends PXScreen {
	header = createSingle(AMClockItem);
	transactions = createCollection(AMClockTran);
	AMClockItemLineSplittingExtension_LotSerOptions = createSingle(LotSerOptions);
	splits = createCollection(AMClockItemSplit);
}

export class AMClockItem extends PXView {
	FillCurrentUser: PXActionState;

	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	ProdOrdID: PXFieldState<PXFieldOptions.CommitChanges>;
	OperationID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShiftCD: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState;
	StartTime: PXFieldState;
	LaborTime: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
})
export class AMClockTran extends PXView {
	LineNbr: PXFieldState;
	TranDate: PXFieldState;
	StartTime: PXFieldState;
	EndTime: PXFieldState;
	LaborTime: PXFieldState;
	@columnConfig({ hideViewLink: true }) OrderType: PXFieldState;
	ProdOrdID: PXFieldState;
	@columnConfig({ hideViewLink: true }) OperationID: PXFieldState;
	@columnConfig({ hideViewLink: true }) InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	EmployeeID: PXFieldState;
	@columnConfig({ hideViewLink: true }) ShiftCD: PXFieldState;
	Qty: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	@columnConfig({ hideViewLink: true }) SiteID: PXFieldState;
	@columnConfig({ hideViewLink: true, nullText: NullTextValues.Split }) LocationID: PXFieldState;
	CloseFlg: PXFieldState;
}

export class LotSerOptions extends PXView {
	UnassignedQty: PXFieldState;
	Qty: PXFieldState;
	StartNumVal: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
})
export class AMClockItemSplit extends PXView {
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LocationID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LotSerialNbr: PXFieldState;
	Qty: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	ExpireDate: PXFieldState;
}
