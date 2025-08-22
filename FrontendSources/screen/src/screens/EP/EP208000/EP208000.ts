import {
	createCollection,
	createSingle,
	graphInfo,
	PXScreen
} from "client-controls";

import {
	Equipment,
	EquipmentProperties,
	Rates,
	Answers
} from "./views";

@graphInfo({
	graphType: "PX.Objects.EP.EquipmentMaint",
	primaryView: "Equipment"
})
export class EP208000 extends PXScreen {
	Equipment = createSingle(Equipment);
	EquipmentProperties = createSingle(EquipmentProperties);
	Rates = createCollection(Rates);
	Answers = createCollection(Answers);
}
