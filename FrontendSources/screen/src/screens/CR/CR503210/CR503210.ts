import {
	createCollection,
	PXScreen,
	PXView,
	graphInfo,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	columnConfig,
	linkCommand,
	GridPreset,
	GridFilterBarVisibility,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CR.AssignCaseMassProcess",
	primaryView: "Items",
})
export class CR503210 extends PXScreen {
	Items = createCollection(CRCase);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	showFilterBar: GridFilterBarVisibility.OnDemand,
	suppressNoteFiles: true,
})
export class CRCase extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	CaseCD: PXFieldState;
	Subject: PXFieldState;
	Status: PXFieldState;
	Resolution: PXFieldState;
	Severity: PXFieldState;
	Priority: PXFieldState;
	ETA: PXFieldState<PXFieldOptions.Hidden>;
	TimeEstimated: PXFieldState<PXFieldOptions.Hidden>;
	RemaininingDate: PXFieldState<PXFieldOptions.Hidden>;
	Age: PXFieldState<PXFieldOptions.Hidden>;
	CRActivityStatistics__LastIncomingActivityDate: PXFieldState<PXFieldOptions.Hidden>;
	CRActivityStatistics__LastOutgoingActivityDate: PXFieldState<PXFieldOptions.Hidden>;
	LastActivity: PXFieldState<PXFieldOptions.Hidden>;
	LastActivityAge: PXFieldState<PXFieldOptions.Hidden>;
	LastModified: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true }) CaseClassID: PXFieldState;
	BAccount__AcctCD: PXFieldState;
	BAccount__AcctName: PXFieldState;
	BAccountParent__AcctCD: PXFieldState<PXFieldOptions.Hidden>;
	BAccountParent__AcctName: PXFieldState<PXFieldOptions.Hidden>;
	Contact__DisplayName: PXFieldState<PXFieldOptions.Hidden>;
	Location__LocationCD: PXFieldState<PXFieldOptions.Hidden>;
	Contract__ContractCD: PXFieldState<PXFieldOptions.Hidden>;
	Contract__Description: PXFieldState<PXFieldOptions.Hidden>;
	Contract__CustomerID: PXFieldState<PXFieldOptions.Hidden>;
	BAccountContract__AcctName: PXFieldState<PXFieldOptions.Hidden>;
	InitResponse: PXFieldState<PXFieldOptions.Hidden>;
	TimeResolution: PXFieldState<PXFieldOptions.Hidden>;
	TimeSpent: PXFieldState<PXFieldOptions.Hidden>;
	OvertimeSpent: PXFieldState<PXFieldOptions.Hidden>;
	TimeBillable: PXFieldState<PXFieldOptions.Hidden>;
	OvertimeBillable: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true })
	WorkgroupID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true }) OwnerID: PXFieldState;
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedByID_Modifier_Username: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}
