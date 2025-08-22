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

export interface CR304500_CopyQuote extends CR304500 {}
export class CR304500_CopyQuote {
	@viewInfo({ containerName: "Services Settings" })
	CopyQuoteInfo = createSingle(CopyQuoteFilter);
}

export class CopyQuoteFilter extends PXView {
	OpportunityId: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalculatePrices: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalculateDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualDocGroupDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
}
