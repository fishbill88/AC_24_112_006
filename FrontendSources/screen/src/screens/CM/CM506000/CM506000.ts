import { createCollection,createSingle,PXScreen,graphInfo,PXView,PXFieldState,PXFieldOptions, gridConfig } from 'client-controls';

@graphInfo({graphType: 'PX.Objects.CM.RevalueGLAccounts', primaryView: 'Filter', })
export class CM506000 extends PXScreen {

	Filter = createSingle(RevalueFilter);
	GLAccountList = createCollection(GLHistory);

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
export class GLHistory extends PXView  {

	Selected: PXFieldState;
	BranchID: PXFieldState;
	AccountID: PXFieldState;
	AccountID_Account_Description: PXFieldState;
	SubID: PXFieldState;
	CuryRateTypeID: PXFieldState;
	CuryRate: PXFieldState;
	CuryFinYtdBalance: PXFieldState;
	FinYtdBalance: PXFieldState;
	FinYtdRevalued: PXFieldState;
	FinPtdRevalued: PXFieldState;
	LastRevaluedFinPeriodID: PXFieldState;

}
