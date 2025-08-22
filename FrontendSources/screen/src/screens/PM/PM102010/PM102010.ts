import {
	createCollection,
	createSingle,
	graphInfo,
	PXScreen
} from "client-controls";

import {
	Project,
	Groups,
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.PMAccessDetail",
	primaryView: "Project"
})
export class PM102010 extends PXScreen {
	Project = createSingle(Project);
	Groups = createCollection(Groups);
}
