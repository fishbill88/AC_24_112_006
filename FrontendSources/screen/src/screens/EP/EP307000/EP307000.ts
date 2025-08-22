import { createCollection, createSingle, PXScreen, graphInfo } from "client-controls";
import { OwnedFilter, PMTimeActivity } from "./views";

@graphInfo({graphType: "PX.Objects.EP.EmployeeActivitiesEntry", primaryView: "Filter" })
export class EP307000 extends PXScreen {
	Filter = createSingle(OwnedFilter);
	Activity = createCollection(PMTimeActivity);
}
