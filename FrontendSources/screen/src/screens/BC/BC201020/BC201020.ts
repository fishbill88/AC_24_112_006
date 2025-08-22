import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
} from "client-controls";

import {
	Bindings,
	CurrentBinding,
	CurrentBindingAmazon,
	CurrentStore,
	Entities,
	ExportLocations,
	PaymentMethods,
	FeeMappings,
	ShippingMappings
} from "./views";

@graphInfo({ graphType: 'PX.Commerce.Amazon.BCAmazonStoreMaint', primaryView: 'Bindings' })
export class BC201020 extends PXScreen {
	Bindings = createSingle(Bindings);

	CurrentBindingAmazon = createSingle(CurrentBindingAmazon);

	CurrentStore = createSingle(CurrentStore);

	CurrentBinding = createSingle(CurrentBinding);

	Entities = createCollection(Entities);

	ExportLocations = createCollection(ExportLocations);

	PaymentMethods = createCollection(PaymentMethods);

	FeeMappings = createCollection(FeeMappings);

	ShippingMappings = createCollection(ShippingMappings);
}
