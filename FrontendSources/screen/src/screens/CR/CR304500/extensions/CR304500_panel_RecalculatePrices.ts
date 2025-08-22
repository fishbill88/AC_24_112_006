import { CR304500 } from "../CR304500";
import {
	PXView,
	PXFieldState,
	linkCommand,
	createCollection,
	PXFieldOptions,
	gridConfig,
	viewInfo,
	createSingle,
} from "client-controls";

export interface CR304500_RecalculatePrices extends CR304500 {}
export class CR304500_RecalculatePrices {
	@viewInfo({ containerName: "Services Settings" })
	recalcdiscountsfilter = createSingle(RecalcDiscountsParamFilter);
}

export class RecalcDiscountsParamFilter extends PXView {
	RecalcTarget: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalcUnitPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalcDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualDocGroupDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
}
