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

export interface SO503050_InventoryLinkFilter extends SO503050 { }
export class SO503050_InventoryLinkFilter {
	@viewInfo({ containerName: "Selected Inventory Items" })
	SelectedItems = createCollection(InventoryLinks);
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
})
export class InventoryLinks extends PXView {
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState<PXFieldOptions.Disabled>;
}