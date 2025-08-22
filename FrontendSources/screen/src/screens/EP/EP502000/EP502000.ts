import {
	createCollection,
	createSingle,
	graphInfo,
	PXScreen
} from "client-controls";

import {
	Filter,
	Customers
} from "./views";

@graphInfo({
	graphType: "PX.Objects.EP.EPCustomerBilling",
	primaryView: "Filter"
})
export class EP502000 extends PXScreen {
	Filter = createSingle(Filter);
	Customers = createCollection(Customers);
}
