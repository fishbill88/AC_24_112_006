import {
	createCollection,
	createSingle,
	graphInfo,
	PXScreen
} from "client-controls";

import {
	AccountGroup,
	Groups,
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.PMAccountGroupAccessDetail",
	primaryView: "AccountGroup"
})
export class PM103010 extends PXScreen {
	AccountGroup = createSingle(AccountGroup);
	Groups = createCollection(Groups);
}
