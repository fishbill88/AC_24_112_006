import {
	createCollection,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	AllocationAuditSource
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.AllocationAudit",
	primaryView: "source"
})
export class PM403000 extends PXScreen {
	ViewAllocationRule: PXActionState;
	ViewBatch: PXActionState;

	source = createCollection(AllocationAuditSource);
}
