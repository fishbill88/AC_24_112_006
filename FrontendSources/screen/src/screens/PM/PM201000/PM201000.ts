import {
	createCollection,
	createSingle,
	graphInfo,
	PXScreen
} from "client-controls";

import {
	AccountGroup,
	AccountGroupProperties,
	Accounts,
	Answers
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.AccountGroupMaint",
	primaryView: "AccountGroup"
})
export class PM201000 extends PXScreen {
	AccountGroup = createSingle(AccountGroup);
	AccountGroupProperties = createSingle(AccountGroupProperties);
	Accounts = createCollection(Accounts);
	Answers = createCollection(Answers);
}
