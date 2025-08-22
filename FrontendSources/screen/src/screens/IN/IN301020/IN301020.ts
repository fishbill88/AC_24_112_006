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
	GridPreset,
} from "client-controls";

import { ScanHeaderBase } from "../../barcodeProcessing/views";
import { BarcodeProcessingScreen } from "../../barcodeProcessing/BarcodeProcessingScreen";

@graphInfo({ graphType: 'PX.Objects.IN.WMS.INScanReceive+Host', primaryView: 'HeaderView' })
export class IN301020 extends BarcodeProcessingScreen {
	@viewInfo({ containerName: "Scan Header" })
	HeaderView = createSingle(ScanHeader);

	@viewInfo({ containerName: "User Setup" })
	UserSetupView = createSingle(UserSetup);

	@viewInfo({ containerName: "Receipt Lines"})
	transactions = createCollection(ReceiptLines);
}

export class ScanHeader extends ScanHeaderBase {
	RefNbr: PXFieldState<PXFieldOptions.Disabled>;
	SiteID: PXFieldState<PXFieldOptions.Disabled>;
	Remove: PXFieldState<PXFieldOptions.Disabled>;
}

export class UserSetup extends PXView {
	DefaultWarehouse: PXFieldState<PXFieldOptions.CommitChanges>;

	PrintInventoryLabelsAutomatically: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryLabelsReportID: PXFieldState<PXFieldOptions.CommitChanges>;

	UseScale: PXFieldState<PXFieldOptions.CommitChanges>;
	ScaleDeviceID: PXFieldState<PXFieldOptions.CommitChanges>;
}
@gridConfig({
	preset: GridPreset.Inquiry,
	suppressNoteFiles: true,
	allowUpdate: false,
})
export class ReceiptLines extends PXView {
	InventoryID: PXFieldState<PXFieldOptions.Disabled>;
	TranDesc: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	LotSerialNbr: PXFieldState<PXFieldOptions.Disabled>;

	ExpireDate: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	LocationID: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	ReasonCode: PXFieldState<PXFieldOptions.Disabled>;

	Qty: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.Disabled>;
}
