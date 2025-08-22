import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo,
	viewInfo
} from 'client-controls';

import {
	WeatherProcessingLogs,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PJ.DailyFieldReports.PJ.Graphs.ClearWeatherProcessingLogProcess',
	primaryView: 'WeatherProcessingLogs'
})
export class PJ504010 extends PXScreen {
	ViewEntity: PXActionState;
	WeatherProcessingLogs = createCollection(WeatherProcessingLogs);
}

