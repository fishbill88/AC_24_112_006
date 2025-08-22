import { createCollection,createSingle,PXScreen,graphInfo,viewInfo,handleEvent,CustomEventType,RowSelectedHandlerArgs,PXViewCollection,PXPageLoadBehavior, PXView,PXFieldState,gridConfig,headerDescription,ICurrencyInfo,disabled,selectorSettings,PXFieldOptions,linkCommand,columnConfig,GridColumnShowHideMode,GridColumnType,PXActionState } from 'client-controls';

@graphInfo({graphType: 'PX.Objects.CM.RevalueARAccounts', primaryView: 'Filter', })
export class CM505000 extends PXScreen {

	Filter = createSingle(RevalueFilter);
	ARAccountList = createCollection(CuryARHistory);

}

export class RevalueFilter extends PXView  {

	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEffDate: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	TotalRevalued_Label: PXFieldState<PXFieldOptions.Disabled>;
	TotalRevalued: PXFieldState<PXFieldOptions.Disabled>;

}

@gridConfig({mergeToolbarWith: 'ScreenToolbar'})
export class CuryARHistory extends PXView  {

	Selected: PXFieldState;
	BranchID: PXFieldState;
	AccountID: PXFieldState;
	AccountID_Account_Description: PXFieldState;
	SubID: PXFieldState;
	CustomerID: PXFieldState;
	CustomerID_BAccountR_acctName: PXFieldState;
	CuryRateTypeID: PXFieldState;
	CuryRate: PXFieldState;
	CuryFinYtdBalance: PXFieldState;
	FinYtdBalance: PXFieldState;
	FinPrevRevalued: PXFieldState;
	FinYtdRevalued: PXFieldState;
	FinPtdRevalued: PXFieldState;
	LastRevaluedFinPeriodID: PXFieldState;

}
