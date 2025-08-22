import {
	createCollection,
	PXScreen,
	graphInfo,
	gridConfig,
	PXView,
	PXFieldState,
	PXFieldOptions
} from "client-controls";

@graphInfo({graphType: "PX.SM.SMScannerMaint", primaryView: "Scanners"})
export class SM206540 extends PXScreen {

	Scanners = createCollection(SMScanner);
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	adjustPageSize: true
})
export class SMScanner extends PXView  {
	DeviceHubID : PXFieldState;
	ScannerName : PXFieldState;
	Description : PXFieldState;
	PaperSourceComboValues : PXFieldState<PXFieldOptions.CommitChanges>;
	PixelTypeComboValues : PXFieldState<PXFieldOptions.CommitChanges>;
	ResolutionComboValues : PXFieldState<PXFieldOptions.CommitChanges>;
	FileTypeComboValues : PXFieldState<PXFieldOptions.CommitChanges>;
	PaperSourceDefValue : PXFieldState;
	PixelTypeDefValue : PXFieldState;
	ResolutionDefValue : PXFieldState;
	FileTypeDefValue : PXFieldState;
	IsActive : PXFieldState;
}