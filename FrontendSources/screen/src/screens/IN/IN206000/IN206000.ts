import { 
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	PXView,
	PXFieldState,
	PXFieldOptions 
} from 'client-controls';

@graphInfo({graphType: 'PX.Objects.IN.INPostClassMaint', primaryView: 'postclass'})
export class IN206000 extends PXScreen {

	@viewInfo({containerName: 'Posting Class Summary'})
	postclass = createSingle(INPostClass);
	@viewInfo({containerName: 'Posting Class Accounts'})
	postclassaccounts = createSingle(INPostClassAccounts);
}

export class INPostClass extends PXView {
	PostClassID: PXFieldState;
	Descr: PXFieldState;
}

export class INPostClassAccounts extends PXView {
	InvtAcctDefault: PXFieldState<PXFieldOptions.CommitChanges>;
	InvtSubMask: PXFieldState;
	SalesAcctDefault: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesSubMask: PXFieldState<PXFieldOptions.CommitChanges>;
	COGSAcctDefault: PXFieldState<PXFieldOptions.CommitChanges>;
	COGSSubFromSales: PXFieldState<PXFieldOptions.CommitChanges>;
	COGSSubMask: PXFieldState;
	StdCstVarAcctDefault: PXFieldState<PXFieldOptions.CommitChanges>;
	StdCstVarSubMask: PXFieldState;
	StdCstRevAcctDefault: PXFieldState<PXFieldOptions.CommitChanges>;
	StdCstRevSubMask: PXFieldState;
	POAccrualAcctDefault: PXFieldState<PXFieldOptions.CommitChanges>;
	POAccrualSubMask: PXFieldState;
	PPVAcctDefault: PXFieldState<PXFieldOptions.CommitChanges>;
	PPVSubMask: PXFieldState;
	LCVarianceAcctDefault: PXFieldState;
	LCVarianceSubMask: PXFieldState;
	PIReasonCode: PXFieldState;
	AMWIPAccountDefault: PXFieldState<PXFieldOptions.CommitChanges>;
	AMWIPSubMask: PXFieldState;
	AMWIPVarianceAccountDefault: PXFieldState<PXFieldOptions.CommitChanges>;
	AMWIPVarianceSubMask: PXFieldState;
	InvtAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	InvtSubID: PXFieldState;
	ReasonCodeSubID: PXFieldState;
	SalesAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesSubID: PXFieldState;
	COGSAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	COGSSubID: PXFieldState;
	StdCstVarAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	StdCstVarSubID: PXFieldState;
	StdCstRevAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	StdCstRevSubID: PXFieldState;
	POAccrualAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	POAccrualSubID: PXFieldState;
	PPVAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	PPVSubID: PXFieldState;
	LCVarianceAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	LCVarianceSubID: PXFieldState;
	DeferralAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	DeferralSubID: PXFieldState;
	AMWIPAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	AMWIPSubID: PXFieldState;
	AMWIPVarianceAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	AMWIPVarianceSubID: PXFieldState;
	EarningsAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	EarningsSubID: PXFieldState;
	BenefitExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	BenefitExpenseSubID: PXFieldState;
	TaxExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxExpenseSubID: PXFieldState;
	PTOExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	PTOExpenseSubID: PXFieldState;
}