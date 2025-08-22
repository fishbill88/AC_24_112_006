import {
	createCollection,
	createSingle,
	graphInfo,
	PXScreen
} from "client-controls";

import {
	ProjectGroup,
	Groups,
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.PMAccessByProjectGroupMaint",
	primaryView: "ProjectGroup"
})
export class PM102020 extends PXScreen {
	ProjectGroup = createSingle(ProjectGroup);
	Groups = createCollection(Groups);
}
