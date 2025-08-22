import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo
} from 'client-controls';

import {
	ProjectManagementSetup,
	ProjectIssueTypes,
	DailyFieldReportCopyConfiguration,
	WeatherIntegrationSetup,
	SubmittalTypes,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PJ.ProjectManagement.PJ.Graphs.ProjectManagementSetupMaint',
	primaryView: 'ProjectManagementSetup'
})
export class PJ101000 extends PXScreen {
	ProjectManagementSetup = createSingle(ProjectManagementSetup);
	ProjectIssueTypes = createCollection(ProjectIssueTypes);
	DailyFieldReportCopyConfiguration = createSingle(DailyFieldReportCopyConfiguration);
	WeatherIntegrationSetup = createSingle(WeatherIntegrationSetup);
	SubmittalTypes = createCollection(SubmittalTypes);
}

