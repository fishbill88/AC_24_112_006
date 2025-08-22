import { createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, PXFieldOptions, columnConfig, gridConfig } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CM.RevalueAPAccounts', primaryView: 'Filter', })
export class CM504000 extends PXScreen {

	Filter = createSingle(RevalueFilter);
	APAccountList = createCollection(CuryAPHistory);

}

export class RevalueFilter extends PXView {

	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEffDate: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	TotalRevalued_Label: PXFieldState<PXFieldOptions.Disabled>;
	TotalRevalued: PXFieldState<PXFieldOptions.Disabled>;

}

@gridConfig({ mergeToolbarWith: 'ScreenToolbar' })
export class CuryAPHistory extends PXView {

	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	BranchID: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	AccountID: PXFieldState;

	AccountID_Account_Description: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	SubID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	VendorID: PXFieldState;

	VendorID_Vendor_AcctName: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	CuryRateTypeID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CuryRate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CuryFinYtdBalance: PXFieldState;

	@columnConfig({ allowUpdate: false })
	FinYtdBalance: PXFieldState;

	@columnConfig({ allowUpdate: false })
	FinPrevRevalued: PXFieldState;

	@columnConfig({ allowUpdate: false })
	FinYtdRevalued: PXFieldState;

	@columnConfig({ allowUpdate: false })
	FinPtdRevalued: PXFieldState;

	LastRevaluedFinPeriodID: PXFieldState;
}
