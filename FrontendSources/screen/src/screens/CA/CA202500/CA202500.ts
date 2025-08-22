import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, PXFieldOptions, PXActionState, createCollection, columnConfig, linkCommand, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CA.CACorpCardsMaint', primaryView: 'CreditCards' })
export class CA202500 extends PXScreen {
	CreditCards = createSingle(CACorpCard);
	EmployeeLinks = createCollection(EPEmployeeCorpCardLink);
}

export class CACorpCard extends PXView {
	CorpCardCD: PXFieldState;
	BranchID: PXFieldState;
	Name: PXFieldState;
	CardNumber: PXFieldState;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsActive: PXFieldState;
}

export class EPEmployeeCorpCardLink extends PXView {
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeID_EPEmployee_acctName: PXFieldState;
}
