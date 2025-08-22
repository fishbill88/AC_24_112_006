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

export interface CR304000_CreateQuote extends CR304000 {}
export class CR304000_CreateQuote {
	@viewInfo({ containerName: "Create Quote" })
	QuoteInfo = createSingle(CreateQuotesFilter);

	@viewInfo({ containerName: "Create Quote" })
	CreateQuoteInfoUDF = createCollection(QuoteInfoUDF);
}

@gridConfig({
	syncPosition: true,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
	adjustPageSize: true,
	statusField: "MatrixAvailability",
})
export class CreateQuotesFilter extends PXView {
	QuoteType: PXFieldState<PXFieldOptions.CommitChanges>;
	AddProductsFromOpportunity: PXFieldState<PXFieldOptions.CommitChanges>;
	MakeNewQuotePrimary: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalculatePrices: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalculateDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualDocGroupDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	initNewRow: true,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
	fastFilterByAllFields: false,
	autoAdjustColumns: true,
	showTopBar: false,
})
export class QuoteInfoUDF extends PXView {
	@columnConfig({ allowUpdate: false })
	DisplayName: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Value: PXFieldState<PXFieldOptions.CommitChanges>;
}
