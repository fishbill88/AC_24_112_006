import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, PXFieldOptions, createCollection, columnConfig, linkCommand, PXActionState, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AR.ARSetupMaint', primaryView: 'ARSetupRecord' })
export class AR101000 extends PXScreen {
	ARSetupRecord = createSingle(ARSetup);
	SetupApproval = createCollection(ARSetupApproval);
	DunningSetup = createCollection(ARDunningSetup);
	Notifications = createCollection(ARNotification);
	Recipients = createCollection(NotificationSetupRecipient);

	ViewAssignmentMap: PXActionState;

}

export class ARSetup extends PXView {
	BatchNumberingID: PXFieldState;
	InvoiceNumberingID: PXFieldState;
	PaymentNumberingID: PXFieldState;
	DebitAdjNumberingID: PXFieldState;
	CreditAdjNumberingID: PXFieldState;
	WriteOffNumberingID: PXFieldState;
	FinChargeNumberingID: PXFieldState;
	PriceWSNumberingID: PXFieldState;
	DunningFeeNumberingID: PXFieldState;
	AutoPost: PXFieldState;
	SummaryPost: PXFieldState;
	MigrationMode: PXFieldState<PXFieldOptions.CommitChanges>;
	DfltCustomerClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesSubMask: PXFieldState;
	IntercompanySalesAccountDefault: PXFieldState;
	InvoiceRounding: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoicePrecision: PXFieldState;
	HoldEntry: PXFieldState;
	RequireControlTotal: PXFieldState;
	RequireExtRef: PXFieldState;
	CreditCheckError: PXFieldState;
	PrintBeforeRelease: PXFieldState;
	EmailBeforeRelease: PXFieldState;
	TermsInCreditMemos: PXFieldState;
	IntegratedCCProcessing: PXFieldState;
	AgeCredits: PXFieldState;
	DefFinChargeFromCycle: PXFieldState;
	FinChargeOnCharge: PXFieldState;
	FinChargeFirst: PXFieldState;
	SPCommnCalcType: PXFieldState;
	SPCommnPeriodType: PXFieldState;
	PrepareStatements: PXFieldState<PXFieldOptions.CommitChanges>;
	StatementBranchID: PXFieldState;
	PrepareDunningLetters: PXFieldState<PXFieldOptions.CommitChanges>;
	DunningLetterBranchID: PXFieldState;
	BalanceWriteOff: PXFieldState;
	CreditWriteOff: PXFieldState;
	AutoReleasePPDCreditMemo: PXFieldState;
	PPDCreditMemoDescr: PXFieldState;
	RetainTaxes: PXFieldState;
	RetainageInvoicesAutoRelease: PXFieldState;

	DefaultRateTypeID: PXFieldState;
	AlwaysFromBaseCury: PXFieldState;
	LoadSalesPricesUsingAlternateID: PXFieldState;
	RetentionType: PXFieldState<PXFieldOptions.CommitChanges>;
	NumberOfMonths: PXFieldState<PXFieldOptions.CommitChanges>;
	LineDiscountTarget: PXFieldState;
	ApplyQuantityDiscountBy: PXFieldState;

	DunningLetterProcessType: PXFieldState<PXFieldOptions.CommitChanges>;
	AutoReleaseDunningLetter: PXFieldState;
	IncludeNonOverdueDunning: PXFieldState;
	AddOpenPaymentsAndCreditMemos: PXFieldState;
	DunningFeeInventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	DunningFeeTermID: PXFieldState<PXFieldOptions.CommitChanges>;
	AutoReleaseDunningFee: PXFieldState;
}

@gridConfig({ syncPosition: true })
export class ARSetupApproval extends PXView {
	IsActive: PXFieldState;
	DocType: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("ViewAssignmentMap")
	AssignmentMapID: PXFieldState<PXFieldOptions.CommitChanges>;

	AssignmentNotificationID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ initNewRow: true, allowImport: false })
export class ARDunningSetup extends PXView {
	DunningLetterLevel: PXFieldState;
	DueDays: PXFieldState<PXFieldOptions.CommitChanges>;
	DaysToSettle: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	DunningFee: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ syncPosition: true, autoRepaint: ["Recipients"] })
export class ARNotification extends PXView {
	Active: PXFieldState;
	NotificationCD: PXFieldState;
	NBranchID: PXFieldState;
	EMailAccountID: PXFieldState;
	DefaultPrinterID: PXFieldState;
	ReportID: PXFieldState<PXFieldOptions.CommitChanges>;
	NotificationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Format: PXFieldState<PXFieldOptions.CommitChanges>;
	RecipientsBehavior: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class NotificationSetupRecipient extends PXView {
	Active: PXFieldState;
	ContactType: PXFieldState<PXFieldOptions.CommitChanges>;
	OriginalContactID: PXFieldState;
	ContactID: PXFieldState<PXFieldOptions.CommitChanges>;
	Format: PXFieldState<PXFieldOptions.CommitChanges>;
	AddTo: PXFieldState;
}
