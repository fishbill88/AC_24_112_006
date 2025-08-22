import {
	createCollection,
	createSingle,
	graphInfo,
	PXScreen
} from "client-controls";

import {
	Group,
	Users,
	AccountGroup,
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.PMAccountGroupAccess",
	primaryView: "Group"
})
export class PM103000 extends PXScreen {
	Group = createSingle(Group);
	Users = createCollection(Users);
	AccountGroup = createCollection(AccountGroup);
}
