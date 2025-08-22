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
} from 'client-controls';

export class Filter extends PXView {
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	ProdOrdID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
})
export class UnapprovedTrans extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	@columnConfig({ hideViewLink: true }) EmployeeID: PXFieldState;
	@columnConfig({ hideViewLink: true }) OrderType: PXFieldState;
	ProdOrdID: PXFieldState;
	@columnConfig({ hideViewLink: true }) OperationID: PXFieldState;
	@columnConfig({ hideViewLink: true }) ShiftCD: PXFieldState;
	@columnConfig({ hideViewLink: true }) InventoryID: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	TranDate: PXFieldState;
	StartTime_Date: PXFieldState;
	StartTime_Time: PXFieldState;
	EndTime_Date: PXFieldState;
	EndTime_Time: PXFieldState;
	LaborTime: PXFieldState;
	WcID: PXFieldState;
	QtyScrapped: PXFieldState<PXFieldOptions.CommitChanges>;
	ReasonCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	ScrapAction: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class AMClockTranLineSplittingExtension_LotSerOptions extends PXView {
	UnassignedQty: PXFieldState;
	Qty: PXFieldState;
	StartNumVal: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
})
export class splits extends PXView {
	@columnConfig({ hideViewLink: true }) InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LocationID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LotSerialNbr: PXFieldState;
	Qty: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	@columnConfig({ width: 200 }) ExpireDate: PXFieldState;
}


@graphInfo({ graphType: 'PX.Objects.AM.ClockApprovalProcess', primaryView: 'UnapprovedTrans' })
export class AM516000 extends PXScreen {
	AMClockTranLineSplittingExtension_GenerateNumbers: PXActionState;

	Filter = createSingle(Filter);
	UnapprovedTrans = createCollection(UnapprovedTrans);
	AMClockTranLineSplittingExtension_LotSerOptions = createSingle(AMClockTranLineSplittingExtension_LotSerOptions);
	splits = createCollection(splits);
}
