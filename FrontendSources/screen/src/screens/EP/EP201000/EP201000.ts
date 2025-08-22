import {
	PXScreen,
	graphInfo,
	PXView,
	createCollection,
	PXFieldState,
	gridConfig,
	GridFilterBarVisibility,
	GridPreset,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.EP.PositionMaint",
	primaryView: "EPPosition",
})
export class EP201000 extends PXScreen {
	EPPosition = createCollection(EPPosition);
}

@gridConfig({
	preset: GridPreset.Primary,
	initNewRow: true,
	showFilterBar: GridFilterBarVisibility.False,
})
export class EPPosition extends PXView {
	PositionID: PXFieldState;
	SDEnabled: PXFieldState;
	Description: PXFieldState;
}
