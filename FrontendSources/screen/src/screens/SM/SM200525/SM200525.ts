import {
	createCollection,
	PXScreen,
	graphInfo,
	PXView,
	PXFieldState,
	linkCommand,
	gridConfig,
	PXActionState,
	GridPreset,
} from "client-controls";

@graphInfo({ graphType: "PX.SM.KBFeedbackExplore", primaryView: "Responses" })
export class SM200525 extends PXScreen {
	Feedback: PXActionState;

	Responses = createCollection(KBFeedback);
}

@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowDelete: false,
	mergeToolbarWith: "ScreenToolbar",
})
export class KBFeedback extends PXView {
	@linkCommand("Feedback")
	FeedbackID: PXFieldState;
	Users__Username: PXFieldState;
	Date: PXFieldState;
	IsFind: PXFieldState;
	Satisfaction: PXFieldState;
	Summary: PXFieldState;
}
