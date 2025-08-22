import {
	createCollection,
	createSingle,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	ProjectManagementClasses,
	ProjectManagementClassesCurrent,
	Attributes,
	ProjectManagementClassPriority
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PJ.ProjectManagement.PJ.Graphs.ProjectManagementClassMaint",
	primaryView: "ProjectManagementClasses"
})
export class PJ201000 extends PXScreen {
	CRAttribute_ViewDetails: PXActionState;

	ProjectManagementClasses = createSingle(ProjectManagementClasses);
	ProjectManagementClassesCurrent = createSingle(ProjectManagementClassesCurrent);
	Attributes = createCollection(Attributes);
	ProjectManagementClassPriority = createCollection(ProjectManagementClassPriority);
}
