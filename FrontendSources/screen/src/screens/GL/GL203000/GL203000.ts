import {
	createCollection, createSingle, PXScreen, graphInfo, PXActionState,
	PXViewCollection, PXPageLoadBehavior, PXView,
	PXFieldState, gridConfig, disabled, PXFieldOptions, linkCommand, columnConfig
} from "client-controls";

@graphInfo({graphType: "PX.Objects.GL.SubAccountMaint", primaryView: "SubRecords" })
export class GL203000 extends PXScreen {

	SubRecords = createCollection(Sub);
}

@gridConfig({ syncPosition: true, mergeToolbarWith: "ScreenToolbar", quickFilterFields: ["SubCD", "Description"] })
export class Sub extends PXView {

	SubCD: PXFieldState;
	Description: PXFieldState;
	Active: PXFieldState;
	Secured: PXFieldState;
	SubID: PXFieldState;
}
