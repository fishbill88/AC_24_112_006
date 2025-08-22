import {
	createCollection,
	createSingle,
	graphInfo,
	PXScreen
} from "client-controls";

import {
	Filter,
	RateDefinitions
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.RateDefinitionMaint",
	primaryView: "Filter"
})
export class PM205000 extends PXScreen {
	Filter = createSingle(Filter);
	RateDefinitions = createCollection(RateDefinitions);
}
