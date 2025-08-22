import {
	createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType,
	RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription,
	ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnType, TextAlign
} from "client-controls";

@graphInfo({graphType: "PX.Objects.AR.ARSalesPriceMaint", primaryView: "Filter" })
export class AR202000 extends PXScreen {

   	@viewInfo({containerName: "Selection"})
	Filter = createSingle(ARSalesPriceFilter);
   	@viewInfo({containerName: "Sales Prices"})
	Records = createCollection(ARSalesPrice);
}

export class ARSalesPriceFilter extends PXView {
	PriceType: PXFieldState<PXFieldOptions.CommitChanges>;
	PriceCode: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCalcMode: PXFieldState<PXFieldOptions.CommitChanges>;
	EffectiveAsOfDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClassCD: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryPriceClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	MyOwner: PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	WorkGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	MyWorkGroup: PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
}

@gridConfig({
	initNewRow: true,
	syncPosition: true
})
export class ARSalesPrice extends PXView {

	@columnConfig({ hideViewLink: true })
	PriceType: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	PriceCode: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	AlternateID: PXFieldState<PXFieldOptions.CommitChanges>;


	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	IsPromotionalPrice: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	SkipLineDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	IsFairValue: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	IsProrated: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	Discountable: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ textAlign: TextAlign.Right })
	BreakQty: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	SalesPrice: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	TaxID: PXFieldState;

	TaxCalcMode: PXFieldState;

	@columnConfig({ hideViewLink: true })
	TaxCategoryID: PXFieldState;

	EffectiveDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpirationDate: PXFieldState;
}