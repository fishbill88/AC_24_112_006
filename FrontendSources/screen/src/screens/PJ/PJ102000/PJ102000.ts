import {
	createCollection,
	createSingle,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	DrawingLogSetup,
	DrawingLogDisciplines,
	DrawingLogStatuses,
	Attributes
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PJ.DrawingLogs.PJ.Graphs.DrawingLogsSetupMaint",
	primaryView: "DrawingLogSetup"
})
export class PJ102000 extends PXScreen {
	CRAttribute_ViewDetails: PXActionState;

	DrawingLogSetup = createSingle(DrawingLogSetup);
	DrawingLogDisciplines = createCollection(DrawingLogDisciplines);
	DrawingLogStatuses = createCollection(DrawingLogStatuses);
	Attributes = createCollection(Attributes);
}
