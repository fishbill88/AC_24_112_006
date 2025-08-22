import {
	createCollection,
	createSingle,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	Filter,
	FilteredItems
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.TaskInquiry",
	primaryView: "Filter"
})
export class PM402000 extends PXScreen {
	ViewProject: PXActionState;
	ViewTask: PXActionState;

	Filter = createSingle(Filter);
	FilteredItems = createCollection(FilteredItems);
}
