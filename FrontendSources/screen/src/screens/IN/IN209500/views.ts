import { PXView,PXFieldState,gridConfig,headerDescription,ICurrencyInfo,disabled,selectorSettings,PXFieldOptions,linkCommand,columnConfig,GridColumnShowHideMode,GridColumnType,PXActionState } from 'client-controls';

// Views

export class INKitSpecHdr extends PXView {
	KitInventoryID: PXFieldState;
	IsNonStock: PXFieldState<PXFieldOptions.Disabled>;
	RevisionID: PXFieldState;
	Descr: PXFieldState;
	KitSubItemID: PXFieldState;
	IsActive: PXFieldState;
	AllowCompAddition: PXFieldState;
}

@gridConfig({syncPosition: true,adjustPageSize: true})
export class INKitSpecStkDet extends PXView {
	CompInventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowUpdate: false})
	CompInventoryID_InventoryItem_Descr: PXFieldState;
	CompSubItemID: PXFieldState;
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	DfltCompQty: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowQtyVariation: PXFieldState;
	@columnConfig({allowNull: true})
	MinCompQty: PXFieldState;
	@columnConfig({allowNull: true})
	MaxCompQty: PXFieldState;
	@columnConfig({allowNull: true})
	DisassemblyCoeff: PXFieldState;
	AllowSubstitution: PXFieldState;
}

@gridConfig({syncPosition: true,adjustPageSize: true})
export class INKitSpecNonStkDet extends PXView {
	CompInventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowUpdate: false})
	CompInventoryID_InventoryItem_Descr: PXFieldState;
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	DfltCompQty: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowQtyVariation: PXFieldState;
	@columnConfig({allowNull: true})
	MinCompQty: PXFieldState;
	@columnConfig({allowNull: true})
	MaxCompQty: PXFieldState;
}
