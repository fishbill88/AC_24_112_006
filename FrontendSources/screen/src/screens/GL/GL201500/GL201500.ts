import {
	PXScreen, createSingle, createCollection, graphInfo, PXView, PXFieldState, PXFieldOptions, PXActionState, columnConfig, gridConfig
} from 'client-controls';

export class Ledger extends PXView {
	LedgerCD: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState<PXFieldOptions.CommitChanges>;
	BalanceType: PXFieldState<PXFieldOptions.CommitChanges>;
	BaseCuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsolAllowed: PXFieldState;
}

@gridConfig({ allowDelete: false, allowUpdate: false, syncPosition: true })
export class OrganizationLedgerLink extends PXView {

	DeleteOrganizationLedgerLink: PXActionState;

	OrganizationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Organization__OrganizationName: PXFieldState;
	Organization__Active: PXFieldState;
	Organization__OrganizationType: PXFieldState;
}

export class Branch extends PXView {

	@columnConfig({ hideViewLink: true })
	BranchCD: PXFieldState;
	AcctName: PXFieldState;
	Active: PXFieldState;
	Organization__OrganizationName: PXFieldState;
}

export class ChangeIDParam extends PXView {

	CD: PXFieldState<PXFieldOptions.CommitChanges>;

}

@graphInfo({ graphType: 'PX.Objects.GL.GeneralLedgerMaint', primaryView: 'LedgerRecords' })
export class GL201500 extends PXScreen {
	LedgerRecords = createSingle(Ledger);

	OrganizationLedgerLinkWithOrganizationSelect = createCollection(OrganizationLedgerLink);

	BranchesView = createCollection(Branch);
	ChangeIDDialog = createSingle(ChangeIDParam);
}
