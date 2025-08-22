import {
	gridConfig,
	PXFieldState,
	PXView
} from "client-controls";

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	wrapToolbar: true,
	adjustPageSize: true
})
export class RateTables extends PXView {
	RateTableID: PXFieldState;
	Description: PXFieldState;
}
