import { createCollection, createSingle, PXScreen, graphInfo, PXActionState } from "client-controls";
import { EPActivityFilter, PMTimeActivity } from "./views";

@graphInfo({graphType: "PX.Objects.EP.EmployeeActivitiesApprove", primaryView: "Filter", })
export class EP507010 extends PXScreen {
	ViewContract: PXActionState;
	ViewDetails: PXActionState;
	ViewCase: PXActionState;

	Filter = createSingle(EPActivityFilter);
	Activity = createCollection(PMTimeActivity);
}
