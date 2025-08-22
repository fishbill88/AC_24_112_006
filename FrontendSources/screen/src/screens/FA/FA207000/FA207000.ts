import { createCollection, PXScreen, graphInfo, PXView, PXFieldState, gridConfig, columnConfig } from "client-controls";

@graphInfo({ graphType: "PX.Objects.FA.DisposalMethodMaint", primaryView: "DisposalMethods", })
export class FA207000 extends PXScreen {

	DisposalMethods = createCollection(FADisposalMethod);

}

@gridConfig({ mergeToolbarWith: "ScreenToolbar" })
export class FADisposalMethod extends PXView {

	DisposalMethodCD: PXFieldState;
	Description: PXFieldState;
	@columnConfig({ hideViewLink: true }) ProceedsAcctID: PXFieldState;
	@columnConfig({ hideViewLink: true }) ProceedsSubID: PXFieldState;
	DisposalMethodID: PXFieldState;

}
