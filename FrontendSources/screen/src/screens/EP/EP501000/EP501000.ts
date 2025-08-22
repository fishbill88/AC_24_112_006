import {
	createCollection,
	graphInfo,
	PXScreen
} from "client-controls";

import {
	EPDocumentList
} from "./views";

@graphInfo({
	graphType: "PX.Objects.EP.EPDocumentRelease",
	primaryView: "EPDocumentList"
})
export class EP501000 extends PXScreen {
	EPDocumentList = createCollection(EPDocumentList);
}
