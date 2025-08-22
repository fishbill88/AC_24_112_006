import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, PXFieldOptions, createCollection, columnConfig, gridConfig
} from 'client-controls';


@graphInfo({ graphType: 'PX.Objects.GL.Consolidation.ConsolBranchMaint', primaryView: 'BranchRecords' })
export class GL103002 extends PXScreen {

	BranchRecords = createCollection(Branch);
}

@gridConfig({ mergeToolbarWith: 'ScreenToolbar', syncPosition: true, adjustPageSize: true })
export class Branch extends PXView {

	@columnConfig({ hideViewLink: true })
	BranchCD: PXFieldState;

	Organization__OrganizationCD: PXFieldState;
	AcctName: PXFieldState;

	@columnConfig({ hideViewLink: true })
	Ledger__LedgerCD: PXFieldState;
}
