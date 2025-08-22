import {
	PXScreen,
	graphInfo,
	PXView,
	createCollection,
	PXFieldState,
	gridConfig,
	linkCommand,
	PXActionState,
	GridPreset,
	columnConfig,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.EP.EPAssignmentAndApprovalMapEnq",
	primaryView: "Maps",
})
export class EP205500 extends PXScreen {
	Maps = createCollection(Maps);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
})
export class Maps extends PXView {
	@linkCommand("ViewDetails") Name: PXFieldState;
	@columnConfig({ width: 230 }) MapType: PXFieldState;
	EntityType: PXFieldState;
}
