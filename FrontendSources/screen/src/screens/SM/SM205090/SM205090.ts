import {
	PXScreen,
	PXView,
	PXFieldState,
	graphInfo,
	createSingle,
	createCollection,
	gridConfig,
	viewInfo,
	columnConfig,
	GridFilterBarVisibility
} from "client-controls";

@graphInfo({graphType: 'PX.CloudServices.Diagnostic.CloudServiceDiagnosticsMaint', primaryView: 'ServiceInformationFilter'})
export class SM205090 extends PXScreen {
	@viewInfo({containerName: 'Cloud Service Information'})
	ServiceInformationFilter = createSingle(ServiceInformation);

	@viewInfo({containerName: 'Results'})
	DiagnosticResults = createCollection(CloudServiceDiagnosticResult);
}

export class ServiceInformation extends PXView {
	DiscoveryUri: PXFieldState;
	CloudTenantID: PXFieldState;
	ClientId: PXFieldState;
	ClientSecret: PXFieldState;
}

@gridConfig({allowDelete: false, allowInsert: false, autoAdjustColumns: true, showFilterBar: GridFilterBarVisibility.False,
	actionsConfig: {
		refresh: { hidden: true },
		adjust: { hidden: true },
		exportToExcel: { hidden: true }
	}})
export class CloudServiceDiagnosticResult extends PXView {
	@columnConfig({width: 50})
	Enabled: PXFieldState;

	@columnConfig({width: 150})
	ServiceName: PXFieldState;

	@columnConfig({width: 150})
	StepName: PXFieldState;

	@columnConfig({width: 150})
	Status: PXFieldState;

	@columnConfig({width: 300})
	Message: PXFieldState;
}
