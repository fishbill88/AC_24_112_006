import {
	PXView,
	PXFieldState,
	commitChanges,
	graphInfo,
	PXScreen,
	createCollection,
	gridConfig,
	GridPreset
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CR.CRActivitySetupMaint",
	primaryView: "ActivityTypes",
})
export class CR102000 extends PXScreen {
	ActivityTypes = createCollection(ActivityTypes);
}

@gridConfig({
	preset: GridPreset.Primary,
	suppressNoteFiles: true,
	fastFilterByAllFields: false,
})
export class ActivityTypes extends PXView {
	ClassID: PXFieldState;
	Type: PXFieldState;
	Description: PXFieldState;
	Active: PXFieldState;
	IsDefault: PXFieldState;
	@commitChanges Application: PXFieldState;
	ImageUrl: PXFieldState;
	@commitChanges PrivateByDefault: PXFieldState;
	@commitChanges RequireTimeByDefault: PXFieldState;
	Incoming: PXFieldState;
	Outgoing: PXFieldState;
}