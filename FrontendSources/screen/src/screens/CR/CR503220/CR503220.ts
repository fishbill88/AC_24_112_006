import {
	PXView,
	PXFieldState,
	columnConfig,
	graphInfo,
	PXScreen,
	linkCommand,
	createCollection,
	gridConfig,
	PXFieldOptions,
	PXPageLoadBehavior,
	GridPagerMode,
	PXActionState,
	createSingle,
	GridPreset,
	GridFilterBarVisibility,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CR.UpdateCaseMassProcess",
	primaryView: "Filter",
	pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues,
})
export class CR503220 extends PXScreen {
	wizardNext: PXActionState;

	Filter = createSingle(CRWorkflowMassActionFilter);
	Items = createCollection(CRCase);
}

export class CRWorkflowMassActionFilter extends PXView {
	Operation: PXFieldState<PXFieldOptions.CommitChanges>;
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	showFilterBar: GridFilterBarVisibility.OnDemand,
	allowUpdate: false,
	pagerMode: GridPagerMode.Numeric,
	suppressNoteFiles: true,
})
export class CRCase extends PXView {
	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;

	@columnConfig({ allowCheckAll: true, width: 35 })
	Selected: PXFieldState;
	@linkCommand("Items_ViewDetails")
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
	@columnConfig({ hideViewLink: true })
	CaseClassID: PXFieldState;
	BAccount__AcctCD: PXFieldState;
	BAccount__AcctName: PXFieldState;
	@linkCommand("Items_BAccountParent_ViewDetails")
	BAccountParent__AcctCD: PXFieldState<PXFieldOptions.Hidden>;
	BAccountParent__AcctName: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("Items_Contact_ViewDetails")
	Contact__DisplayName: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("Items_Location_ViewDetails")
	Location__LocationCD: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("Items_Contract_ViewDetails")
	Contract__ContractCD: PXFieldState<PXFieldOptions.Hidden>;
	Contract__Description: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("Items_Contract_CustomerID_ViewDetails")
	Contract__CustomerID: PXFieldState<PXFieldOptions.Hidden>;
	BAccountContract__AcctName: PXFieldState<PXFieldOptions.Hidden>;
	InitResponse: PXFieldState<PXFieldOptions.Hidden>;
	TimeResolution: PXFieldState<PXFieldOptions.Hidden>;
	TimeSpent: PXFieldState<PXFieldOptions.Hidden>;
	OvertimeSpent: PXFieldState<PXFieldOptions.Hidden>;
	TimeBillable: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true })
	WorkgroupID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true })
	OwnerID: PXFieldState;
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedByID_Modifier_Username: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}
