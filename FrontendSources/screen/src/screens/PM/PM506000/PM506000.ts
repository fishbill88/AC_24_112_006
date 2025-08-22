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
	graphType: "PX.Objects.PM.ProFormaProcess",
	primaryView: "Filter"
})
export class PM506000 extends PXScreen {
	viewDocumentProject: PXActionState;
	viewDocumentRef: PXActionState;

	Filter = createSingle(Filter);
	Items = createCollection(Items);
}
