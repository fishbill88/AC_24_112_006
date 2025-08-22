import { CR301000 } from "../CR301000";
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
	viewInfo,
} from "client-controls";

export interface CR301000_panel_SelectContactForLink extends CR301000 {}
export class CR301000_panel_SelectContactForLink {
	@viewInfo({ containerName: "Associate the Contact with the Lead" })
	Link_Filter = createSingle(EntityForLinkFilter);
	@viewInfo({ containerName: "Associate the Contact with the Lead" })
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
