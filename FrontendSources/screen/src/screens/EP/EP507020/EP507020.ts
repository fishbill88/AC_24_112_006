import { createCollection, createSingle, PXScreen, graphInfo, PXActionState } from "client-controls";
import { OwnedFilter, PMTimeActivity } from "./views";

@graphInfo({graphType: "PX.Objects.EP.EmployeeActivitiesRelease", primaryView: "Filter", })
export class EP507020 extends PXScreen {
	ViewContract: PXActionState;
	ViewDetails: PXActionState;
	ViewCase: PXActionState;

	Filter = createSingle(OwnedFilter);
	Activity = createCollection(PMTimeActivity);
}
