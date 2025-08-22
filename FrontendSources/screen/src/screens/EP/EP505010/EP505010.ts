import {
	createCollection,
	graphInfo,
	PXScreen
} from "client-controls";

import {
	FilteredItems
} from "./views";

@graphInfo({
	graphType: "PX.Objects.EP.TimeCardRelease",
	primaryView: "FilteredItems"
})
export class EP505010 extends PXScreen {
	FilteredItems = createCollection(FilteredItems);
}
