import {
	PXScreen, createSingle, createCollection, graphInfo, PXView, PXFieldState, PXFieldOptions, columnConfig, PXActionState
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AP.APPriceWorksheetMaint', primaryView: 'Document', showUDFIndicator: true })
export class AP202010 extends PXScreen {

	Document = createSingle(APPriceWorksheet);
	Details = createCollection(APPriceWorksheetDetail);
	ItemFilter = createSingle(AddItemFilter);
	addItemParameters = createSingle(AddItemParameters);
	ItemInfo = createCollection(APAddItemSelected);
	CopyPricesSettings = createSingle(CopyPricesFilter);
	CalculatePricesSettings = createSingle(CalculatePricesFilter);

}

export class APPriceWorksheet extends PXView {

	RefNbr: PXFieldState;
	Status: PXFieldState;
	Descr: PXFieldState;
	EffectiveDate: PXFieldState<PXFieldOptions.CommitChanges>;
	IsPromotional: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpirationDate: PXFieldState<PXFieldOptions.CommitChanges>;
	OverwriteOverlapping: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class APPriceWorksheetDetail extends PXView {

	ShowItems: PXActionState;
	CopyPrices: PXActionState;
	CalculatePrices: PXActionState;

	VendorID: PXFieldState;
	AlternateID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false })
	Description: PXFieldState<PXFieldOptions.CommitChanges>;

	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState;
	BreakQty: PXFieldState<PXFieldOptions.CommitChanges>;
	CurrentPrice: PXFieldState;
	PendingPrice: PXFieldState;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxID: PXFieldState;

}

export class AddItemFilter extends PXView {

	Inventory: PXFieldState;
	ItemClass: PXFieldState<PXFieldOptions.CommitChanges>;
	PriceClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	MyOwner: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	MyWorkGroup: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class AddItemParameters extends PXView {

	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class APAddItemSelected extends PXView {

	@columnConfig({ allowNull: false, allowCheckAll: true })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	InventoryCD: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ItemClassID: PXFieldState;

	ItemClassDescription: PXFieldState;
	Descr: PXFieldState;
	PriceClassID: PXFieldState;
	PriceClassDescription: PXFieldState;
	ProductWorkgroupID: PXFieldState;
	ProductManagerID: PXFieldState;

}

export class CopyPricesFilter extends PXView {

	SourceVendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	SourceCuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SourceSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	EffectiveDate: PXFieldState<PXFieldOptions.CommitChanges>;
	IsPromotional: PXFieldState<PXFieldOptions.CommitChanges>;
	DestinationVendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	DestinationCuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	DestinationSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	RateTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	CurrencyDate: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class CalculatePricesFilter extends PXView {

	CorrectionPercent: PXFieldState;
	Rounding: PXFieldState;
	UpdateOnZero: PXFieldState;
	PriceBasis: PXFieldState;
	BaseCuryID: PXFieldState<PXFieldOptions.CommitChanges>;

}
