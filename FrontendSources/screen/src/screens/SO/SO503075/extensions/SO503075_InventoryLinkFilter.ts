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

export interface SO503075_InventoryLinkFilter extends SO503075 { }
export class SO503075_InventoryLinkFilter {
	@viewInfo({ containerName: "Selected Inventory Items" })
	SelectedItems = createCollection(InventoryLinks);
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true
})
export class InventoryLinks extends PXView {
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState<PXFieldOptions.Disabled>;
}