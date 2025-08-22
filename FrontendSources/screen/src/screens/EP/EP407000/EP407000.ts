import {
	createCollection,
	graphInfo,
	PXScreen
} from "client-controls";

import {
	Items
} from "./views";

@graphInfo({
	graphType: "PX.Objects.EP.EquipmentTimecardPrimary",
	primaryView: "Items"
})
export class EP407000 extends PXScreen {
	Items = createCollection(Items);
}
