import {
	PXScreen,
	createSingle,
	createCollection,
	graphInfo
} from "client-controls";

import {
	Task,
	TaskProperties,
	Budget,
	BillingItems,
	Answers,
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.TemplateGlobalTaskMaint",
	primaryView: "Task", showUDFIndicator: true
})
export class PM208030 extends PXScreen {
	Task = createSingle(Task);
	TaskProperties = createSingle(TaskProperties);
	Budget = createCollection(Budget);
	BillingItems = createCollection(BillingItems);
	Answers = createCollection(Answers);
}

