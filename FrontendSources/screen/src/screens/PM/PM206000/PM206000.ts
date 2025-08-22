import {
	createCollection,
	createSingle,
	graphInfo,
	PXScreen
} from "client-controls";

import {
	RateSequence,
	RateDefinition,
	Items,
	Rates,
	Projects,
	Tasks,
	AccountGroups,
	Employees,
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.RateMaint",
	primaryView: "RateSequence"
})
export class PM206000 extends PXScreen {
	RateSequence = createSingle(RateSequence);
	RateDefinition = createSingle(RateDefinition);
	Items = createCollection(Items);
	Rates = createCollection(Rates);
	Projects = createCollection(Projects);
	Tasks = createCollection(Tasks);
	AccountGroups = createCollection(AccountGroups);
	Employees = createCollection(Employees);

	afterConstructor() {
		super.afterConstructor();
		this.screenService.registerViewBinding(this.element, RateDefinition.name);
	}
}
