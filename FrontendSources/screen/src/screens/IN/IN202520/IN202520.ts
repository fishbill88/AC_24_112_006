import {
	PXView,
	PXFieldState,
	PXFieldOptions,

	createSingle,
	createCollection,

	graphInfo,
	viewInfo,
	gridConfig,
	columnConfig,
	localizable,

	GridColumnShowHideMode,
	GridPreset,
} from "client-controls";

import { ScanHeaderBase } from "../../barcodeProcessing/views";
import { BarcodeProcessingScreen } from "../../barcodeProcessing/BarcodeProcessingScreen";

@graphInfo({ graphType: 'PX.Objects.IN.WMS.InventoryItemLookup+Host', primaryView: 'HeaderView' })
export class IN202520 extends BarcodeProcessingScreen {
	@viewInfo({ containerName: "Scan Header" })
	HeaderView = createSingle(ScanHeader);

	@viewInfo({ containerName: "Item Info" })
	InventoryItem = createSingle(ItemInfo);

	@viewInfo({ containerName: "Item Availability"})
	ISERecords = createCollection(ItemAvailability);
}

export class ScanHeader extends ScanHeaderBase {
	SiteID: PXFieldState<PXFieldOptions.Disabled>;
	InventoryID: PXFieldState<PXFieldOptions.Disabled>;
}

export class ItemInfo extends PXView {
	ItemClassID: PXFieldState<PXFieldOptions.Disabled>;
	ItemType: PXFieldState<PXFieldOptions.Disabled>;
	KitItem: PXFieldState<PXFieldOptions.Disabled>;
	ValMethod: PXFieldState<PXFieldOptions.Disabled>;
	LotSerClassID: PXFieldState<PXFieldOptions.Disabled>;

	ProductWorkgroupID: PXFieldState<PXFieldOptions.Disabled>;
	ProductManagerID: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	BaseUnit: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	SalesUnit: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	PurchaseUnit: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	suppressNoteFiles: true,
	allowUpdate: false,
})
export class ItemAvailability extends PXView {
	@localizable
	static Unassigned = "<UNASSIGNED>";

	@columnConfig({ hideViewLink: true, nullText: ItemAvailability.Unassigned })
	LocationID: PXFieldState<PXFieldOptions.Disabled>;

	QtyAvail: PXFieldState<PXFieldOptions.Disabled>;
	QtyHardAvail: PXFieldState<PXFieldOptions.Disabled>;
	QtyInTransit: PXFieldState<PXFieldOptions.Disabled>;
	QtyExpired: PXFieldState<PXFieldOptions.Disabled>;
	QtyOnHand: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	BaseUnit: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true, allowShowHide: GridColumnShowHideMode.Server })
	LotSerialNbr: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	ExpireDate: PXFieldState<PXFieldOptions.Disabled>;
}
