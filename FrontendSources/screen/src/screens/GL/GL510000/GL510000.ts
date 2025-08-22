import {
	createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig
} from "client-controls";

@graphInfo({graphType: "PX.GLAnomalyDetection.SendTransactionsProcess", primaryView: "Filter" })
export class GL510000 extends PXScreen {

	Filter = createSingle(SendTransactionsProcessParameters);
	Organizations = createCollection(Organization);
}

export class SendTransactionsProcessParameters extends PXView {
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ mergeToolbarWith: "ScreenToolbar" })
export class Organization extends PXView {

	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;

	@columnConfig({ hideViewLink: true })
	OrganizationCD: PXFieldState;
	OrganizationName: PXFieldState;

	@columnConfig({ hideViewLink: true })
	FromPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	ToPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;

	ServiceStatus: PXFieldState;
}
