import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, GridPreset } from 'client-controls';

@graphInfo({graphType: 'PX.Objects.IN.INPIReview', primaryView: 'PIHeader', pageLoadBehavior: PXPageLoadBehavior.GoLastRecord, showUDFIndicator: true })
export class IN305000 extends PXScreen {
	AddLine2: PXActionState;

	@viewInfo({containerName: 'Document Summary'})
	PIHeader = createSingle(INPIHeader);
	@viewInfo({containerName: 'Document Summary'})
	PIHeaderInfo = createSingle(INPIHeader2);
	@viewInfo({containerName: 'Generate Physical Count'})
	GeneratorSettings = createSingle(PIGeneratorSettings);
	@viewInfo({containerName: 'Add Line'})
	AddByBarCode = createSingle(INBarCodeItem);
	@viewInfo({containerName: 'Physical Inventory Details'})
	PIDetail = createCollection(INPIDetail);
}

// Views

export class INPIHeader extends PXView  {
	PIID: PXFieldState;
	SiteID: PXFieldState;
	Status: PXFieldState;
	CountDate: PXFieldState;
	Descr: PXFieldState;
}

export class INPIHeader2 extends PXView  {
	TotalPhysicalQty: PXFieldState<PXFieldOptions.Disabled>;
	TotalVarQty: PXFieldState<PXFieldOptions.Disabled>;
	TotalVarCost: PXFieldState<PXFieldOptions.Disabled>;
	BaseCuryID: PXFieldState<PXFieldOptions.Disabled>;
	PIAdjRefNbr: PXFieldState<PXFieldOptions.Disabled>;
}

export class PIGeneratorSettings extends PXView  {
	PIClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	Method: PXFieldState<PXFieldOptions.Disabled>;
}

export class INBarCodeItem extends PXView  {
	BarCode: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState<PXFieldOptions.Disabled>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpireDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ByOne: PXFieldState<PXFieldOptions.CommitChanges>;
	AutoAddLine: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
	allowImport: true,
	adjustPageSize: true
})
export class INPIDetail extends PXView  {
	AddLine: PXActionState;

	@columnConfig({allowUpdate: false})	Status: PXFieldState;
	LineNbr: PXFieldState;
	TagNumber: PXFieldState;
	InventoryID: PXFieldState;
	InventoryID_InventoryItem_descr: PXFieldState;
	SubItemID: PXFieldState;
	@columnConfig({hideViewLink: true})	LocationID: PXFieldState;
	LotSerialNbr: PXFieldState;
	ExpireDate: PXFieldState;
	BookQty: PXFieldState;
	@columnConfig({hideViewLink: true})	InventoryItem__BaseUnit: PXFieldState;
	PhysicalQty: PXFieldState<PXFieldOptions.CommitChanges>;
	VarQty: PXFieldState;
	UnitCost: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowUpdate: false})	ExtVarCost: PXFieldState;
	FinalExtVarCost: PXFieldState;
	ManualCost: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true}) ReasonCode: PXFieldState;
}
