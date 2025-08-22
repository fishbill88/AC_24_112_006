import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, PXFieldOptions, createCollection, columnConfig, gridConfig
} from 'client-controls';


@graphInfo({ graphType: 'PX.Objects.GL.Consolidation.ConsolOrganizationMaint', primaryView: 'OrganizationRecords' })
export class GL103004 extends PXScreen {

	OrganizationRecords = createCollection(Organization);
}

@gridConfig({ mergeToolbarWith: 'ScreenToolbar', syncPosition: true, adjustPageSize: true })
export class Organization extends PXView {

	@columnConfig({ hideViewLink: true })
	OrganizationCD: PXFieldState;
	OrganizationName: PXFieldState;

	@columnConfig({ hideViewLink: true })
	Ledger__LedgerCD: PXFieldState;
}
