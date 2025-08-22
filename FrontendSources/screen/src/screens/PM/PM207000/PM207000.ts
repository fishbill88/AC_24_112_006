import {
	PXScreen,
	createSingle,
	createCollection,
	graphInfo
} from "client-controls";

import {
	Billing,
	BillingRules,
	BillingRule,
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.BillingMaint",
	primaryView: "Billing"
})
export class PM207000 extends PXScreen {
	Billing = createSingle(Billing);
	BillingRules = createCollection(BillingRules);
	BillingRule = createSingle(BillingRule);
}

