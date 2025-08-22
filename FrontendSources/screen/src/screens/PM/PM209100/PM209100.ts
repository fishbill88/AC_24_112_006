import {
	createSingle,
	graphInfo,
	PXScreen
} from "client-controls";

import { Items } from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.TimeEntry",
	primaryView: "Items"
})
export class PM209100 extends PXScreen {
	Items = createSingle(Items);
}
