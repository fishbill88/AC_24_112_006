import { CR304000 } from "../CR304000";
import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	createCollection,
	gridConfig,
	viewInfo,
	PXActionState,
	columnConfig,
	GridColumnShowHideMode,
	TextAlign,
	GridColumnType,
	GridPreset,
} from "client-controls";

export interface CR304000_Details extends CR304000 {}
export class CR304000_Details {
	@viewInfo({ containerName: "Details" })
	Products = createCollection(CROpportunityProducts);
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
})
export class CROpportunityProducts extends PXView {
	AddNew: PXActionState;
	Copy: PXActionState;
	Paste: PXActionState;
	ShowMatrixPanel: PXActionState;
	ConfigureEntry: PXActionState;

	@columnConfig({ width: 90, textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	IsConfigurable: PXFieldState;
	@columnConfig({ width: 85, textAlign: TextAlign.Center })
	AMParentLineNbr: PXFieldState;
	@columnConfig({ width: 85, textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	AMIsSupplemental: PXFieldState;
	@columnConfig({ format: "CCCCCCCCCCCCCCCCCCCC" })
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	@columnConfig({ allowNull: false, textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	IsFree: PXFieldState;
	BillingRule: PXFieldState;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ textAlign: TextAlign.Right })
	Quantity: PXFieldState;
	EstimatedDuration: PXFieldState;
	@columnConfig({ format: ">aaaaaa" })
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	CuryUnitPrice: PXFieldState;
	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	CuryExtPrice: PXFieldState;
	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	DiscPct: PXFieldState;
	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	CuryDiscAmt: PXFieldState;
	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	CuryAmount: PXFieldState;
	@columnConfig({ allowNull: false, textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	ManualDisc: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	SkipLineDiscounts: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Left })
	DiscountID: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Left })
	DiscountSequenceID: PXFieldState;
	TaxCategoryID: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	ManualPrice: PXFieldState;
	@columnConfig({ format: ">#####" })
	TaskID: PXFieldState;
	CostCodeID: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Right, type: GridColumnType.CheckBox })
	POCreate: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Right })
	CuryUnitCost: PXFieldState;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ textAlign: TextAlign.Right })
	SortOrder: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Right })
	LineNbr: PXFieldState;
	@columnConfig({ width: 90 })
	AMConfigKeyID: PXFieldState<PXFieldOptions.CommitChanges>;
}