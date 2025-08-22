import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, PXFieldOptions, PXActionState, createCollection, linkCommand, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CA.CCBatchEnq', primaryView: 'Filter' })
export class CA507000 extends PXScreen {
	Filter = createSingle(BatchFilter);
	Batches = createCollection(CCBatch);

	viewDocument: PXActionState;
}

export class BatchFilter extends PXView {
	ProcessingCenterID: PXFieldState<PXFieldOptions.CommitChanges>;
	LastSettlementDate: PXFieldState<PXFieldOptions.Disabled>;
	ImportThroughDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ syncPosition: true, adjustPageSize: true, mergeToolbarWith: 'ScreenToolbar' })
export class CCBatch extends PXView {
	@linkCommand('viewDocument')
	BatchID: PXFieldState;
	Status: PXFieldState;
	SettlementTime: PXFieldState;
	SettlementState: PXFieldState;
	ExtBatchID: PXFieldState;
	SettledAmount: PXFieldState;
	RefundAmount: PXFieldState;
	TransactionCount: PXFieldState;
	ImportedTransactionCount: PXFieldState;
	UnprocessedCount: PXFieldState;
	MissingCount: PXFieldState;
	DepositNbr: PXFieldState;
}
