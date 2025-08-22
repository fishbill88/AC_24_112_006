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

@graphInfo({ graphType: 'PX.Objects.SO.WMS.PickPackShip+Host', primaryView: 'HeaderView' })
export class SO302020 extends BarcodeProcessingScreen {
	ViewOrder: PXActionState;

	@viewInfo({ containerName: "Scan Header" })
	HeaderView = createSingle(ScanHeader);

	@viewInfo({ containerName: "Setup" })
	Setup = createSingle(PPSSetup);

	@viewInfo({ containerName: "User Setup" })
	UserSetupView = createSingle(UserSetup);
}

export class ScanHeader extends ScanHeaderBase {
	RefNbr: PXFieldState<PXFieldOptions.Disabled>;
	WorksheetNbr: PXFieldState<PXFieldOptions.Disabled>;
	SingleShipmentNbr: PXFieldState<PXFieldOptions.Disabled>;
	LastVisitedLocationID: PXFieldState<PXFieldOptions.Disabled>;
	CartID: PXFieldState<PXFieldOptions.Disabled>;
	Remove: PXFieldState<PXFieldOptions.Disabled>;
	CartLoaded: PXFieldState<PXFieldOptions.Disabled>;

	ShowPickWS: PXFieldState<PXFieldOptions.Hidden | PXFieldOptions.Disabled>;
	ShowPick: PXFieldState<PXFieldOptions.Hidden | PXFieldOptions.Disabled>;
	ShowPack: PXFieldState<PXFieldOptions.Hidden | PXFieldOptions.Disabled>;
	ShowShip: PXFieldState<PXFieldOptions.Hidden | PXFieldOptions.Disabled>;
	ShowReturn: PXFieldState<PXFieldOptions.Hidden | PXFieldOptions.Disabled>;
	ShowLog: PXFieldState<PXFieldOptions.Hidden | PXFieldOptions.Disabled>;

	PackageLineNbrUI: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class PPSSetup extends PXView {
	ShowPickTab : PXFieldState<PXFieldOptions.Disabled>;
	ShowPackTab : PXFieldState<PXFieldOptions.Disabled>;
	ShowShipTab : PXFieldState<PXFieldOptions.Disabled>;
	ShowReturningTab : PXFieldState<PXFieldOptions.Disabled>;
	ShowScanLogTab : PXFieldState<PXFieldOptions.Disabled>;
}

export class UserSetup extends PXView {
	DefaultLocationFromShipment: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultLotSerialFromShipment: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintShipmentConfirmation: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintShipmentLabels: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintCommercialInvoices: PXFieldState<PXFieldOptions.CommitChanges>;
	UseScale: PXFieldState<PXFieldOptions.CommitChanges>;
	ScaleDeviceID: PXFieldState<PXFieldOptions.CommitChanges>;
	EnterSizeForPackages: PXFieldState<PXFieldOptions.CommitChanges>;
}