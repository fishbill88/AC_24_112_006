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
	TextAlign,
	GridColumnType,
	GridPreset
} from "client-controls";

export interface CR304000_Discounts extends CR304000 {}
export class CR304000_Discounts {
	@viewInfo({ containerName: "Discounts" })
	DiscountDetails = createCollection(CROpportunityDiscountDetail);
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true
})
export class CROpportunityDiscountDetail extends PXView {
	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	SkipDiscount: PXFieldState;
	@columnConfig({ allowFastFilter: false })
	DiscountID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowFastFilter: false })
	DiscountSequenceID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowFastFilter: false })
	Type: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	IsManual: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Right })
	CuryDiscountableAmt: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Right })
	DiscountableQty: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Right })
	CuryDiscountAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ textAlign: TextAlign.Right })
	DiscountPct: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowFastFilter: false })
	FreeItemID: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Right })
	FreeItemQty: PXFieldState;
	@columnConfig({ allowFastFilter: false })
	ExtDiscCode: PXFieldState;
	@columnConfig({ allowFastFilter: false })
	Description: PXFieldState;
}
