import { VendorDiscountSequence, APDiscountEx, UpdateSettingsFilter, DiscountDetail, DiscountItem, APDiscountLocation, DiscountInventoryPriceClass } from './views';
import { graphInfo, PXScreen, createSingle, createCollection } from "client-controls";

@graphInfo({ graphType: 'PX.Objects.AP.APDiscountSequenceMaint', primaryView: 'Sequence', bpEventsIndicator: false, showUDFIndicator: true })
export class AP205000 extends PXScreen {

	Sequence = createSingle(VendorDiscountSequence);
	Discount = createSingle(APDiscountEx);
	UpdateSettings = createSingle(UpdateSettingsFilter);
	Details = createCollection(DiscountDetail);
	Items = createCollection(DiscountItem);
	Locations = createCollection(APDiscountLocation);
	InventoryPriceClasses = createCollection(DiscountInventoryPriceClass);

}
