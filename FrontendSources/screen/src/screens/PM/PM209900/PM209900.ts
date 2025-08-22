import {
	createCollection,
	createSingle,
	graphInfo,
	PXScreen
} from "client-controls";

import {
	Filter,
	Items
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.LaborCostRateMaint",
	primaryView: "Filter"
})
export class PM209900 extends PXScreen {
	Filter = createSingle(Filter);
	Items = createCollection(Items);
}
