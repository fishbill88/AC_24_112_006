import {
	createCollection,
	createSingle,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	Filter,
	Records
} from "./views";

@graphInfo({
	graphType: "PX.Objects.CN.Subcontracts.SC.Graphs.PrintSubcontract",
	primaryView: "Filter"
})
export class SC503000 extends PXScreen {
	ViewSubcontractDetails: PXActionState;

	Filter = createSingle(Filter);
	Records = createCollection(Records);
}
