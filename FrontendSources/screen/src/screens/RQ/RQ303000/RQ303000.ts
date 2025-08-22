import { Messages as SysMessages } from "client-controls/services/messages";
import { createCollection, createSingle, PXScreen, graphInfo, PXActionState,
	viewInfo } from "client-controls";
import { RQBiddingVendorHeader, RQBiddingVendor, RQRequisitionLineBidding,
	POContact, POAddress, CurrencyInfo } from "./views";

@graphInfo({ graphType: "PX.Objects.RQ.RQBiddingEntry", primaryView: "Vendor", showUDFIndicator: true })
export class RQ303000 extends PXScreen {

   	@viewInfo({containerName: "Bidding Response"})
	Vendor = createSingle(RQBiddingVendorHeader);
	CurrentDocument = createSingle(RQBiddingVendor);
   	@viewInfo({containerName: "Bidding Details"})
	Lines = createCollection(RQRequisitionLineBidding);
   	@viewInfo({containerName: "Vendor Contact"})
	Remit_Contact = createSingle(POContact);
   	@viewInfo({containerName: "Vendor Address"})
	Remit_Address = createSingle(POAddress);
	_RQBiddingVendor_CurrencyInfo_ = createSingle(CurrencyInfo);
}
