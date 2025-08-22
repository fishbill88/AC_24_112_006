import {
	createCollection,
	createSingle,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	Filter,
	Items
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.AllocationProcessByProject",
	primaryView: "Filter"
})
export class PM502500 extends PXScreen {
	ViewProject: PXActionState;

	Filter = createSingle(Filter);
	Items = createCollection(Items);
}
