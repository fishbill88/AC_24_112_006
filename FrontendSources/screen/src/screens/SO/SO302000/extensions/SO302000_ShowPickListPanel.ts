import {
	PXView,
	PXActionState,
	PXFieldState,
	PXFieldOptions,

	createSingle,
	createCollection,

	viewInfo,
	gridConfig,
	columnConfig,
} from "client-controls";

import { SO302000 } from '../SO302000';

export interface SO302000_ShowPickListPanel extends SO302000 { }
export class SO302000_ShowPickListPanel {
	ShowPickList: PXActionState;
	DeletePickList: PXActionState;
	ViewPickListSource: PXActionState;

	@viewInfo({ containerName: "Pick List Header" })
	PickListHeader = createSingle(PickListHeader);

	@viewInfo({ containerName: "Pick List Entries" })
	PickListEntries = createCollection(PickListEntries);
}

export class PickListHeader extends PXView {
	PickListNbr: PXFieldState<PXFieldOptions.Disabled>;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	Priority: PXFieldState<PXFieldOptions.Disabled>;
	AutomaticShipmentConfirmation: PXFieldState<PXFieldOptions.Disabled>;

	SOPicker__PathLength: PXFieldState<PXFieldOptions.Disabled>;
	PreferredAssigneeID: PXFieldState<PXFieldOptions.Disabled>;
	ActualAssigneeID: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	syncPosition: true,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false,
	allowImport: false
})
export class PickListEntries extends PXView {
	@columnConfig({ hideViewLink: true })
	LocationID: PXFieldState<PXFieldOptions.Disabled>;

	InventoryID: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	SubItemID: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	LotSerialNbr: PXFieldState<PXFieldOptions.Disabled>;

	Qty: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.Disabled>;

	ExpireDate: PXFieldState<PXFieldOptions.Disabled>;
	PickedQty: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	ToteID: PXFieldState<PXFieldOptions.Disabled>;
}