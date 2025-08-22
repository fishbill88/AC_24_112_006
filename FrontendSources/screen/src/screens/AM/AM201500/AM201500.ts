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
	linkCommand,
} from 'client-controls';

export class ProdMaint extends PXView {
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	ProdOrdID: PXFieldState;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;

	ProdDate: PXFieldState<PXFieldOptions.CommitChanges>;
	StatusID: PXFieldState<PXFieldOptions.CommitChanges>;
	Hold: PXFieldState<PXFieldOptions.CommitChanges>;
	ProductWorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProductManagerID: PXFieldState;

	Descr: PXFieldState;
}

// General and References tabs
export class ProdItem extends PXView {
	// General tab
	QtytoProd: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState;
	QtyComplete: PXFieldState;
	QtyScrapped: PXFieldState;
	QtyRemaining: PXFieldState;

	ScheduleStatus: PXFieldState;
	SchedulingMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	ConstDate: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState;
	EndDate: PXFieldState;
	FMLTime: PXFieldState;
	FMLTMRPOrdorOP: PXFieldState;
	ExcludeFromMRP: PXFieldState;
	AutoBackwardReporting: PXFieldState<PXFieldOptions.CommitChanges>;
	SchPriority: PXFieldState<PXFieldOptions.CommitChanges>;
	CostMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	ScrapOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	ScrapSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ScrapLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	PreassignLotSerial: PXFieldState<PXFieldOptions.CommitChanges>;
	ParentLotSerialRequired: PXFieldState<PXFieldOptions.CommitChanges>;

	// References tab
	CustomerID: PXFieldState;
	OrdTypeRef: PXFieldState;
	OrdNbr: PXFieldState;
	OrdLineRef: PXFieldState;

	ProductOrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	ProductOrdID: PXFieldState<PXFieldOptions.CommitChanges>;
	ParentOrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	ParentOrdID: PXFieldState<PXFieldOptions.CommitChanges>;

	BranchID: PXFieldState;
	WIPAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	WIPSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	WIPVarianceAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	WIPVarianceSubID: PXFieldState<PXFieldOptions.CommitChanges>;

	DetailSource: PXFieldState<PXFieldOptions.CommitChanges>;
	BOMEffDate: PXFieldState<PXFieldOptions.CommitChanges>;
	BOMID: PXFieldState<PXFieldOptions.CommitChanges>;
	BOMRevisionID: PXFieldState<PXFieldOptions.CommitChanges>;

	EstimateID: PXFieldState<PXFieldOptions.CommitChanges>;
	EstimateRevisionID: PXFieldState<PXFieldOptions.CommitChanges>;
	SourceOrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	SourceProductionNbr: PXFieldState;

	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	UpdateProject: PXFieldState;
}

export class ItemConfiguration extends PXView {
	ConfigurationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Revision: PXFieldState<PXFieldOptions.CommitChanges>;
	KeyID: PXFieldState<PXFieldOptions.CommitChanges>;
}

// Events tab
@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	initNewRow: true,
})
export class ProdEventRecords extends PXView {
	CreatedDateTime: PXFieldState;
	EventType: PXFieldState;
	Description: PXFieldState;
	CreatedByScreenIDTitle: PXFieldState;
	CreatedByScreenID: PXFieldState;
	@columnConfig({ hideViewLink: true }) CreatedByID: PXFieldState;
	RefBatNbr: PXFieldState;
	RefDocType: PXFieldState;
	@linkCommand("AMProdEvnt$RefNoteID$Link") RefNoteID: PXFieldState;
	LineNbr: PXFieldState;
}

// Attributes tab
@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
})
export class ProductionAttributes extends PXView {
	OrderType: PXFieldState;
	ProdOrdID: PXFieldState;
	LineNbr: PXFieldState;
	Level: PXFieldState;
	OperationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Source: PXFieldState;
	AttributeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Label: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	Enabled: PXFieldState;
	TransactionRequired: PXFieldState;
	Value: PXFieldState;
}

// Totals tab
export class ProdTotalRecs extends PXView {
	PlanLaborTime: PXFieldState;
	PlanLaborTimeRaw: PXFieldState<PXFieldOptions.Hidden>;
	PlanLabor: PXFieldState;
	PlanMachine: PXFieldState;
	PlanMaterial: PXFieldState;
	PlanTool: PXFieldState;
	PlanFixedOverhead: PXFieldState;
	PlanVariableOverhead: PXFieldState;
	PlanSubcontract: PXFieldState;
	PlanQtyToProduce: PXFieldState;
	PlanTotal: PXFieldState;
	PlanUnitCost: PXFieldState;
	PlanCostDate: PXFieldState;
	PlanReferenceMaterial: PXFieldState;

	ActualLaborTime: PXFieldState;
	ActualLaborTimeRaw: PXFieldState<PXFieldOptions.Hidden>;
	ActualLabor: PXFieldState;
	ActualMachine: PXFieldState;
	ActualMaterial: PXFieldState;
	ActualTool: PXFieldState;
	ActualFixedOverhead: PXFieldState;
	ActualVariableOverhead: PXFieldState;
	ActualSubcontract: PXFieldState;
	QtyComplete: PXFieldState;
	WIPAdjustment: PXFieldState;
	ScrapAmount: PXFieldState;
	WIPTotal: PXFieldState;
	WIPComp: PXFieldState;

	VarianceLaborTime: PXFieldState;
	VarianceLaborTimeRaw: PXFieldState<PXFieldOptions.Hidden>;
	VarianceLabor: PXFieldState;
	VarianceMachine: PXFieldState;
	VarianceMaterial: PXFieldState;
	VarianceTool: PXFieldState;
	VarianceFixedOverhead: PXFieldState;
	VarianceVariableOverhead: PXFieldState;
	VarianceSubcontract: PXFieldState;
	QtyRemaining: PXFieldState;
	VarianceTotal: PXFieldState;
	WIPBalance: PXFieldState;
}

// Line Details tab
export class ItemLineSplittingExtension_LotSerOptions extends PXView {
	UnassignedQty: PXFieldState;
	Qty: PXFieldState;
	StartNumVal: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	initNewRow: true,
})
export class Splits extends PXView {
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LocationID: PXFieldState;
	LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	Qty: PXFieldState;
	QtyComplete: PXFieldState;
	QtyScrapped: PXFieldState;
	QtyRemaining: PXFieldState;
	ExpireDate: PXFieldState;
}

// Smart panel SO Line Details
export class LinkSalesLinesFilter extends PXView {
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class LinkSOLineRecord extends PXView {
	@columnConfig({ allowCheckAll: true }) AMSelected: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderType: PXFieldState;
	OrderNbr: PXFieldState;
	OrderQty: PXFieldState;
	OpenQty: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	LineNbr: PXFieldState;
}


@graphInfo({ graphType: 'PX.Objects.AM.ProdMaint', primaryView: 'ProdMaintRecords' })
export class AM201500 extends PXScreen {
	linkSalesOrder: PXActionState;
	ConfigureEntry: PXActionState;
	Reconfigure: PXActionState;
	ItemLineSplittingExtension_GenerateNumbers: PXActionState;

	ProdMaintRecords = createSingle(ProdMaint);
	ProdItemSelected = createSingle(ProdItem);
	ItemConfiguration = createSingle(ItemConfiguration);
	ProdEventRecords = createCollection(ProdEventRecords);
	ProductionAttributes = createCollection(ProductionAttributes);
	ProdTotalRecs = createSingle(ProdTotalRecs);
	ItemLineSplittingExtension_LotSerOptions = createSingle(ItemLineSplittingExtension_LotSerOptions);
	splits = createCollection(Splits);
	linkSalesLinesFilter = createSingle(LinkSalesLinesFilter);
	LinkSOLineRecords = createCollection(LinkSOLineRecord);
}
