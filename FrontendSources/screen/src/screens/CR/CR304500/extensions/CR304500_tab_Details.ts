import { CR304500 } from "../CR304500";
import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	createCollection,
	gridConfig,
	viewInfo,
	PXActionState,
	GridPreset,
} from "client-controls";

export interface CR304500_Products extends CR304500 {}
export class CR304500_Products {
	@viewInfo({ containerName: "Details" })
	Products = createCollection(CROpportunityProducts);
}

@gridConfig({
	preset: GridPreset.Details,
	statusField: "TextForProductsGrid",
	initNewRow: true,
	allowDelete: true,
})
export class CROpportunityProducts extends PXView {
	ShowMatrixPanel: PXActionState;
	ConfigureEntry: PXActionState;

	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	UOM: PXFieldState;
	VendorID: PXFieldState;
	IsConfigurable: PXFieldState;
	AMParentLineNbr: PXFieldState;
	AMIsSupplemental: PXFieldState;
	Descr: PXFieldState;
	IsFree: PXFieldState;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	Quantity: PXFieldState;
	CuryUnitPrice: PXFieldState;
	CuryExtPrice: PXFieldState;
	DiscPct: PXFieldState;
	CuryDiscAmt: PXFieldState;
	CuryAmount: PXFieldState;
	ManualDisc: PXFieldState<PXFieldOptions.CommitChanges>;
	SkipLineDiscounts: PXFieldState;
	DiscountID: PXFieldState;
	DiscountSequenceID: PXFieldState;
	TaxCategoryID: PXFieldState;
	TaskID: PXFieldState;
	CostCodeID: PXFieldState;
	ManualPrice: PXFieldState;
	POCreate: PXFieldState;
	SortOrder: PXFieldState;
	LineNbr: PXFieldState;
	AMConfigKeyID: PXFieldState<PXFieldOptions.CommitChanges>;
	TextForProductsGrid: PXFieldState;
}
