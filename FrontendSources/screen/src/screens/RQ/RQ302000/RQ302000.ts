import { Messages as SysMessages } from "client-controls/services/messages";
import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior } from "client-controls";
import { RQRequisition, RQRequisition2, RQRequisitionLine, POContact2, POAddress2, RQBiddingVendor, CurrencyInfo, POContact3, POAddress3, POOrder, SOOrder } from "./views";

@graphInfo({graphType: "PX.Objects.RQ.RQRequisitionEntry", primaryView: "Document", bpEventsIndicator: true, showUDFIndicator: true })
export class RQ302000 extends PXScreen {
	AddSelectedItems: PXActionState;
	RecalculatePricesActionOk: PXActionState;
	AddressLookup: PXActionState;
	RemitAddressLookup: PXActionState;
	AddressLookupSelectAction: PXActionState;

   	@viewInfo({containerName: "Document Summary"})
	Document = createSingle(RQRequisition);
	CurrentDocument = createSingle(RQRequisition2);
   	@viewInfo({containerName: "Details"})
	Lines = createCollection(RQRequisitionLine);
   	@viewInfo({containerName: "Ship-To Contact"})
	Shipping_Contact = createSingle(POContact2);
   	@viewInfo({containerName: "Ship-To Address"})
	Shipping_Address = createSingle(POAddress2);
   	@viewInfo({containerName: "Bidding Vendors"})
	Vendors = createCollection(RQBiddingVendor);
   	@viewInfo({containerName: "Bidding Vendors"})
	_RQBiddingVendor_CurrencyInfo_ = createSingle(CurrencyInfo);
   	@viewInfo({containerName: "Vendor Contact"})
	Remit_Contact = createSingle(POContact3);
   	@viewInfo({containerName: "Vendor Address"})
	Remit_Address = createSingle(POAddress3);
   	@viewInfo({containerName: "Purchase Orders"})
	POOrders = createCollection(POOrder);
   	@viewInfo({containerName: "Sales Orders"})
	SOOrders = createCollection(SOOrder);
   	@viewInfo({containerName: "_RQRequisition_CurrencyInfo_"})
	_RQRequisition_CurrencyInfo_ = createSingle(CurrencyInfo);
}
