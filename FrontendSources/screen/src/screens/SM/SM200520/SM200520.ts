// TODO: Apply to Multiple Tenants smart panel loses its DialogResult (https://jira.acumatica.com/browse/AC-290473)
// TODO: missing RefreshSitemap functionality (https://jira.acumatica.com/browse/AC-290458)
// TODO: hide Import from Excel button (https://jira.acumatica.com/browse/AC-290457)
// TODO: toolbar actions are in invalid order (https://jira.acumatica.com/browse/AC-290471)

import {
	PXScreen,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	columnConfig,
	viewInfo,
	GridPagerMode,
} from "client-controls";
import { Messages as SysMessages } from "client-controls/services/messages";

@graphInfo({
	graphType: "PX.SiteMap.Graph.SiteMapMaint",
	primaryView: "SiteMap",
})
export class SM200520 extends PXScreen {
	SysMessages = SysMessages;

	SiteMap = createCollection(SiteMap);

	@viewInfo({ containerName: "Apply to Multiple Tenants" })
	ViewCompanyList = createCollection(Company);
}

@gridConfig({
	adjustPageSize: true,
	mergeToolbarWith: "ScreenToolbar",
	syncPosition: true,
	// TODO: add "fastFilterByAllFields: false" once available in the main UI Pool branch
})
export class SiteMap extends PXView {
	@columnConfig({ allowFastFilter: true })
	ScreenID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowFastFilter: true })
	Title: PXFieldState;

	@columnConfig({ allowFastFilter: false }) // TODO: remove allowFastFilter: false once fastFilterByAllFields is available
	SelectedUI: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowFastFilter: false })
	Url: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowFastFilter: false })
	Graphtype: PXFieldState;

	@columnConfig({ allowFastFilter: false })
	Workspaces: PXFieldState;

	@columnConfig({ allowFastFilter: false })
	Category: PXFieldState;

	@columnConfig({ allowFastFilter: false })
	ListIsEntryPoint: PXFieldState;
}

@gridConfig({
	autoAdjustColumns: true,
	batchUpdate: true,
	syncPosition: true,
	pagerMode: GridPagerMode.InfiniteScroll,
})
export class Company extends PXView {
	Selected: PXFieldState;

	Name: PXFieldState;

	ID: PXFieldState;

	ParentID: PXFieldState;
}
