import { CR304500 } from "../CR304500";
import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	createCollection,
	gridConfig,
	viewInfo,
	PXActionState,
	columnConfig,
	GridPreset,
} from "client-controls";

export interface CR304500_Discounts extends CR304500 {}
export class CR304500_Discounts {
	@viewInfo({ containerName: "Discounts" })
	DiscountDetails = createCollection(CROpportunityDiscountDetail);
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
	allowDelete: true,
	fastFilterByAllFields: false,
})
export class CROpportunityDiscountDetail extends PXView {
	SkipDiscount: PXFieldState;
	DiscountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscountSequenceID: PXFieldState<PXFieldOptions.CommitChanges>;
	Type: PXFieldState;
	IsManual: PXFieldState;
	CuryDiscountableAmt: PXFieldState;
	DiscountableQty: PXFieldState;
	CuryDiscountAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscountPct: PXFieldState<PXFieldOptions.CommitChanges>;
	FreeItemID: PXFieldState;
	FreeItemQty: PXFieldState;
	ExtDiscCode: PXFieldState;
	Description: PXFieldState;
}
