import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, PXFieldOptions, columnConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CA.CABankTranRuleMaintPopup', primaryView: 'Rule' })
export class CA204550 extends PXScreen {

	Rule = createSingle(Rule)

}

export class Rule extends PXView {

	Description: PXFieldState;
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;

	BankDrCr: PXFieldState<PXFieldOptions.CommitChanges>;
	BankTranCashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	TranCuryID: PXFieldState;
	TranCode: PXFieldState;

	BankTranDescription: PXFieldState;
	MatchDescriptionCase: PXFieldState;
	UseDescriptionWildcards: PXFieldState;

	PayeeName: PXFieldState;
	UsePayeeNameWildcards: PXFieldState;

	AmountMatchingMode: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowNull: true })
	CuryTranAmt: PXFieldState;

	@columnConfig({ allowNull: true })
	CuryMinTranAmt: PXFieldState;

	@columnConfig({ allowNull: true })
	MaxCuryTranAmt: PXFieldState;

	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	DocumentEntryTypeID: PXFieldState;

}

