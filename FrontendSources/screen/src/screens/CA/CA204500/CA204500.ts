import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, PXFieldOptions
} from 'client-controls';

@graphInfo({graphType: 'PX.Objects.CA.CABankTranRuleMaint', primaryView: 'Rule'})
export class CA204500 extends PXScreen {

	Rule = createSingle(CABankTranRule);

}

export class CABankTranRule extends PXView {
	RuleID: PXFieldState<PXFieldOptions.Hidden>;
	Description: PXFieldState;
	IsActive: PXFieldState;
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
	CuryTranAmt: PXFieldState;
	CuryMinTranAmt: PXFieldState;
	MaxCuryTranAmt: PXFieldState;
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	DocumentEntryTypeID: PXFieldState;
}
