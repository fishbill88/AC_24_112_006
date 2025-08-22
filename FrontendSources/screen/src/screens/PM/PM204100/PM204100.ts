import {
	createCollection,
	graphInfo,
	PXScreen,
} from "client-controls";

import {
	RateTypes
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.RateTypeMaint",
	primaryView: "RateTypes"
})
export class PM204100 extends PXScreen {
	RateTypes = createCollection(RateTypes);
}
