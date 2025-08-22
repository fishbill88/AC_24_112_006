import {
	createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, TextAlign, PXActionState
} from "client-controls";

@graphInfo({ graphType: "PX.Objects.AR.ARSPCommissionReview", primaryView: "Filter", pageLoadBehavior: PXPageLoadBehavior.GoLastRecord })
export class AR506500 extends PXScreen {

	EditDetail: PXActionState;

	@viewInfo({ containerName: "Selection" })
	Filter = createSingle(ARSPCommissionPeriod);

	@viewInfo({ containerName: "Salespersons' Commissions" })
	ToProcess = createCollection(ARSPCommnHistory);

}


export class ARSPCommissionPeriod extends PXView {

	CommnPeriodID: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	StartDateUI: PXFieldState<PXFieldOptions.Disabled>;
	EndDateUI: PXFieldState<PXFieldOptions.Disabled>;

}

@gridConfig({ syncPosition: true, mergeToolbarWith: "ScreenToolbar" })
export class ARSPCommnHistory extends PXView {

	@linkCommand("EditDetail")
	SalesPersonID: PXFieldState;

	Type: PXFieldState;
	SalesPersonID_SalesPerson_descr: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	CommnblAmt: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	CommnAmt: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	BaseCuryID: PXFieldState;

	PRProcessedDate: PXFieldState;

}
