import { PXView, createSingle, graphInfo, PXScreen, createCollection, gridConfig, columnConfig } from "client-controls";
import { PXFieldOptions, PXFieldState } from "client-controls/descriptors/fieldstate";
import { autoinject } from 'aurelia-framework';

@graphInfo({ graphType: 'PX.Objects.GL.GLHistoryValidate', primaryView: 'Filter' })
@autoinject
export class GL509900 extends PXScreen {

	Filter = createSingle(GLIntegrityCheckFilter);

	LedgerList = createCollection(Ledger);
}

@gridConfig({ adjustPageSize: true, mergeToolbarWith: 'ScreenToolbar', suppressNoteFiles: true })
export class Ledger extends PXView {

	@columnConfig({ allowCheckAll: true, allowNull: false })
	Selected: PXFieldState;
	LedgerCD: PXFieldState;
	Descr: PXFieldState;
}

export class GLIntegrityCheckFilter extends PXView {
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
}
