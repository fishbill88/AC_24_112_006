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

	GridColumnShowHideMode,
	GridPreset,
} from "client-controls";

import { ScanHeaderBase } from "../../barcodeProcessing/views";
import { BarcodeProcessingScreen } from "../../barcodeProcessing/BarcodeProcessingScreen";

@graphInfo({ graphType: 'PX.Objects.IN.WMS.StoragePlaceLookup+Host', primaryView: 'HeaderView' })
export class IN409020 extends BarcodeProcessingScreen {
	@viewInfo({ containerName: "Scan Header" })
	HeaderView = createSingle(ScanHeader);

	@viewInfo({ containerName: "Storages"})
	storages = createCollection(Storages);
}

export class ScanHeader extends ScanHeaderBase {
	SiteID: PXFieldState<PXFieldOptions.Disabled>;
	StorageID: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	suppressNoteFiles: true,
	allowUpdate: false,
})
export class Storages extends PXView {
	InventoryID: PXFieldState<PXFieldOptions.Disabled>;
	InventoryDescr: PXFieldState<PXFieldOptions.Disabled>;
	SubItemID: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true, allowShowHide: GridColumnShowHideMode.Server })
	LotSerialNbr: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	ExpireDate: PXFieldState<PXFieldOptions.Disabled>;

	Qty: PXFieldState<PXFieldOptions.Disabled>;
	QtyPickedToCart: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	BaseUnit: PXFieldState<PXFieldOptions.Disabled>;
}
