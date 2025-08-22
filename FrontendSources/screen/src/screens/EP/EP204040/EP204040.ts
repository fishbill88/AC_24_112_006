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
	graphType: "PX.Objects.EP.EventTaskCategoryMaint",
	primaryView: "Categories",
})
export class EP204040 extends PXScreen {
	Categories = createCollection(Categories);
}

@gridConfig({
	preset: GridPreset.Primary,
	initNewRow: true,
	showFilterBar: GridFilterBarVisibility.False,
})
export class Categories extends PXView {
	Description: PXFieldState;
	Style: PXFieldState;
}
