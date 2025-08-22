import { Messages as SysMessages } from "client-controls/services/messages";
import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, TextAlign } from "client-controls";

@graphInfo({graphType: "PX.Objects.PO.POCreateIntercompanySalesOrder", primaryView: "Filter" })
export class PO504000 extends PXScreen {
	ViewSODocument: PXActionState;
	ViewPOReceipt: PXActionState;

	@viewInfo({containerName: "Selection"})
	Filter = createSingle(SOForPurchaseReceiptFilter);
   	@viewInfo({containerName: "Documents"})
	Documents = createCollection(SOShipment);
}

// Views

export class SOForPurchaseReceiptFilter extends PXView  {
	DocDate : PXFieldState<PXFieldOptions.CommitChanges>;
	PurchasingCompany : PXFieldState<PXFieldOptions.CommitChanges>;
	SellingCompany : PXFieldState<PXFieldOptions.CommitChanges>;
	PutReceiptsOnHold : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	mergeToolbarWith: "ScreenToolbar"
})
export class SOShipment extends PXView  {
	@columnConfig({allowSort: false, allowCheckAll: true})	Selected : PXFieldState;
	@columnConfig({hideViewLink: true}) CustomerID : PXFieldState;
	@columnConfig({hideViewLink: true}) SOOrder__BranchID : PXFieldState;
	@linkCommand("ViewSODocument")
	ShipmentNbr : PXFieldState;
	Status : PXFieldState;
	ShipDate : PXFieldState;
	ShipmentQty : PXFieldState;
	@columnConfig({hideViewLink: true}) SiteID : PXFieldState;
	ShipmentDesc : PXFieldState;
	WorkgroupID : PXFieldState;
	ShipmentWeight : PXFieldState;
	ShipmentVolume : PXFieldState;
	PackageCount : PXFieldState;
	PackageWeight : PXFieldState;
	SOOrder__IntercompanyPONbr : PXFieldState;
	Excluded : PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewPOReceipt")
	IntercompanyPOReceiptNbr : PXFieldState;
}
