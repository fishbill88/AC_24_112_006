import {
	PXView,
	PXFieldState,
	PXFieldOptions,

	createCollection,

	viewInfo,
	gridConfig,
	GridPreset,
} from "client-controls";

import { SO503075 } from '../SO503075';

export interface SO503075_LocationLinkFilter extends SO503075 { }
export class SO503075_LocationLinkFilter {
	@viewInfo({ containerName: "Selected Locations" })
	SelectedLocations = createCollection(LocationLinks);
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
})
export class LocationLinks extends PXView {
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState<PXFieldOptions.Disabled>;
}