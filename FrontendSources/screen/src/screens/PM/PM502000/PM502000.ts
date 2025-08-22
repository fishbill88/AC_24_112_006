import {
	createCollection,
	graphInfo,
	createSingle,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	Filter,
	Items
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.AllocationProcess",
	primaryView: "Filter"
})
export class PM502000 extends PXScreen {
	ViewProject: PXActionState;
	ViewTask: PXActionState;

	Filter = createSingle(Filter);
	Items = createCollection(Items);
}
