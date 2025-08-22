import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, columnConfig, PXFieldOptions, createCollection, linkCommand, gridConfig, GridColumnShowHideMode, PXActionState
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AP.VendorClassMaint', primaryView: 'VendorClassRecord' })
export class AP201000 extends PXScreen {

	CRAttribute_ViewDetails: PXActionState;

	VendorClassRecord = createSingle(VendorClass);
	CurVendorClassRecord = createSingle(VendorClass);
	Mapping = createCollection(CSAttributeGroupList);
	NotificationSources = createCollection(NotificationSource);
	NotificationRecipients = createCollection(NotificationRecipient);
	LienWaiverRecipientProjects = createCollection(LienWaiverRecipient);
}

export class VendorClass extends PXView {

	VendorClassID: PXFieldState;
	Descr: PXFieldState;
	LocaleName: PXFieldState;

	// General
	CountryID: PXFieldState;
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxZoneID: PXFieldState;
	RequireTaxZone: PXFieldState;
	TaxCalcMode: PXFieldState;
	DefaultLocationCDFromBranch: PXFieldState;
	GroupMask: PXFieldState;

	// Purchasing
	ShipTermsID: PXFieldState;
	RcptQtyAction: PXFieldState;

	//Financial
	TermsID: PXFieldState;
	PaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	CashAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	PaymentByType: PXFieldState;
	CuryID: PXFieldState;
	AllowOverrideCury: PXFieldState;
	CuryRateTypeID: PXFieldState;
	AllowOverrideRate: PXFieldState;
	PaymentsByLinesAllowed: PXFieldState;
	RetainageApply: PXFieldState;

	// Email and Printing settings
	PrintPO: PXFieldState;
	EmailPO: PXFieldState;

	ShouldGenerateLienWaivers: PXFieldState;

	// GL Accounts

	APAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	APSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscountAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscountSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscTakenAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscTakenSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepaymentAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepaymentSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrebookAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrebookSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	POAccrualAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	POAccrualSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	UnrealizedGainAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	UnrealizedGainSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	UnrealizedLossAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	UnrealizedLossSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainageAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainageSubID: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class CSAttributeGroupList extends PXView {

	@columnConfig({ allowNull: false})
	IsActive: PXFieldState;

	@linkCommand("CRAttribute_ViewDetails")
	AttributeID: PXFieldState;

	@columnConfig({ allowNull: false })
	Description: PXFieldState;

	SortOrder: PXFieldState;

	@columnConfig({ allowNull: false })
	Required: PXFieldState;

	@columnConfig({ allowNull: true })
	CSAttribute__IsInternal: PXFieldState;

	@columnConfig({ allowNull: false })
	ControlType: PXFieldState;

	@columnConfig({ allowNull: true })
	DefaultValue: PXFieldState;

}

@gridConfig({ syncPosition: true, autoRepaint: ["NotificationRecipients"] })
export class NotificationSource extends PXView {

	Active: PXFieldState;
	SetupID: PXFieldState;
	NBranchID: PXFieldState;
	EMailAccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ReportID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	NotificationID: PXFieldState;

	Format: PXFieldState;
	RecipientsBehavior: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class NotificationRecipient extends PXView {

	Active: PXFieldState;
	ContactType: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	OriginalContactID: PXFieldState;

	ContactID: PXFieldState;
	Format: PXFieldState;
	AddTo: PXFieldState;

}

@gridConfig({ allowInsert: false })
export class LienWaiverRecipient extends PXView {

	@columnConfig({ allowSort: false, allowCheckAll: true })
	Selected: PXFieldState;

	PMProject__ContractCD: PXFieldState;
	PMProject__Status: PXFieldState;
	PMProject__CustomerID: PXFieldState;
	PMProject__Description: PXFieldState;
	PMProject__CustomerID_Customer_acctName: PXFieldState;
	PMProject__CuryID: PXFieldState;
	MinimumCommitmentAmount: PXFieldState;
}
