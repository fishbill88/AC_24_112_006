import {
	graphInfo,
	createSingle,
	createCollection,
	linkCommand,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.SvrOrdTypeMaint', primaryView: 'SvrOrdTypeRecords' })
export class FS202300 extends PXScreen {
	SvrOrdTypeRecords = createSingle(FSSrvOrdType);
	CurrentSrvOrdTypeRecord = createSingle(FSSrvOrdType);
	SrvOrdTypeProblemRecords = createCollection(FSSrvOrdTypeProblem);
	QuickProcessSettings = createSingle(FSQuickProcessParameters);
	Mapping = createCollection(FSAttributeGroupList);
	NotificationRecipients = createCollection(NotificationRecipient);
	NotificationSources = createCollection(NotificationSource);
}

export class NotificationRecipient extends PXView {
	Active: PXFieldState;
	ContactType: PXFieldState;
	OriginalContactID: PXFieldState;
	ContactID: PXFieldState;
	Format: PXFieldState;
	AddTo: PXFieldState;
}

export class NotificationSource extends PXView {
	Active: PXFieldState;
	SetupID: PXFieldState;
	NBranchID: PXFieldState;
	EMailAccountID: PXFieldState;
	ReportID: PXFieldState;
	NotificationID: PXFieldState;
	Format: PXFieldState;
	RecipientsBehavior: PXFieldState;
}

export class FSAttributeGroupList extends PXView {
	IsActive: PXFieldState;
	@linkCommand("CRAttribute_ViewDetails")	AttributeID: PXFieldState;
	Description: PXFieldState;
	SortOrder: PXFieldState;
	Required: PXFieldState;
	CSAttribute__IsInternal: PXFieldState;
	ControlType: PXFieldState;
	DefaultValue: PXFieldState;
}

export class FSQuickProcessParameters extends PXView {
	CloseAppointment: PXFieldState<PXFieldOptions.CommitChanges>;
	EmailSignedAppointment: PXFieldState<PXFieldOptions.CommitChanges>;
	GenerateInvoiceFromAppointment: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowInvoiceServiceOrder: PXFieldState<PXFieldOptions.CommitChanges>;
	CompleteServiceOrder: PXFieldState<PXFieldOptions.CommitChanges>;
	CloseServiceOrder: PXFieldState<PXFieldOptions.CommitChanges>;
	GenerateInvoiceFromServiceOrder: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepareInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	SOQuickProcess: PXFieldState<PXFieldOptions.CommitChanges>;
	EmailSalesOrder: PXFieldState<PXFieldOptions.CommitChanges>;
	ReleaseInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	EmailInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	ReleaseBill: PXFieldState<PXFieldOptions.CommitChanges>;
	PayBill: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class FSSrvOrdTypeProblem extends PXView {
	ProblemID: PXFieldState<PXFieldOptions.CommitChanges>;
	FSProblem__Descr: PXFieldState;
}

export class FSSrvOrdType extends PXView {
	OnStartApptSetStartTimeInHeader: PXFieldState<PXFieldOptions.CommitChanges>;
	OnStartApptSetNotStartItemInProcess: PXFieldState<PXFieldOptions.CommitChanges>;
	OnStartApptStartUnassignedStaff: PXFieldState<PXFieldOptions.CommitChanges>;
	OnStartApptStartServiceAndStaff: PXFieldState<PXFieldOptions.CommitChanges>;
	OnCompleteApptSetEndTimeInHeader: PXFieldState<PXFieldOptions.CommitChanges>;
	OnCompleteApptSetInProcessItemsAs: PXFieldState;
	OnCompleteApptSetNotStartedItemsAs: PXFieldState;
	OnStartTimeChangeUpdateLogStartTime: PXFieldState<PXFieldOptions.CommitChanges>;
	OnEndTimeChangeUpdateLogEndTime: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowManualLogTimeEdition: PXFieldState;
	SetTimeInHeaderBasedOnLog: PXFieldState<PXFieldOptions.CommitChanges>;
	OnCompleteApptRequireLog: PXFieldState;
	SrvOrdType: PXFieldState;
	Active: PXFieldState;
	Descr: PXFieldState;
	ShowQuickProcessTab: PXFieldState<PXFieldOptions.Hidden>;
	SrvOrdNumberingID: PXFieldState;
	Behavior: PXFieldState<PXFieldOptions.CommitChanges>;
	ServiceOrderWorkflowTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	AppointmentWorkflowTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	CompleteSrvOrdWhenSrvDone: PXFieldState<PXFieldOptions.CommitChanges>;
	CloseSrvOrdWhenSrvDone: PXFieldState<PXFieldOptions.CommitChanges>;
	RequireContact: PXFieldState<PXFieldOptions.CommitChanges>;
	RequireRoom: PXFieldState;
	RequireCustomerSignature: PXFieldState;
	CopyNotesFromCustomer: PXFieldState;
	CopyAttachmentsFromCustomer: PXFieldState;
	CopyNotesFromCustomerLocation: PXFieldState;
	CopyAttachmentsFromCustomerLocation: PXFieldState;
	CopyNotesToAppoinment: PXFieldState;
	CopyAttachmentsToAppoinment: PXFieldState;
	CopyNotesToInvoice: PXFieldState;
	CopyAttachmentsToInvoice: PXFieldState;
	CopyLineNotesToInvoice: PXFieldState;
	CopyLineAttachmentsToInvoice: PXFieldState;
	OnTravelCompleteStartAppt: PXFieldState;
	AppAddressSource: PXFieldState<PXFieldOptions.CommitChanges>;
	DfltCostCodeID: PXFieldState;
	SalesPersonID: PXFieldState<PXFieldOptions.CommitChanges>;
	Commissionable: PXFieldState<PXFieldOptions.CommitChanges>;
	DfltBillableTravelItem: PXFieldState<PXFieldOptions.CommitChanges>;
	SetLotSerialNbrInAppts: PXFieldState;
	PostTo: PXFieldState<PXFieldOptions.CommitChanges>;
	PostNegBalanceToAp: PXFieldState<PXFieldOptions.CommitChanges>;
	EnableINPosting: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowQuickProcess: PXFieldState<PXFieldOptions.CommitChanges>;
	PostOrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	PostOrderTypeNegativeBalance: PXFieldState<PXFieldOptions.CommitChanges>;
	AllocationOrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	DfltTermIDARSO: PXFieldState;
	DfltTermIDAP: PXFieldState;
	SalesAcctSource: PXFieldState<PXFieldOptions.CommitChanges>;
	CombineSubFrom: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState;
	AccountGroupID: PXFieldState;
	ReasonCode: PXFieldState;
	BillingType: PXFieldState;
	ReleaseProjectTransactionOnInvoice: PXFieldState;
	ReleaseIssueOnInvoice: PXFieldState;
	AllowInvoiceOnlyClosedAppointment: PXFieldState;
	RequireTimeApprovalToInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	CreateTimeActivitiesFromAppointment: PXFieldState<PXFieldOptions.CommitChanges>;
	DfltEarningType: PXFieldState<PXFieldOptions.CommitChanges>;
}
