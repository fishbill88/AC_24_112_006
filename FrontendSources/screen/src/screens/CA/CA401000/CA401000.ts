import {
	createCollection, createSingle,
	PXScreen, PXView, PXFieldState,
	graphInfo, viewInfo, gridConfig, columnConfig,
	PXPageLoadBehavior, PXFieldOptions
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CA.CashFlowEnq", primaryView: "Filter",
	pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues
})
export class CA401000 extends PXScreen {
   	@viewInfo({containerName: "Selection"})
    Filter = createSingle(CashFlowFilter);

   	@viewInfo({containerName: "Forecast Details"})
	CashFlow = createCollection(CashFlowForecast);
}

export class CashFlowFilter extends PXView {
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeUnassignedDocs: PXFieldState<PXFieldOptions.CommitChanges>;
	AllCashAccounts: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRateTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeUnreleased: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeUnapplied: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeScheduled: PXFieldState<PXFieldOptions.CommitChanges>;
	SummaryOnly: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar"
})
export class CashFlowForecast extends PXView {
	RecordType: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CashAccountID: PXFieldState;

	CashAccountID_CashAccount_Descr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BAccountID: PXFieldState;

	BAccountID_BAccountR_AcctName: PXFieldState;
	EntryID: PXFieldState;
	CuryAmountDay0: PXFieldState;
	CuryAmountDay1: PXFieldState;
	CuryAmountDay2: PXFieldState;
	CuryAmountDay3: PXFieldState;
	CuryAmountDay4: PXFieldState;
	CuryAmountDay5: PXFieldState;
	CuryAmountDay6: PXFieldState;
	CuryAmountDay7: PXFieldState;
	CuryAmountDay8: PXFieldState;
	CuryAmountDay9: PXFieldState;
	CuryAmountDay10: PXFieldState;
	CuryAmountDay11: PXFieldState;
	CuryAmountDay12: PXFieldState;
	CuryAmountDay13: PXFieldState;
	CuryAmountDay14: PXFieldState;
	CuryAmountDay15: PXFieldState;
	CuryAmountDay16: PXFieldState;
	CuryAmountDay17: PXFieldState;
	CuryAmountDay18: PXFieldState;
	CuryAmountDay19: PXFieldState;
	CuryAmountDay20: PXFieldState;
	CuryAmountDay21: PXFieldState;
	CuryAmountDay22: PXFieldState;
	CuryAmountDay23: PXFieldState;
	CuryAmountDay24: PXFieldState;
	CuryAmountDay25: PXFieldState;
	CuryAmountDay26: PXFieldState;
	CuryAmountDay27: PXFieldState;
	CuryAmountDay28: PXFieldState;
	CuryAmountDay29: PXFieldState;
	CuryAmountSummary: PXFieldState;
}
