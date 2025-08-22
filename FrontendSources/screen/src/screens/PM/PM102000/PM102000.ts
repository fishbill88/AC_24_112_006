import {
	createCollection,
	createSingle,
	graphInfo,
	PXScreen
} from "client-controls";

import {
	Group,
	Users,
	ProjectGroup,
	Project,
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.PMAccess",
	primaryView: "Group"
})
export class PM102000 extends PXScreen {
	Group = createSingle(Group);
	Users = createCollection(Users);
	ProjectGroup = createCollection(ProjectGroup);
	Project = createCollection(Project);
}
