import {
	createCollection,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	Items
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.CommitmentInquiry",
	primaryView: "Filter"
})
export class PM306000 extends PXScreen {
	ViewVendor: PXActionState;
	ViewProject: PXActionState;
	ViewExternalCommitment: PXActionState;
	PMCommitment$RefNoteID$Link: PXActionState;
	PMCommitmentAlias$RefNoteID$Link: PXActionState;

	Items = createCollection(Items);
}
