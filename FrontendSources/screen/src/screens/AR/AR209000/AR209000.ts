import {
	createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs,
	PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings,
	PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, TextAlign
} from "client-controls";

@graphInfo({ graphType: "PX.Objects.AR.ARDiscountMaint", primaryView: "Document" })
export class AR209000 extends PXScreen {

	ViewARDiscountSequence: PXActionState;

	Document = createCollection(ARDiscount);
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar", autoAdjustColumns: true
})
export class ARDiscount extends PXView {

	@linkCommand("ViewARDiscountSequence")
	DiscountID: PXFieldState<PXFieldOptions.CommitChanges>;

	Description: PXFieldState;

	@columnConfig({ allowNull: false })
	Type: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowNull: false })
	ApplicableTo: PXFieldState;

	@columnConfig({ allowNull: false, textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	IsAppliedToDR: PXFieldState;

	@columnConfig({ allowNull: false, textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	IsManual: PXFieldState;

	@columnConfig({ allowNull: false, textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	ExcludeFromDiscountableAmt: PXFieldState;

	@columnConfig({ allowNull: false, textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	SkipDocumentDiscounts: PXFieldState;

	@columnConfig({ allowNull: false, textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	IsAutoNumber: PXFieldState;

	LastNumber: PXFieldState;
}
