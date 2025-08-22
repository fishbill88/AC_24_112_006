import {
	createCollection,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	Items
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.ProjectBalanceMaint",
	primaryView: "Items"
})
export class PM309000 extends PXScreen {
	ViewProject: PXActionState;
	ViewTask: PXActionState;

	Items = createCollection(Items);
}
