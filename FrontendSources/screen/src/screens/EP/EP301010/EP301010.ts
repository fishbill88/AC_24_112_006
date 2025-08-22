import {
	createCollection,
	createSingle,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	Filter,
	ClaimDetails
} from "./views";

@graphInfo({
	graphType: "PX.Objects.EP.ExpenseClaimDetailMaint",
	primaryView: "Filter"
})
export class EP301010 extends PXScreen {
	editDetail: PXActionState;
	viewClaim: PXActionState;

	Filter = createSingle(Filter);
	ClaimDetails = createCollection(ClaimDetails);
}
