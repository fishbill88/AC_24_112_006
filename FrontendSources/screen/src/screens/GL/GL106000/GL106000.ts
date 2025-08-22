import { PXView, createSingle, graphInfo, PXScreen, createCollection, columnConfig, commitChanges, gridConfig } from "client-controls";
import { PXFieldOptions, PXFieldState } from "client-controls/descriptors/fieldstate";

@graphInfo({ graphType: 'PX.Objects.GL.GLTranCodeMaint', primaryView: 'TranCodes' })
export class GL106000 extends PXScreen {

	TranCodes = createCollection(TranCodes);
}

@gridConfig({ adjustPageSize: true, mergeToolbarWith: 'ScreenToolbar', initNewRow: true, allowImport: true })
export class TranCodes extends PXView {
	Module: PXFieldState<PXFieldOptions.CommitChanges>;
	TranType: PXFieldState<PXFieldOptions.CommitChanges>;
	TranCode: PXFieldState;
	Descr: PXFieldState;
	Active: PXFieldState;
}
