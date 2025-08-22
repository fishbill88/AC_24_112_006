import { CR302000 } from "../CR302000";
import {
	createSingle,
	createCollection,
	PXFieldState,
	PXView,
	columnConfig,
	gridConfig,
	PXFieldOptions,
	GridFilterBarVisibility,
	GridPagerMode,
} from "client-controls";

export interface CR302000_panel_SelectContactForLink extends CR302000 {}
export class CR302000_panel_SelectContactForLink {
	Link_Filter = createSingle(EntityForLinkFilter);
	Link_VisibleComparisonRows = createCollection(EntityForLink);
}

export class EntityForLinkFilter extends PXView {
	Caption: PXFieldState;
	ProcessLink: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	allowInsert: false,
	allowDelete: false,
	showFilterBar: GridFilterBarVisibility.False,
	fastFilterByAllFields: false,
	pagerMode: GridPagerMode.InfiniteScroll,
	showTopBar: false,
})
export class EntityForLink extends PXView {
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
