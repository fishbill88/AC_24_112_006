import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, PXActionState } from "client-controls";

@graphInfo({ graphType: "PX.Objects.CA.CCBatchMaint", primaryView: "BatchView", })
export class CA307000 extends PXScreen {

	@viewInfo({ containerName: "Settlement Batches" })
	BatchView = createSingle(CCBatch);

	@viewInfo({ containerName: "All Transactions" })
	Transactions = createCollection(CCBatchTransaction);

	@viewInfo({ containerName: "Missing Transactions" })
	MissingTransactions = createCollection(CCBatchTransaction2); //duplicated class causes by separated grids

	@viewInfo({ containerName: "Transactions Excluded from Deposit" })
	ExcludedFromDepositTransactions = createCollection(CCBatchTransaction3); //duplicated class causes by separated grids

	@viewInfo({ containerName: "Card Type Summary" })
	CardTypeSummary = createCollection(CCBatchStatistics);
}

export class CCBatch extends PXView {

	BatchID: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	ProcessingCenterID: PXFieldState<PXFieldOptions.Disabled>;
	ExtBatchID: PXFieldState<PXFieldOptions.Disabled>;
	SettlementTime: PXFieldState<PXFieldOptions.Disabled>;
	SettlementState: PXFieldState<PXFieldOptions.Disabled>;
	DepositNbr: PXFieldState<PXFieldOptions.Disabled>;
	SettledAmount: PXFieldState<PXFieldOptions.Disabled>;
	SettledCount: PXFieldState<PXFieldOptions.Disabled>;
	RefundAmount: PXFieldState<PXFieldOptions.Disabled>;
	RefundCount: PXFieldState<PXFieldOptions.Disabled>;
	VoidCount: PXFieldState<PXFieldOptions.Disabled>;
	DeclineCount: PXFieldState<PXFieldOptions.Disabled>;
	TransactionCount: PXFieldState<PXFieldOptions.Disabled>;
	ImportedTransactionCount: PXFieldState<PXFieldOptions.Disabled>;
	ProcessedCount: PXFieldState<PXFieldOptions.Disabled>;
	MissingCount: PXFieldState<PXFieldOptions.Disabled>;
	HiddenCount: PXFieldState<PXFieldOptions.Disabled>;
	ExcludedCount: PXFieldState<PXFieldOptions.Disabled>;

}

@gridConfig({ syncPosition: true, allowDelete: false, allowInsert: false, })
export class CCBatchTransaction extends PXView {

	Unhide: PXActionState;
	SelectedToUnhide: PXFieldState;
	PCTranNumber: PXFieldState;
	SettlementStatus: PXFieldState;
	ProcessingStatus: PXFieldState;
	Amount: PXFieldState;
	DocType: PXFieldState;
	RefNbr: PXFieldState;
	ARPayment__Status: PXFieldState;
	AccountNumber: PXFieldState;
	InvoiceNbr: PXFieldState;
	SubmitTime: PXFieldState;
	DisplayCardType: PXFieldState;
	FixedFee: PXFieldState;
	PercentageFee: PXFieldState;
	TotalFee: PXFieldState;
	FeeType: PXFieldState;

}

@gridConfig({ syncPosition: true, allowDelete: false, allowInsert: false })
export class CCBatchTransaction2 extends PXView {
	Record: PXActionState;
	Hide: PXActionState;
	RepeatMatching: PXActionState;
	SelectedToHide: PXFieldState;
	PCTranNumber: PXFieldState;
	SettlementStatus: PXFieldState;
	Amount: PXFieldState;
	AccountNumber: PXFieldState;
	InvoiceNbr: PXFieldState;
	SubmitTime: PXFieldState;
	DisplayCardType: PXFieldState;
}

@gridConfig({ syncPosition: true, allowDelete: false, allowInsert: false })
export class CCBatchTransaction3 extends PXView {

	PCTranNumber: PXFieldState;
	SettlementStatus: PXFieldState;
	SubmitTime: PXFieldState;
	Amount: PXFieldState;
	AccountNumber: PXFieldState;
	InvoiceNbr: PXFieldState;
	DocType: PXFieldState;
	RefNbr: PXFieldState;
	ARPayment__CashAccountID: PXFieldState;
	ARPayment__DepositNbr: PXFieldState;

}

@gridConfig({ allowDelete: false, allowInsert: false })
export class CCBatchStatistics extends PXView {

	DisplayCardType: PXFieldState;
	SettledAmount: PXFieldState;
	SettledCount: PXFieldState;
	RefundAmount: PXFieldState;
	RefundCount: PXFieldState;
	VoidCount: PXFieldState;
	DeclineCount: PXFieldState;
	ErrorCount: PXFieldState;

}
