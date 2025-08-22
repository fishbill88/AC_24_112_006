import {
	columnConfig,
	gridConfig,
	linkCommand,
	PXActionState,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class DailyFieldReport extends PXView {
	DailyFieldReportCd: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	Hold: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Hidden|PXFieldOptions.NoLabel>;
	Date: PXFieldState;
	ProjectId: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectManagerId: PXFieldState<PXFieldOptions.CommitChanges>;
	CreatedById: PXFieldState;
	SiteAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	City: PXFieldState<PXFieldOptions.CommitChanges>;
	CountryId: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	Latitude: PXFieldState<PXFieldOptions.CommitChanges>;
	Longitude: PXFieldState<PXFieldOptions.CommitChanges>;
	TemperatureLevel: PXFieldState;
	Humidity: PXFieldState;
	Icon: PXFieldState;
	TimeObserved_Time: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true,
	initNewRow: true
})
export class EmployeeActivities extends PXView {
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID_description: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Hidden>;
	WorkgroupID: PXFieldState;
	EarningTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Date_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	TimeSpent: PXFieldState<PXFieldOptions.CommitChanges>;
	IsBillable: PXFieldState<PXFieldOptions.CommitChanges>;
	TimeBillable: PXFieldState;
	Summary: PXFieldState<PXFieldOptions.CommitChanges>;
	LastModifiedById: PXFieldState<PXFieldOptions.CommitChanges>;
	LastModifiedDateTime: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewTimeCard")
	TimeCardCD: PXFieldState;
	Hold: PXFieldState<PXFieldOptions.CommitChanges>;
	ApprovalStatus: PXFieldState;
	Date_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	ParentTaskNoteID: PXFieldState<PXFieldOptions.CommitChanges>;
	CertifiedJob: PXFieldState;
	UnionID: PXFieldState<PXFieldOptions.CommitChanges>;
	LabourItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShiftID: PXFieldState<PXFieldOptions.CommitChanges>;
	ApproverID: PXFieldState;
	ContractID: PXFieldState<PXFieldOptions.CommitChanges>;
	RefNoteID: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	initNewRow: true
})
export class ProgressWorksheetLines extends PXView {
	LoadTemplate: PXActionState;
	SelectBudgetLines: PXActionState;

	@linkCommand("ViewProgressWorksheet")
	PMProgressWorksheet__HiddenRefNbr: PXFieldState;
	PMProgressWorksheet__HiddenStatus: PXFieldState;
	OwnerID: PXFieldState;
	WorkgroupID: PXFieldState;
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	UOM: PXFieldState;
	PreviouslyCompletedQuantity: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	PriorPeriodQuantity: PXFieldState;
	CurrentPeriodQuantity: PXFieldState;
	TotalCompletedQuantity: PXFieldState;
	CompletedPercentTotalQuantity: PXFieldState;
	TotalBudgetedQuantity: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false
})
export class ChangeRequests extends PXView {
	CreateChangeRequest: PXActionState;

	@linkCommand("ViewChangeRequest")
	ChangeRequestId: PXFieldState<PXFieldOptions.CommitChanges>;
	PMChangeRequest__Date: PXFieldState;
	PMChangeRequest__ExtRefNbr: PXFieldState;
	PMChangeRequest__Description: PXFieldState;
	PMChangeRequest__Status: PXFieldState;
	PMChangeRequest__CostTotal: PXFieldState;
	PMChangeRequest__LineTotal: PXFieldState;
	PMChangeRequest__MarkupTotal: PXFieldState;
	PMChangeRequest__PriceTotal: PXFieldState;
	PMChangeRequest__LastModifiedByID: PXFieldState;
	PMChangeRequest__LastModifiedDateTime: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false
})
export class ChangeOrders extends PXView {
	CreateChangeOrder: PXActionState;

	@linkCommand("ViewChangeOrder")
	ChangeOrderId: PXFieldState<PXFieldOptions.CommitChanges>;
	PMChangeOrder__ClassID: PXFieldState;
	PMChangeOrder__CustomerID: PXFieldState;
	Customer__AcctName: PXFieldState;
	PMChangeOrder__DelayDays: PXFieldState;
	PMChangeOrder__ProjectNbr: PXFieldState;
	PMChangeOrder__ExtRefNbr: PXFieldState;
	PMChangeOrder__Description: PXFieldState;
	PMChangeOrder__Status: PXFieldState;
	PMChangeOrder__RevenueTotal: PXFieldState;
	PMChangeOrder__CommitmentTotal: PXFieldState;
	PMChangeOrder__CostTotal: PXFieldState;
	PMChangeOrder__LastModifiedByID: PXFieldState;
	PMChangeOrder__LastModifiedDateTime: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	initNewRow: true
})
export class Subcontractors extends PXView {
	@linkCommand("ViewVendor")
	VendorId: PXFieldState<PXFieldOptions.CommitChanges>;
	Vendor__AcctName: PXFieldState;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeId: PXFieldState<PXFieldOptions.CommitChanges>;
	NumberOfWorkers: PXFieldState<PXFieldOptions.CommitChanges>;
	TimeArrived_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	TimeDeparted_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkingTimeSpent: PXFieldState<PXFieldOptions.CommitChanges>;
	TotalWorkingTimeSpent: PXFieldState;
	Description: PXFieldState<PXFieldOptions.CommitChanges>;
	LastModifiedById: PXFieldState;
	LastModifiedDateTime: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false
})
export class ProjectIssues extends PXView {
	CreateProjectIssue: PXActionState;

	@linkCommand("ViewProjectIssue")
	ProjectIssueId: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectIssue__Summary: PXFieldState;
	ProjectIssue__Status: PXFieldState;
	ProjectIssue__PriorityId: PXFieldState;
	ProjectIssue__ProjectTaskId: PXFieldState;
	ProjectIssue__ProjectIssueTypeId: PXFieldState;
	ProjectIssue__LastModifiedByID: PXFieldState;
	ProjectIssue__LastModifiedDateTime: PXFieldState;
	ProjectIssue__CreationDate: PXFieldState<PXFieldOptions.Hidden>;
	ProjectIssue__DueDate: PXFieldState<PXFieldOptions.Hidden>;
	ProjectIssue__OwnerID: PXFieldState<PXFieldOptions.Hidden>;
	ProjectIssue__WorkgroupID: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false
})
export class PhotoLogs extends PXView {
	CreatePhotoLog: PXActionState;

	@linkCommand("ViewPhotoLog")
	PhotoLogId: PXFieldState<PXFieldOptions.CommitChanges>;
	PhotoLog__StatusId: PXFieldState;
	PhotoLog__Date: PXFieldState;
	PhotoLog__ProjectTaskId: PXFieldState;
	PhotoLog__Description: PXFieldState;
	PhotoLog__CreatedById: PXFieldState;
	PhotoLog__LastModifiedByID: PXFieldState;
	PhotoLog__LastModifiedDateTime: PXFieldState;
}

export class MainPhoto extends PXView {
	ImageUrl: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	initNewRow: true
})
export class Notes extends PXView {
	Time_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	LastModifiedById: PXFieldState;
	LastModifiedDateTime: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false
})
export class PurchaseReceipts extends PXView {
	CreateNewPurchaseReceipt: PXActionState;
	CreateNewPurchaseReturn: PXActionState;

	@linkCommand("ViewPurchaseReceipt")
	PurchaseReceiptId: PXFieldState<PXFieldOptions.CommitChanges>;
	POReceipt__ReceiptType: PXFieldState;
	POReceipt__Status: PXFieldState;
	POReceipt__VendorID: PXFieldState;
	Vendor__AcctName: PXFieldState;
	POReceipt__OrderQty: PXFieldState;
	POReceipt__LastModifiedByID: PXFieldState;
	POReceipt__LastModifiedDateTime: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	initNewRow: true
})
export class Equipment extends PXView {
	EquipmentId: PXFieldState<PXFieldOptions.CommitChanges>;
	EquipmentDescription: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	SetupTime: PXFieldState<PXFieldOptions.CommitChanges>;
	RunTime: PXFieldState<PXFieldOptions.CommitChanges>;
	SuspendTime: PXFieldState<PXFieldOptions.CommitChanges>;
	IsBillable: PXFieldState;
	Description: PXFieldState;
	LastModifiedByID: PXFieldState;
	LastModifiedDateTime: PXFieldState;
	@linkCommand("ViewEquipmentTimeCard")
	EquipmentTimeCardCd: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	initNewRow: true
})
export class Weather extends PXView {
	LoadWeatherConditions: PXActionState;

	TimeObserved_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	Cloudiness: PXFieldState;
	SkyState: PXFieldState;
	TemperatureLevel: PXFieldState;
	TemperatureLevelMobile: PXFieldState;
	Temperature: PXFieldState;
	Humidity: PXFieldState;
	PrecipitationAmount: PXFieldState;
	PrecipitationAmountMobile: PXFieldState;
	Precipitation: PXFieldState;
	WindSpeed: PXFieldState;
	WindSpeedMobile: PXFieldState;
	WindPower: PXFieldState;
	LocationCondition: PXFieldState;
	IsObservationDelayed: PXFieldState;
	Description: PXFieldState;
	LastModifiedById: PXFieldState;
	LastModifiedDateTime: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	initNewRow: true
})
export class Visitors extends PXView {
	VisitorType: PXFieldState;
	VisitorName: PXFieldState;
	@linkCommand("ViewBAccount")
	BusinessAccountId: PXFieldState<PXFieldOptions.CommitChanges>;
	Company: PXFieldState;
	TimeArrived_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	TimeDeparted_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	PurposeOfVisit: PXFieldState;
	AreaVisited: PXFieldState;
	Description: PXFieldState;
	LastModifiedById: PXFieldState;
	LastModifiedDateTime: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false
})
export class EmployeeExpenses extends PXView {
	CreateExpenseReceipt: PXActionState;

	@linkCommand("ViewExpenseReceipt")
	EmployeeExpenseId: PXFieldState<PXFieldOptions.CommitChanges>;
	EPExpenseClaimDetails__TaskID: PXFieldState;
	EPExpenseClaimDetails__CostCodeID: PXFieldState;
	EPExpenseClaimDetails__Status: PXFieldState;
	EPExpenseClaimDetails__TranDesc: PXFieldState;
	EPExpenseClaimDetails__ExpenseRefNbr: PXFieldState;
	EPExpenseClaimDetails__CuryTranAmtWithTaxes: PXFieldState;
	EPExpenseClaimDetails__CuryID: PXFieldState;
	EPExpenseClaimDetails__EmployeeID: PXFieldState;
	@linkCommand("ViewExpenseClaim")
	EPExpenseClaimDetails__RefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	EPExpenseClaimDetails__LastModifiedByID: PXFieldState;
	EPExpenseClaimDetails__LastModifiedDateTime: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class Approval extends PXView {
	ApproverEmployee__AcctCD: PXFieldState;
	ApproverEmployee__AcctName: PXFieldState;
	WorkgroupId: PXFieldState;
	ApprovedByEmployee__AcctCD: PXFieldState;
	ApprovedByEmployee__AcctName: PXFieldState;
	OrigOwnerID: PXFieldState<PXFieldOptions.Hidden>;
	ApproveDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Status: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Reason: PXFieldState;
	AssignmentMapId: PXFieldState<PXFieldOptions.Hidden>;
	RuleId: PXFieldState<PXFieldOptions.Hidden>;
	StepId: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	suppressNoteFiles: true,
	allowInsert: false,
	allowDelete: false
})
export class History extends PXView {
	@linkCommand("ViewAttachedReport")
	FileName: PXFieldState<PXFieldOptions.CommitChanges>;
	Comment: PXFieldState;
	CreatedDateTime: PXFieldState;
	CreatedById: PXFieldState;
}

export class costBudgetfilter extends PXView {
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeFrom: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeTo: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class CostBudgets extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	ProjectTaskID: PXFieldState;
	InventoryID: PXFieldState;
	CostCodeID: PXFieldState;
	AccountGroupID: PXFieldState;
	Description: PXFieldState;
	UOM: PXFieldState;
	RevisedQty: PXFieldState;
}

export class ReasonApproveRejectParams extends PXView {
	Reason: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Multiline>;
}

export class ReassignApprovalFilter extends PXView {
	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;
}
