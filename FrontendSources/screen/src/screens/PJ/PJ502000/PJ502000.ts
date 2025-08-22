import {
	createCollection,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	ProjectIssues
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PJ.ProjectsIssue.PJ.Graphs.AssignProjectIssueMassProcess",
	primaryView: "ProjectIssues"
})
export class PJ502000 extends PXScreen {
	ProjectIssues_ViewDetails: PXActionState;
	ProjectIssues = createCollection(ProjectIssues);
}
