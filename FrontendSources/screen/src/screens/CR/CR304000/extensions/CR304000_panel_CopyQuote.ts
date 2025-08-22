import { CR304000 } from "../CR304000";
import {
	PXView,
	PXFieldState,
	linkCommand,
	createCollection,
	PXFieldOptions,
	gridConfig,
	viewInfo,
	createSingle,
	columnConfig,
} from "client-controls";

export interface CR304000_CopyQuote extends CR304000 {}
export class CR304000_CopyQuote {
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
