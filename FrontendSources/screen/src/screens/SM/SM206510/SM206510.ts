import {
	createCollection,
	PXScreen,
	graphInfo,
	gridConfig,
	PXView,
	PXFieldState
} from "client-controls";

@graphInfo({graphType: "PX.SM.SMPrinterMaint", primaryView: "Printers"})
export class SM206510 extends PXScreen {

	Printers = createCollection(SMPrinter);
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	adjustPageSize: true
})
export class SMPrinter extends PXView  {
	DeviceHubID : PXFieldState;
	PrinterName : PXFieldState;
	Description : PXFieldState;
	DefaultNumberOfCopies : PXFieldState;
	IsActive : PXFieldState;
}