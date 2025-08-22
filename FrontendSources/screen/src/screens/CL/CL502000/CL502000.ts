import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	PXPageLoadBehavior
} from "client-controls";
import { ProcessLienWaiversFilter, ComplianceDocument } from "./views";

@graphInfo({
	graphType: "PX.Objects.CN.Compliance.CL.Graphs.PrintEmailLienWaiversProcess",
	primaryView: "Filter",
	pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues
})
export class CL502000 extends PXScreen {
	@viewInfo({ containerName: "Selection" })
	Filter = createSingle(ProcessLienWaiversFilter);
	LienWaivers = createCollection(ComplianceDocument);
}
