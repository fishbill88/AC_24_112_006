import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, PXFieldOptions, createCollection, columnConfig, gridConfig
} from 'client-controls';


@graphInfo({ graphType: 'PX.Objects.GL.Consolidation.ConsolAccountMaint', primaryView: 'AccountRecords' })
export class GL103001 extends PXScreen {

	AccountRecords = createCollection(GLConsolAccount);
}

@gridConfig({ mergeToolbarWith: 'ScreenToolbar', syncPosition: true })
export class GLConsolAccount extends PXView {

	AccountCD: PXFieldState;
	Description: PXFieldState;
}
