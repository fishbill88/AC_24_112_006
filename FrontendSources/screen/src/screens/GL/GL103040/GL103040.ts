import {
	PXScreen, createSingle, createCollection, graphInfo, PXView, PXFieldState, PXFieldOptions, columnConfig, gridConfig, TextAlign
} from 'client-controls';

@gridConfig({ allowDelete: false })
export class Group extends PXView {
	GroupName: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.CommitChanges>;
	GroupType: PXFieldState<PXFieldOptions.CommitChanges>;
	Active: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ allowDelete: false, allowUpdate: false, syncPosition: true, quickFilterFields: ['AccountCD', 'Description'] })
export class Account extends PXView {

	@columnConfig({ textAlign: TextAlign.Center, allowCheckAll: true })
	Included: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountCD: PXFieldState;

	Type: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountClassID: PXFieldState;

	Active: PXFieldState;
	Description: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
}

@gridConfig({ allowDelete: false, allowInsert: false, quickFilterFields: ['BranchCD', 'AcctName'] })
export class Branch extends PXView {

	Included: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BranchCD: PXFieldState;

	AcctName: PXFieldState;

	@columnConfig({ hideViewLink: true })
	LedgerID: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.GL.GLBranchAccess', primaryView: 'Group' })
export class GL103040 extends PXScreen {

	Group = createSingle(Group);
	Account = createCollection(Account);
	Branch = createCollection(Branch);
}
