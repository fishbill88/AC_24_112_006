import {
	createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType,
	RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription,
	ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType,
	TextAlign
} from "client-controls";

@graphInfo({graphType: "PX.Objects.AR.ARPriceWorksheetMaint", primaryView: "Document", showUDFIndicator: true  })
export class AR202010 extends PXScreen {

	AddSelectedItems: PXActionState;
	AddAllItems: PXActionState;

   	@viewInfo({containerName: "Worksheet Summary"})
	Document = createSingle(ARPriceWorksheet);
   	@viewInfo({containerName: "Sales Prices"})
	Details = createCollection(ARPriceWorksheetDetail);

   	@viewInfo({containerName: "Add Item to Worksheet"})
	ItemFilter = createSingle(INSiteStatusFilter);
   	@viewInfo({containerName: "Add Item to Worksheet"})
	addItemParameters = createSingle(AddItemParameters);
   	@viewInfo({containerName: "Add Item to Worksheet"})
	ItemInfo = createCollection(ARAddItemSelected);
   	@viewInfo({containerName: "Copy Prices"})
	CopyPricesSettings = createSingle(CopyPricesFilter);
   	@viewInfo({containerName: "Calculate Pending Prices"})
	CalculatePricesSettings = createSingle(CalculatePricesFilter);
}


export class ARPriceWorksheet extends PXView {

	RefNbr: PXFieldState;
	Status: PXFieldState;
	TaxCalcMode: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState<PXFieldOptions.Multiline>;
	EffectiveDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpirationDate: PXFieldState<PXFieldOptions.CommitChanges>;
	OverwriteOverlapping: PXFieldState<PXFieldOptions.CommitChanges>;
	IsPromotional: PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	SkipLineDiscounts: PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	IsFairValue: PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	IsProrated: PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	Discountable: PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
}

@gridConfig({
	initNewRow: true,
	syncPosition: true,
	allowUpdate: false
})
export class ARPriceWorksheetDetail extends PXView {

	ShowItems: PXActionState;
	CopyPrices: PXActionState;
	CalculatePrices: PXActionState;

	PriceType: PXFieldState;

	@columnConfig({ hideViewLink: true })
	PriceCode: PXFieldState;
	AlternateID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false })
	Description: PXFieldState<PXFieldOptions.CommitChanges >;

	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	BreakQty: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ textAlign: TextAlign.Right })
	CurrentPrice: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	PendingPrice: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	SkipLineDiscounts: PXFieldState;

	@columnConfig({ hideViewLink: true })
	TaxID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	TaxCategoryID: PXFieldState;
}

export class INSiteStatusFilter extends PXView {

	Inventory: PXFieldState;
	ItemClass: PXFieldState<PXFieldOptions.CommitChanges>;
	PriceClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	MyOwner: PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	WorkGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	MyWorkGroup: PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
}

export class AddItemParameters extends PXView {

	PriceType: PXFieldState<PXFieldOptions.CommitChanges>;
	PriceCode: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	SkipLineDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ARAddItemSelected extends PXView {

	@columnConfig({ allowNull: false, textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	Selected: PXFieldState;

	InventoryCD: PXFieldState;
	ItemClassID: PXFieldState;
	ItemClassDescription: PXFieldState;
	Descr: PXFieldState;
	PriceClassID: PXFieldState;
	PriceClassDescription: PXFieldState;
	PriceWorkgroupID: PXFieldState;
	PriceManagerID: PXFieldState;
}

export class CopyPricesFilter extends PXView {

	SourcePriceType: PXFieldState<PXFieldOptions.CommitChanges>;
	SourcePriceCode: PXFieldState<PXFieldOptions.CommitChanges>;
	SourceCuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SourceSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	EffectiveDate: PXFieldState<PXFieldOptions.CommitChanges>;
	IsPromotional: PXFieldState<PXFieldOptions.CommitChanges>;
	IsFairValue: PXFieldState<PXFieldOptions.CommitChanges>;
	IsProrated: PXFieldState<PXFieldOptions.CommitChanges>;
	Discountable: PXFieldState<PXFieldOptions.CommitChanges>;
	DestinationPriceType: PXFieldState<PXFieldOptions.CommitChanges>;
	DestinationPriceCode: PXFieldState<PXFieldOptions.CommitChanges>;
	DestinationCuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	DestinationSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	RateTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	CurrencyDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CalculatePricesFilter extends PXView {

	CorrectionPercent: PXFieldState;
	Rounding: PXFieldState;
	UpdateOnZero: PXFieldState<PXFieldOptions.NoLabel>;
	PriceBasis: PXFieldState;
	BaseCuryID: PXFieldState<PXFieldOptions.CommitChanges>;
}
