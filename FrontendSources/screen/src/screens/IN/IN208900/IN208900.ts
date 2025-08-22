import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, GridPreset } from 'client-controls';

@graphInfo({graphType: 'PX.Objects.IN.INPIClassMaint', primaryView: 'Classes', })
export class IN208900 extends PXScreen {
	@viewInfo({containerName: 'Physical Inventory Type Summary'})
	Classes = createSingle(Header);
	@viewInfo({containerName: 'PI Class'})
	CurrentClass = createSingle(INPIClass);
	@viewInfo({containerName: 'Items'})
	Items = createCollection(INPIClassItem);

	@viewInfo({containerName: 'Item Classes'})
	ItemClasses = createCollection(INPIClassItemClass);

	@viewInfo({containerName: 'Locations'})
	Locations = createCollection(INPIClassLocation);

	@viewInfo({containerName: 'Add Locations'})
	LocationFilter = createSingle(INPILocationFilter);
	@viewInfo({containerName: 'Add Items'})
	InventoryFilter = createSingle(INPIInventoryFilter);
}

// Views

export class Header extends PXView  {
	PIClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	Method: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeZeroItems: PXFieldState;
	UnlockSiteOnCountingFinish: PXFieldState<PXFieldOptions.CommitChanges>;
	HideBookQty: PXFieldState;
}

export class INPIClass extends PXView  {
	SelectedMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	RandomItemsLimit: PXFieldState;
	LastCountPeriod: PXFieldState;
	ABCCodeID: PXFieldState;
	ByABCFrequency: PXFieldState<PXFieldOptions.CommitChanges>;
	MovementClassID: PXFieldState;
	ByMovementClassFrequency: PXFieldState<PXFieldOptions.CommitChanges>;
	CycleID: PXFieldState;
	ByCycleFrequency: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	NAO1: PXFieldState;
	NAO2: PXFieldState;
	NAO3: PXFieldState;
	NAO4: PXFieldState;
	BlankLines: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true
})
export class INPIClassItem extends PXView  {
	addItem: PXActionState;

	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryItem__Descr: PXFieldState;
	@columnConfig({allowNull: false, nullText: "Active"})
	InventoryItem__ItemStatus: PXFieldState;
	InventoryItem__ItemClassID: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details
})
export class INPIClassItemClass extends PXView  {
	ItemClassID: PXFieldState;
	INItemClass__Descr: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details
})
export class INPIClassLocation extends PXView  {
	AddLocation: PXActionState; // Add Location button on Warehouse/Location Selection tab.

	LocationID: PXFieldState;
	INLocation__Descr: PXFieldState;
	@columnConfig({allowNull: false, nullText: "1"})
	INLocation__PickPriority: PXFieldState;
}

export class INPILocationFilter extends PXView  {
	StartLocationID: PXFieldState;
	EndLocationID: PXFieldState;
}

export class INPIInventoryFilter extends PXView  {
	StartInventoryID: PXFieldState;
	EndInventoryID: PXFieldState;
}
