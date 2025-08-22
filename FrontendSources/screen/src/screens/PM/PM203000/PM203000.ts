import {
	createCollection,
	createSingle,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	Item,
	ItemSettings,
	Mapping
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.ChangeOrderClassMaint",
	primaryView: "Item"
})
export class PM203000 extends PXScreen {
	CRAttribute_ViewDetails: PXActionState;

	Item = createSingle(Item);
	ItemSettings = createSingle(ItemSettings);
	Mapping = createCollection(Mapping);
}
