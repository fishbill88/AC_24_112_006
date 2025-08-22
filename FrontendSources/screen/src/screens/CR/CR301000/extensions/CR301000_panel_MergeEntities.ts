import { CR301000 } from "../CR301000";
import {
	createSingle,
	createCollection,
	PXFieldState,
	PXView,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	viewInfo,
	GridFilterBarVisibility,
	GridPagerMode,
} from "client-controls";

export interface CR301000_panel_MergeEntities extends CR301000 {}
export class CR301000_panel_MergeEntities {
	@viewInfo({ containerName: "Merge Conflicts" })
	Merge_Filter = createSingle(MergeEntitiesFilter);

	@viewInfo({ containerName: "Merge Conflicts" })
	Merge_VisibleComparisonRows = createCollection(MergeComparisonRow);
}

export class MergeEntitiesFilter extends PXView {
	TargetRecord: PXFieldState<PXFieldOptions.CommitChanges>;
	Caption: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	allowInsert: false,
	allowDelete: false,
	allowUpdate: false,
	showFilterBar: GridFilterBarVisibility.False,
	fastFilterByAllFields: false,
	pagerMode: GridPagerMode.InfiniteScroll,
	autoAdjustColumns: true,
	showTopBar: false,
})
export class MergeComparisonRow extends PXView {
	@columnConfig({ allowUpdate: false, width: 200 })
	FieldDisplayName: PXFieldState;
	@columnConfig({ allowCheckAll: true })
	LeftValueSelected: PXFieldState;
	@columnConfig({ allowUpdate: false, width: 203 })
	LeftValue: PXFieldState;
	@columnConfig({ allowCheckAll: true })
	RightValueSelected: PXFieldState;
	@columnConfig({ allowUpdate: false, width: 203 })
	RightValue: PXFieldState;
}
