import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, PXFieldOptions, createCollection, columnConfig, gridConfig
} from 'client-controls';


@graphInfo({ graphType: 'PX.Objects.GL.Consolidation.ConsolLedgerMaint', primaryView: 'LedgerRecords' })
export class GL103003 extends PXScreen {

	LedgerRecords = createCollection(Ledger);
}

@gridConfig({ mergeToolbarWith: 'ScreenToolbar', syncPosition: true, adjustPageSize: true })
export class Ledger extends PXView {

	@columnConfig({ hideViewLink: true })
	LedgerCD: PXFieldState;
	Descr: PXFieldState;
	BalanceType: PXFieldState;
}
