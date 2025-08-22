import {
	createCollection,
	createSingle,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	Filter,
	Claim
} from "./views";

@graphInfo({
	graphType: "PX.Objects.EP.ExpenseClaimMaint",
	primaryView: "Filter"
})
export class EP301030 extends PXScreen {
	EditDetail: PXActionState;

	Filter = createSingle(Filter);
	Claim = createCollection(Claim);
}
