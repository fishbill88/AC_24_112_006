import {
	createCollection,
	createSingle,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	ProjectIssue,
	CurrentProjectIssue,
	Activities,
	Attributes,
	LinkedDrawingLogs,
	DrawingLogs,
	DrawingLogsAttachments
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PJ.ProjectsIssue.PJ.Graphs.ProjectIssueMaint",
	primaryView: "ProjectIssue", showUDFIndicator: true
})
export class PJ302000 extends PXScreen {
	Save: PXActionState;
	LinkDrawingLogToEntity: PXActionState;
	ViewActivity: PXActionState;
	OpenActivityOwner: PXActionState;
	ViewEntity: PXActionState;
	DrawingLog$OriginalDrawingId$Link: PXActionState;
	ViewAttachment: PXActionState;
	ProjectIssue$Navigate_ByRefNote: PXActionState;
	ProjectIssue$Select_RefNote: PXActionState;
	ProjectIssue$Attach_RefNote: PXActionState;
	ProjectIssue$ConvertedTo$Link: PXActionState;

	ProjectIssue = createSingle(ProjectIssue);
	CurrentProjectIssue = createSingle(CurrentProjectIssue);
	Activities = createCollection(Activities);
	Attributes = createCollection(Attributes);
	LinkedDrawingLogs = createCollection(LinkedDrawingLogs);
	DrawingLogs = createCollection(DrawingLogs);
	DrawingLogsAttachments = createCollection(DrawingLogsAttachments);
}
