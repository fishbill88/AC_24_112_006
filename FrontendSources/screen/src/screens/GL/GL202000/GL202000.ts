import { PXView, createSingle, graphInfo, PXScreen, createCollection, columnConfig, gridConfig } from "client-controls";
import { PXFieldOptions, PXFieldState } from "client-controls/descriptors/fieldstate";

@graphInfo({ graphType: 'PX.Objects.GL.AccountClassMaint', primaryView: 'AccountClassRecords' })
export class GL202000 extends PXScreen {
	AccountClassRecords = createCollection(AccountClassRecords);
}

@gridConfig({ adjustPageSize: true, mergeToolbarWith: 'ScreenToolbar', allowImport: true })
export class AccountClassRecords extends PXView {
	AccountClassID: PXFieldState;
	Type: PXFieldState;
	Descr: PXFieldState;
}
