import {
	PXView,
	PXFieldState,
	PXFieldOptions,

	createCollection,

	viewInfo,
	gridConfig,
	GridPreset,
} from "client-controls";

import { SO503050 } from '../SO503050';

export interface SO503050_LocationLinkFilter extends SO503050 { }
export class SO503050_LocationLinkFilter {
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