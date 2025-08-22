import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo
} from "client-controls";

import {
	DrawingLog,
	Drawings,
	Attributes,
	Activities,
	Revisions,
	LinkedDrawingLogRelations,
	UnlinkedDrawingLogRelations,
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PJ.DrawingLogs.PJ.Graphs.DrawingLogEntry",
	primaryView: "DrawingLog", showUDFIndicator: true
})
export class PJ303000 extends PXScreen {
	DrawingLog$OriginalDrawingId$Link: PXActionState;
	LinkEntity: PXActionState;
	ViewAttachment: PXActionState;
	ViewActivity: PXActionState;
	OpenActivityOwner: PXActionState;
	ViewEntity: PXActionState;
	LinkedDrawingLogRelation$DocumentId$Link: PXActionState;

	DrawingLog = createSingle(DrawingLog);
	Drawings = createCollection(Drawings);
	Attributes = createCollection(Attributes);
	Activities = createCollection(Activities);
	Revisions = createCollection(Revisions);
	LinkedDrawingLogRelations = createCollection(LinkedDrawingLogRelations);
	UnlinkedDrawingLogRelations = createCollection(UnlinkedDrawingLogRelations);
}

