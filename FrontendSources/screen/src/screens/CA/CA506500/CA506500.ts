import {
	PXScreen, PXView,
	PXFieldState, PXFieldOptions, GridColumnShowHideMode,
	PXActionState,
	createCollection, createSingle, gridConfig, graphInfo, columnConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CA.PaymentReclassifyProcess', primaryView: 'filter' })
export class CA506500 extends PXScreen {
	Filter = createSingle(Filter);
	Adjustments = createCollection(CASplitExt);
}

export class Filter extends PXView {
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	EntryTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeUnreleased: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowReclassified: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	CopyDescriptionfromDetails: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ adjustPageSize: true, syncPosition: true, mergeToolbarWith: 'ScreenToolbar' })
export class CASplitExt extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;

	CashAccountID: PXFieldState;

	ExtRefNbr: PXFieldState;

	TranDate: PXFieldState;

	AdjRefNbr: PXFieldState;

	CuryID: PXFieldState;

	DrCr: PXFieldState;

	FinPeriodID: PXFieldState;

	ReclassCashAccountID: PXFieldState;

	AccountID: PXFieldState;

	SubID: PXFieldState;

	CuryTranAmt: PXFieldState;

	@columnConfig({ allowNull: false, allowUpdate: false })
	TranAmt: PXFieldState;

	@columnConfig({ allowNull: false, allowUpdate: false })
	Cleared: PXFieldState;

	@columnConfig({ allowNull: false })
	OrigModule: PXFieldState;

	@columnConfig({ allowUpdate: false })
	ChildOrigTranType: PXFieldState;

	@columnConfig({ allowUpdate: false })
	ChildOrigRefNbr: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	ReferenceID: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	ReferenceID_BAccountR_AcctName: PXFieldState;

	LocationID: PXFieldState;

	PaymentMethodID: PXFieldState;

	PMInstanceID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	TranDesc: PXFieldState;
}
