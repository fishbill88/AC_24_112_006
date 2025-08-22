import {
	createSingle,
	graphInfo,
	PXScreen
} from "client-controls";

import {
	Commitments
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.ExternalCommitmentEntry",
	primaryView: "Commitments"
})
export class PM209000 extends PXScreen {
	Commitments = createSingle(Commitments);
}
