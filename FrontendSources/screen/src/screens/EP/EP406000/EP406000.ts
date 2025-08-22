import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXPageLoadBehavior } from "client-controls";
import { TimecardFilter, TimecardWithTotals } from "./views";

@graphInfo({
	graphType: "PX.Objects.EP.TimecardPrimary",
	primaryView: "Filter",
	pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues
})
export class EP406000 extends PXScreen {
   	@viewInfo({ containerName: "Selection" })
	Filter = createSingle(TimecardFilter);
   	@viewInfo({ containerName: "Time Cards" })
	Items = createCollection(TimecardWithTotals);
}
