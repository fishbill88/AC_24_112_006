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
	graphType: "PX.Objects.PM.ReverseUnbilledProcess",
	primaryView: "Filter"
})
export class PM505000 extends PXScreen {
	ViewDocument: PXActionState;

	Filter = createSingle(Filter);
	Items = createCollection(Items);
}
