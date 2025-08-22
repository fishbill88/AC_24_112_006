import {
	PXView,
	PXFieldState,
	headerDescription,
	PXFieldOptions,
	graphInfo,
	PXScreen,
	viewInfo,
	createSingle,
	createCollection,
	PXActionState,
	gridConfig,
	columnConfig,
	GridColumnShowHideMode,
	GridPreset
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.IN.KitAssemblyEntry', primaryView: 'Document', udfTypeField: "DocType", bpEventsIndicator: true, showUDFIndicator: true })
export class IN307000 extends PXScreen {
    INComponentLineSplittingExtension_GenerateNumbers: PXActionState; //Generate button on Line Details panel of Components tab
    INKitLineSplittingExtension_GenerateNumbers: PXActionState; //Generate button on Kit Details

	@viewInfo({containerName: "Kit Assembly Summary"})
	Document = createSingle(INKitRegisterHeader);

	@viewInfo({containerName: "Kit Assembly Properties"})
	DocumentProperties = createSingle(INKitRegister);

	@viewInfo({containerName: "Components"})
	Components = createCollection(INComponentTran);

	@viewInfo({containerName: "Component Details Header"})
	INComponentLineSplittingExtension_LotSerOptions = createSingle(LineSplittingHeader);

    @viewInfo({containerName: "Component Splits"})
    ComponentSplits = createCollection(INComponentTranSplit);

    @viewInfo({containerName: "Non-Stock Components"})
    Overhead = createCollection(INOverheadTran);

	@viewInfo({containerName: "Kit Details Header"})
	INKitLineSplittingExtension_LotSerOptions = createSingle(LineSplittingHeader);

    @viewInfo({containerName: "Kit Line Details"})
    MasterSplits = createCollection(INKitTranSplit);
}

export class INKitRegisterHeader extends PXView {
	DocType: PXFieldState;
	RefNbr: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	TranDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState;

	KitInventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	KitRevisionID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	ReasonCode: PXFieldState;

	@headerDescription
	TranTranDesc: PXFieldState;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState;
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class INKitRegister extends PXView {
    BatchNbr: PXFieldState<PXFieldOptions.Disabled>;
    BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
    BranchBaseCuryID: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
	statusField: "Availability",
})
export class INComponentTran extends PXView {
	INComponentLineSplittingExtension_ShowSplits: PXActionState; //Line Details button on Components tab

	@columnConfig({
		visible: false,
		allowShowHide: GridColumnShowHideMode.False
	})
	Availability: PXFieldState;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true,
		nullText: "<SPLIT>"
	})
    SubItemID: PXFieldState;
	@columnConfig({
		hideViewLink: true
	})
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true
	})
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		allowNull: false
	})
	Qty: PXFieldState;
	@columnConfig({
		allowNull: false
	})
	UnitCost: PXFieldState;
	ReasonCode: PXFieldState;
	TranDesc: PXFieldState;
	INKitSpecStkDet__DfltCompQty: PXFieldState;
	@columnConfig({
		hideViewLink: true
	})
    INKitSpecStkDet__UOM: PXFieldState;
	INKitSpecStkDet__AllowQtyVariation: PXFieldState;
	INKitSpecStkDet__MinCompQty: PXFieldState;
	INKitSpecStkDet__MaxCompQty: PXFieldState;
	INKitSpecStkDet__DisassemblyCoeff: PXFieldState;
	INKitSpecStkDet__AllowSubstitution: PXFieldState;
}

export class LineSplittingHeader extends PXView {
	UnassignedQty: PXFieldState;
	Qty: PXFieldState;
	StartNumVal: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true
})
export class INComponentTranSplit extends PXView {
	@columnConfig({hideViewLink: true})
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
    @columnConfig({allowNull: false})
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	UOM: PXFieldState;
	ExpireDate: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true
})
export class INOverheadTran extends PXView {
    InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
    @columnConfig({hideViewLink: true})
    UOM: PXFieldState;
    @columnConfig({allowNull: false})
    Qty: PXFieldState;
    @columnConfig({allowNull: false})
    UnitCost: PXFieldState;
    ReasonCode: PXFieldState;
    TranDesc: PXFieldState;
    INKitSpecNonStkDet__DfltCompQty: PXFieldState;
    @columnConfig({hideViewLink: true})
    INKitSpecNonStkDet__UOM: PXFieldState;
    INKitSpecNonStkDet__AllowQtyVariation: PXFieldState;
    INKitSpecNonStkDet__MinCompQty: PXFieldState;
    INKitSpecNonStkDet__MaxCompQty: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true
})
export class INKitTranSplit extends PXView {
	@columnConfig({
		hideViewLink: true,
		nullText: "<SPLIT>"
	})
	SubItemID: PXFieldState;
	@columnConfig({
		hideViewLink: true,
		nullText: "<SPLIT>"
	})
	LocationID: PXFieldState;
    @columnConfig({hideViewLink: true})
	LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true,
		allowUpdate: false
	})
	UOM: PXFieldState;
	@columnConfig({allowNull: false})
	Qty: PXFieldState;
	ExpireDate: PXFieldState;
}
