import {
	columnConfig,
	gridConfig,
	PXFieldState,
	PXView
} from "client-controls";

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	wrapToolbar: true,
	adjustPageSize: true
})
export class RateTypes extends PXView {
	@columnConfig({ width: 150 })
	RateTypeID: PXFieldState;
	@columnConfig({ width: 400 })
	Description: PXFieldState;
}
