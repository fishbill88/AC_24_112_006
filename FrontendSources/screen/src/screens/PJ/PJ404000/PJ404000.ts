import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo,
	viewInfo
} from 'client-controls';

import {
	Filter,
	WeatherProcessingLogs,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PJ.DailyFieldReports.PJ.Graphs.DailyFieldReportWeatherProcessingLogInquiry',
	primaryView: 'WeatherProcessingLogs'
})
export class PJ404000 extends PXScreen {
	ViewEntity: PXActionState;

	Filter = createSingle(Filter);
	WeatherProcessingLogs = createCollection(WeatherProcessingLogs);
}

