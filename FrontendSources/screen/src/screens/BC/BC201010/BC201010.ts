import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
} from "client-controls";

import {
	Bindings,
	CurrentBinding,
	CurrentBindingShopify,
	CurrentStore,
	Entities,
	ExportLocations,
	ImportLocations,
	PaymentMethods,
	FeeMappings,
	ShippingMappings
} from "./views";

@graphInfo({ graphType: 'PX.Commerce.Shopify.BCShopifyStoreMaint', primaryView: 'Bindings' })
export class BC201010 extends PXScreen {
	Bindings = createSingle(Bindings);

	CurrentBindingShopify = createSingle(CurrentBindingShopify);

	CurrentStore = createSingle(CurrentStore);

	CurrentBinding = createSingle(CurrentBinding);

	Entities = createCollection(Entities);

	ExportLocations = createCollection(ExportLocations);

	ImportLocations = createCollection(ImportLocations);

	PaymentMethods = createCollection(PaymentMethods);

	FeeMappings = createCollection(FeeMappings);

	ShippingMappings = createCollection(ShippingMappings);
}
