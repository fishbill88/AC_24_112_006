import {
	PXScreen,
	createSingle,
	createCollection,
	graphInfo
} from "client-controls";

import {
	Task,
	TaskProperties,
	BillingItems,
	Answers,
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.TemplateTaskMaint",
	primaryView: "Task"
})
export class PM208010 extends PXScreen {
	Task = createSingle(Task);
	TaskProperties = createSingle(TaskProperties);
	BillingItems = createCollection(BillingItems);
	Answers = createCollection(Answers);
}

