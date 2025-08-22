import {
	createCollection,
	createSingle,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	Filter,
	DailyFieldReports
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PJ.DailyFieldReports.PJ.Graphs.DailyFieldReportWeatherProcess",
	primaryView: "Filter"
})
export class PJ504000 extends PXScreen {
	ViewEntity: PXActionState;

	Filter = createSingle(Filter);
	DailyFieldReports = createCollection(DailyFieldReports);
}
