import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	columnConfig,
	GridPreset
} from "client-controls";

@graphInfo({graphType: "PX.SM.SMPrintJobMaint", primaryView: "Filter"})
export class SM206500 extends PXScreen {

	@viewInfo({containerName: "Print Job Selection"})
	Filter = createSingle(SMPrintJobFilter);
	@viewInfo({containerName: "Print Jobs"})
	Job = createCollection(SMPrintJob);
   	@viewInfo({containerName: "Print Job Parameters"})
	PrintJobParameters = createCollection(SMPrintJobParameter);
}

export class SMPrintJobFilter extends PXView  {
	StartDate : PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate : PXFieldState<PXFieldOptions.CommitChanges>;
	HideProcessed : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details
})
export class SMPrintJob extends PXView  {
	@columnConfig({allowCheckAll: true})
	Selected : PXFieldState;
	JobID : PXFieldState;
	Description : PXFieldState;
	ReportID : PXFieldState;
	DeviceHubID : PXFieldState;
	PrinterName : PXFieldState;
	NumberOfCopies : PXFieldState;
	CreatedByID : PXFieldState;
	CreatedDateTime : PXFieldState;
	Status : PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false
})
export class SMPrintJobParameter extends PXView  {
	ParameterName : PXFieldState;
	Parametervalue : PXFieldState;
}