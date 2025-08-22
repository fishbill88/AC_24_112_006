import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,

	createSingle,

	graphInfo,
	viewInfo,
} from "client-controls";

import { ScanHeaderBase } from "../../barcodeProcessing/views";
import { BarcodeProcessingScreen } from "../../barcodeProcessing/BarcodeProcessingScreen";

@graphInfo({ graphType: 'PX.Objects.PO.WMS.ReceivePutAway+Host', primaryView: 'HeaderView' })
export class PO302020 extends BarcodeProcessingScreen {
	ViewOrder: PXActionState;

	@viewInfo({ containerName: "Scan Header" })
	HeaderView = createSingle(ScanHeader);

	@viewInfo({ containerName: "Receipt" })
	Document = createSingle(Receipt);

	@viewInfo({ containerName: "User Setup" })
	UserSetupView = createSingle(UserSetup);
}

export class ScanHeader extends ScanHeaderBase {
	RefNbr: PXFieldState<PXFieldOptions.Disabled>;
	TransferRefNbr: PXFieldState<PXFieldOptions.Disabled>;
	CartID: PXFieldState<PXFieldOptions.Disabled>;
	Remove: PXFieldState<PXFieldOptions.Disabled>;
	CartLoaded: PXFieldState<PXFieldOptions.Disabled>;

	ShowReceive: PXFieldState<PXFieldOptions.Hidden | PXFieldOptions.Disabled>;
	ShowReturn: PXFieldState<PXFieldOptions.Hidden | PXFieldOptions.Disabled>;
	ShowPutAway: PXFieldState<PXFieldOptions.Hidden | PXFieldOptions.Disabled>;
	ShowLog: PXFieldState<PXFieldOptions.Hidden | PXFieldOptions.Disabled>;
}

export class Receipt extends PXView {
	WMSSingleOrder : PXFieldState<PXFieldOptions.Disabled>;
}

export class UserSetup extends PXView {
	DefaultLotSerialNumber: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultExpireDate: PXFieldState<PXFieldOptions.CommitChanges>;
	SingleLocation: PXFieldState<PXFieldOptions.CommitChanges>;

	PrintInventoryLabelsAutomatically: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryLabelsReportID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintPurchaseReceiptAutomatically: PXFieldState<PXFieldOptions.CommitChanges>;

	UseScale: PXFieldState<PXFieldOptions.CommitChanges>;
	ScaleDeviceID: PXFieldState<PXFieldOptions.CommitChanges>;
}