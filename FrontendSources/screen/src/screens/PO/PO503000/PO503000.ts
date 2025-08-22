import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXPageLoadBehavior } from "client-controls";
import { PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig } from "client-controls";

@graphInfo({graphType: "PX.Objects.PO.POPrintOrder", primaryView: "Filter", pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues})
export class PO503000 extends PXScreen {

   	@viewInfo({containerName: "Selection"})
	Filter = createSingle(POPrintOrderFilter);
   	@viewInfo({containerName: "Orders"})
	Records = createCollection(POOrder);
}

// Views

export class POPrintOrderFilter extends PXView  {
	Action : PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID : PXFieldState<PXFieldOptions.CommitChanges>;
	MyOwner : PXFieldState<PXFieldOptions.CommitChanges>;
	WorkGroupID : PXFieldState<PXFieldOptions.CommitChanges>;
	MyWorkGroup : PXFieldState<PXFieldOptions.CommitChanges>;
	PrintWithDeviceHub : PXFieldState<PXFieldOptions.CommitChanges>;
	DefinePrinterManually : PXFieldState<PXFieldOptions.CommitChanges>;
	PrinterID : PXFieldState<PXFieldOptions.CommitChanges>;
	NumberOfCopies : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	mergeToolbarWith: "ScreenToolbar"
})
export class POOrder extends PXView  {
	@columnConfig({ allowCheckAll: true })
	Selected : PXFieldState;
	OrderType : PXFieldState<PXFieldOptions.Disabled>;
	OrderNbr : PXFieldState;
	OrderDate : PXFieldState;
	Status : PXFieldState;
	EPEmployee__acctName : PXFieldState;
	OrderDesc : PXFieldState;
	OwnerID : PXFieldState;
	CuryID : PXFieldState;
	CuryControlTotal : PXFieldState;
	Vendor__AcctCD : PXFieldState;
	Vendor__AcctName : PXFieldState;
	Vendor__VendorClassID : PXFieldState;
}
