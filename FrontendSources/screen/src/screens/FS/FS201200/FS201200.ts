import {
	graphInfo,
	gridConfig,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.ProblemMaint', primaryView: 'ProblemRecords' })
export class FS201200 extends PXScreen {
	ProblemRecords = createCollection(FSProblem);
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	allowImport: true
})
export class FSProblem extends PXView {
	ProblemCD: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
}
