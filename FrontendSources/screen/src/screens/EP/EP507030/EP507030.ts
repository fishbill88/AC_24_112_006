import { createCollection, createSingle, PXScreen, graphInfo } from "client-controls";
import { EPSummaryFilter, EPSummaryApprove } from "./views";

@graphInfo({graphType: "PX.Objects.EP.EmployeeSummaryApprove", primaryView: "Filter", })
export class EP507030 extends PXScreen {

	Filter = createSingle(EPSummaryFilter);
	Summary = createCollection(EPSummaryApprove);
}
