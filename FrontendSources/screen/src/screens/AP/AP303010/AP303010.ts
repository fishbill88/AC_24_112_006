import { Location, Address, Contact, LocationAPPaymentInfo, VendorPaymentMethodDetail, LocationAPAccountSub, AddressLookupFilter, LocationBranchSettings } from './views';
import { PXActionState, graphInfo, PXScreen, createSingle, createCollection } from "client-controls";

@graphInfo({ graphType: 'PX.Objects.AP.VendorLocationMaint', primaryView: 'Location', bpEventsIndicator: false, showUDFIndicator: true })
export class AP303010 extends PXScreen {
	AddressLookup: PXActionState;
	ViewOnMap: PXActionState;
	RemitAddressLookup: PXActionState;
	ViewRemitOnMap: PXActionState;
	AddressLookupSelectAction: PXActionState;

	Location = createSingle(Location);
	LocationCurrent = createSingle(Location);
	Address = createSingle(Address);
	Contact = createSingle(Contact);
	RemitAddress = createSingle(Address);
	RemitContact = createSingle(Contact);
	APPaymentInfoLocation = createSingle(LocationAPPaymentInfo);
	PaymentDetails = createCollection(VendorPaymentMethodDetail);
	APAccountSubLocation = createSingle(LocationAPAccountSub);
	AddressLookupFilter = createSingle(AddressLookupFilter);

	LocationBranchSettings = createSingle(LocationBranchSettings);
}
