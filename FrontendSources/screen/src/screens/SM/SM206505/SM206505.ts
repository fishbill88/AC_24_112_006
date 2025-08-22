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
	GridPreset
} from "client-controls";

@graphInfo({graphType: "PX.SM.SMScanJobMaint", primaryView: "ScanJob"})
export class SM206505 extends PXScreen {

	Filter = createSingle(SMScanJobFilter);
   	@viewInfo({containerName: "Scan Jobs"})
	ScanJob = createCollection(SMScanJob);
   	@viewInfo({containerName: "Document Parameters"})
	Parameters = createCollection(SMScanJobParameter);
}

export class SMScanJobFilter extends PXView  {
	StartDate : PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate : PXFieldState<PXFieldOptions.CommitChanges>;
	HideProcessed : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	autoRepaint: ['Parameters']
})
export class SMScanJob extends PXView  {
	ScanJobID : PXFieldState;
	DeviceHubID : PXFieldState;
	ScannerName : PXFieldState;
	ScannerName_SMScanner_description : PXFieldState;
	Status : PXFieldState;
	EntityScreenID : PXFieldState;
	PaperSource : PXFieldState;
	PixelType : PXFieldState;
	Resolution : PXFieldState;
	FileType : PXFieldState;
	FileName : PXFieldState;
	Error : PXFieldState;
	CreatedByID : PXFieldState;
	CreatedDateTime : PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry
})
export class SMScanJobParameter extends PXView  {
	LineNbr : PXFieldState;
	ParameterName : PXFieldState;
	ParameterValue : PXFieldState;
}