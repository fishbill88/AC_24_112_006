import {
	createCollection,
	createSingle,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	PhotoLogSetup,
	PhotoLogStatuses,
	Attributes
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PJ.PhotoLogs.PJ.Graphs.PhotoLogSetupMaint",
	primaryView: "PhotoLogSetup"
})
export class PJ103000 extends PXScreen {
	CRAttribute_ViewDetails: PXActionState;

	PhotoLogSetup = createSingle(PhotoLogSetup);
	PhotoLogStatuses = createCollection(PhotoLogStatuses);
	Attributes = createCollection(Attributes);
}
