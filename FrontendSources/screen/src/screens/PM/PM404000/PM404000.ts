import {
	createCollection,
	createSingle,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	Filter,
	BudgetControlLines
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.BudgetOverrunInquiry",
	primaryView: "Filter"
})
export class PM404000 extends PXScreen {
	EditDocument: PXActionState;

	Filter = createSingle(Filter);
	BudgetControlLines = createCollection(BudgetControlLines);
}
