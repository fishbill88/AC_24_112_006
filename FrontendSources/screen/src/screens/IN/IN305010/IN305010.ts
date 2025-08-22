import { Messages as SysMessages } from 'client-controls/services/messages';
import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType } from 'client-controls';

@graphInfo({graphType: 'PX.Objects.IN.INPICountEntry',  primaryView: 'PIHeader' })
export class IN305010 extends PXScreen {
	AddLine2: PXActionState;

	@viewInfo({containerName: 'Document Summary'})
	PIHeader = createSingle(INPIHeader);
	@viewInfo({containerName: 'Document Summary'})
	Filter = createSingle(PICountFilter);
	@viewInfo({containerName: 'Physical Inventory Details'})
	PIDetail = createCollection(INPIDetail);
	@viewInfo({containerName: 'Add Line'})
	AddByBarCode = createSingle(INBarCodeItem);
}

// Views

export class INPIHeader extends PXView  {
	PIID: PXFieldState;
	SiteID: PXFieldState<PXFieldOptions.Disabled>;
	Descr: PXFieldState;
	CountDate: PXFieldState<PXFieldOptions.Disabled>;
	LineCntr: PXFieldState<PXFieldOptions.Disabled>;
}

export class PICountFilter extends PXView  {
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItem: PXFieldState<PXFieldOptions.CommitChanges>;
	LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	StartLineNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	EndLineNbr: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({allowDelete: false, allowInsert: false, allowImport: false, adjustPageSize: true})
export class INPIDetail extends PXView  {
	AddLine: PXActionState;

	@columnConfig({allowUpdate: false})	Status: PXFieldState;
	@columnConfig({allowUpdate: false})	LineNbr: PXFieldState;
	@columnConfig({allowUpdate: false})	TagNumber: PXFieldState;
	InventoryID: PXFieldState;
	InventoryID_InventoryItem_descr: PXFieldState;
	SubItemID: PXFieldState;
	@columnConfig({hideViewLink: true}) LocationID: PXFieldState;
	LotSerialNbr: PXFieldState;
	@columnConfig({allowUpdate: false, allowShowHide: GridColumnShowHideMode.Server})	BookQty: PXFieldState;
	@columnConfig({hideViewLink: true}) InventoryItem__BaseUnit: PXFieldState;
	PhysicalQty: PXFieldState;
	@columnConfig({allowUpdate: false, allowShowHide: GridColumnShowHideMode.Server})	VarQty: PXFieldState;
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
