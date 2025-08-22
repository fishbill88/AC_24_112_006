import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo
} from "client-controls";

import {
	PhotoLog,
	Photos,
	PhotoImage,
	Activities,
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PJ.PhotoLogs.PJ.Graphs.PhotoLogEntry",
	primaryView: "PhotoLog", showUDFIndicator: true
})
export class PJ305000 extends PXScreen {
	viewPhoto: PXActionState;
	ViewEntity: PXActionState;
	ViewActivity: PXActionState;
	OpenActivityOwner: PXActionState;

	PhotoLog = createSingle(PhotoLog);
	Photos = createCollection(Photos);
	PhotoImage = createSingle(PhotoImage);
	Activities = createCollection(Activities);
}

