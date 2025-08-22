import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, PXFieldOptions, columnConfig, createCollection,
	linkCommand, PXActionState, GridColumnShowHideMode, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AP.APSetupMaint', primaryView: 'Setup' })
export class AP101000 extends PXScreen {

	ViewAssignmentMap: PXActionState;

	Setup = createSingle(APSetup);
	SetupApproval = createCollection(APSetupApproval);
	Boxes1099 = createCollection(AP1099Box);
	Notifications = createCollection(APNotification);
	Recipients = createCollection(NotificationSetupRecipient);

}

export class APSetup extends PXView {

	// Numbering Settings
	BatchNumberingID: PXFieldState;
	InvoiceNumberingID: PXFieldState;
	DebitAdjNumberingID: PXFieldState;
	CreditAdjNumberingID: PXFieldState;
	CheckNumberingID: PXFieldState;
	PriceWSNumberingID: PXFieldState;

	// Posting Settings
	AutoPost: PXFieldState;
	SummaryPost: PXFieldState;
	MigrationMode: PXFieldState;
	ReclassifyInvoices: PXFieldState<PXFieldOptions.CommitChanges>;

	// Aging Settings
	PastDue00: PXFieldState;
	PastDue01: PXFieldState;
	PastDue02: PXFieldState;

	// Data Entry Settings
	DfltVendorClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseSubMask: PXFieldState;
	IntercompanyExpenseAccountDefault: PXFieldState;
	InvoiceRounding: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoicePrecision: PXFieldState;

	@columnConfig({ allowNull: false })
	PaymentLeadTime: PXFieldState;

	HoldEntry: PXFieldState<PXFieldOptions.CommitChanges>;
	RequireApprovePayments: PXFieldState;
	EarlyChecks: PXFieldState;
	RequireControlTotal: PXFieldState;
	RequireControlTaxTotal: PXFieldState;
	SuggestPaymentAmount: PXFieldState;
	RequireVendorRef: PXFieldState;
	RaiseErrorOnDoubleInvoiceNbr: PXFieldState;
	RequireSingleProjectPerDocument: PXFieldState;
	TermsInDebitAdjustments: PXFieldState;

	// Retainage Settings
	RetainTaxes: PXFieldState;
	RetainageBillsAutoRelease: PXFieldState;

	// VAT Recalculation Settings
	PPDDebitAdjustmentDescr: PXFieldState;

	// Pricing
	@columnConfig({ allowNull: false })
	VendorPriceUpdate: PXFieldState;

	LoadVendorsPricesUsingAlternateID: PXFieldState;
	RetentionType: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowNull: false })
	NumberOfMonths: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowNull: false })
	ApplyQuantityDiscountBy: PXFieldState;

	// 1099
	PrintDirectSalesOn: PXFieldState;

}

export class APSetupApproval extends PXView {

	IsActive: PXFieldState;
	DocType: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("ViewAssignmentMap")
	AssignmentMapID_EPAssignmentMap_Name: PXFieldState<PXFieldOptions.CommitChanges>;

	AssignmentNotificationID: PXFieldState;

}

export class AP1099Box extends PXView {

	@columnConfig({ hideViewLink: true })
	BoxNbr: PXFieldState;
	Descr: PXFieldState;
	MinReportAmt: PXFieldState;
	AccountID: PXFieldState;

}

@gridConfig({ syncPosition: true, autoRepaint: ["Recipients"] })
export class APNotification extends PXView {

	Active: PXFieldState;
	NotificationCD: PXFieldState;
	NBranchID: PXFieldState;
	EMailAccountID: PXFieldState;
	DefaultPrinterID: PXFieldState;
	ReportID: PXFieldState;
	NotificationID: PXFieldState;
	Format: PXFieldState;
	RecipientsBehavior: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class NotificationSetupRecipient extends PXView {

	Active: PXFieldState;
	ContactType: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	OriginalContactID: PXFieldState;

	ContactID: PXFieldState;
	Format: PXFieldState;
	AddTo: PXFieldState;

}
