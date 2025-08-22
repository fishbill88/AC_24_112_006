// TODO: missing PXSelector.FilterByAllFields (https://jira.acumatica.com/browse/AC-290454)
// TODO: missing PXSelector.DisplayMode and PXSelector.TextField (https://jira.acumatica.com/browse/AC-285843)
// TODO: missing PXGridColumn.DisplayMode (https://jira.acumatica.com/browse/AC-268656)
// TODO: missing RefreshSitemap functionality (https://jira.acumatica.com/browse/AC-290458)
// TODO: disable FastFilter in the grid (https://jira.acumatica.com/browse/AC-290459)
// TODO: hide Import from Excel button (https://jira.acumatica.com/browse/AC-290457)

import {
	PXScreen,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	columnConfig,
} from "client-controls";

@graphInfo({ graphType: "PX.Data.LEPMaint", primaryView: "Items" })
export class SM208500 extends PXScreen {
	Items = createCollection(ListEntryPoint);
}

@gridConfig({
	adjustPageSize: true,
	mergeToolbarWith: "ScreenToolbar",
})
export class ListEntryPoint extends PXView {
	IsActive: PXFieldState;

	EntryScreenID: PXFieldState<PXFieldOptions.CommitChanges>;

	ListScreenID: PXFieldState<PXFieldOptions.CommitChanges>;
}
