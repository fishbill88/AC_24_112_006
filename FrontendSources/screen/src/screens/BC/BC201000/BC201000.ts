import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
} from "client-controls";

import {
	Bindings,
	CurrentBindingBigCommerce,
	CurrentStore,
	CurrentBinding,
	Entities,
	ExportLocations,
	PaymentMethods,
	ShippingMappings
} from './views';

@graphInfo({ graphType: 'PX.Commerce.BigCommerce.BCBigCommerceStoreMaint', primaryView: 'Bindings' })
export class BC201000 extends PXScreen {
	Bindings = createSingle(Bindings);

	CurrentBindingBigCommerce = createSingle(CurrentBindingBigCommerce);

	CurrentStore = createSingle(CurrentStore);

	CurrentBinding = createSingle(CurrentBinding);

	Entities = createCollection(Entities);

	ExportLocations = createCollection(ExportLocations);

	PaymentMethods = createCollection(PaymentMethods);

	ShippingMappings = createCollection(ShippingMappings);
}
