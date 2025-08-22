import {
	PXScreen, createSingle, createCollection, graphInfo, PXView, PXFieldState, PXFieldOptions, columnConfig, gridConfig, TextAlign
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.GL.GLAccessByAccount', primaryView: 'Account' })
export class GL104020 extends PXScreen {

	Account = createSingle(Account);
	Groups = createCollection(Groups);
}

@gridConfig({ allowDelete: false, allowInsert: false })
export class Groups extends PXView {

	@columnConfig({ textAlign: TextAlign.Center, allowCheckAll: true })
	Included: PXFieldState;

	@columnConfig({ hideViewLink: true })
	GroupName: PXFieldState;
	Description: PXFieldState;
	GroupType: PXFieldState;
	Active: PXFieldState;
}

@gridConfig({ allowDelete: false })
export class Account extends PXView {

	AccountCD: PXFieldState<PXFieldOptions.CommitChanges>;
	Type: PXFieldState;
	Description: PXFieldState;
	AccountClassID: PXFieldState;
	CuryID: PXFieldState;
}
