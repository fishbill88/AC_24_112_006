import {
	createCollection,
	graphInfo,
	PXScreen
} from "client-controls";

import {
	Items
} from "./views";

@graphInfo({
	graphType: "PX.Objects.CN.ProjectAccounting.CostProjectionClassMaint",
	primaryView: "Items",
	hideFilesIndicator: true,
	hideNotesIndicator: true
})
export class PM203500 extends PXScreen {
	Items = createCollection(Items);
}
