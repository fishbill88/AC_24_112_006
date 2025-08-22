import {
	createCollection,
	createSingle,
	graphInfo,
	PXScreen
} from "client-controls";

import {
	Filter,
	Mapping,
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.ProjectAttributeGroupMaint",
	primaryView: "Filter"
})
export class PM202000 extends PXScreen {
	Filter = createSingle(Filter);
	Mapping = createCollection(Mapping);
}
