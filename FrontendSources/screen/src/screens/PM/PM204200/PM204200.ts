import {
	createCollection,
	graphInfo,
	PXScreen
} from "client-controls";

import {
	RateTables
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.RateTableMaint",
	primaryView: "RateTables",
	hideFilesIndicator: true,
	hideNotesIndicator: true
})
export class PM204200 extends PXScreen {
	RateTables = createCollection(RateTables);
}
