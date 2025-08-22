import {
	columnConfig,
	gridConfig,
	headerDescription,
	linkCommand,
	GridColumnShowHideMode,
	ICurrencyInfo,
	PXActionState,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class Quote extends PXView {
	QuoteNbr: PXFieldState;
	OpportunityID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsPrimary: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Disabled>;
	DocumentDate: PXFieldState;
	ExpirationDate: PXFieldState;
	ExternalRef: PXFieldState;
	Subject: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Multiline>;
	TemplateID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectManager: PXFieldState;
	BAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState;
	CuryID: PXFieldState;
	QuoteProjectID: PXFieldState<PXFieldOptions.Disabled>;
	QuoteProjectCD: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAmount: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Disabled>;
	CuryCostTotal: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Disabled>;
	CuryGrossMarginAmount: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Disabled>;
	GrossMarginPct: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Disabled>;
	CuryTaxTotal: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Disabled>;
	CuryQuoteTotal: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Disabled>;
	@headerDescription FormCaptionDescription: PXFieldState;
}

export class QuoteCurrent extends PXView {
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	TermsID: PXFieldState<PXFieldOptions.CommitChanges>;
	CreatedByID_Creator_Username: PXFieldState;
	AllowOverrideContactAddress: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	initNewRow: true
})
export class Products extends PXView {
	AddNew: PXActionState;
	Copy: PXActionState;
	Paste: PXActionState;

	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	Quantity: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitCost: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryExtCost: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	ManualPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryExtPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscPct: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDiscAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAmount: PXFieldState;
	DiscountID: PXFieldState;
	DiscountSequenceID: PXFieldState;
	ManualDisc: PXFieldState<PXFieldOptions.CommitChanges>;
	TaskCD: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	ExpenseAccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	RevenueAccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	TaxCategoryID: PXFieldState;
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false
})
export class Tasks extends PXView {
	AddCommonTasks: PXActionState;

	TaskCD: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	Type: PXFieldState;
	PlannedStartDate: PXFieldState;
	PlannedEndDate: PXFieldState;
	TaxCategoryID: PXFieldState;
	isDefault: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class Taxes extends PXView {
	@columnConfig({ allowUpdate: false })
	TaxID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	TaxRate: PXFieldState;
	CuryTaxableAmt: PXFieldState;
	CuryTaxAmt: PXFieldState;
	Tax__TaxType: PXFieldState;
	Tax__PendingTax: PXFieldState;
	Tax__ReverseTax: PXFieldState;
	Tax__ExemptTax: PXFieldState;
	Tax__StatisticalTax: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	allowInsert: false,
	allowDelete: false
})
export class Answers extends PXView {
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False,
		hideViewLink: true
	})
	AttributeID: PXFieldState;
	isRequired: PXFieldState;
	Value: PXFieldState;
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
	WorkgroupID: PXFieldState;
	ApprovedByEmployee__AcctCD: PXFieldState;
	ApprovedByEmployee__AcctName: PXFieldState;
	OrigOwnerID: PXFieldState<PXFieldOptions.Hidden>;
	ApproveDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Status: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Reason: PXFieldState;
	AssignmentMapID: PXFieldState<PXFieldOptions.Hidden>;
	RuleID: PXFieldState<PXFieldOptions.Hidden>;
	StepID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}

export class Quote_Contact extends PXView {
	FullName: PXFieldState<PXFieldOptions.CommitChanges>;
	Title: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.NoLabel>;
	FirstName: PXFieldState<PXFieldOptions.CommitChanges>;
	LastName: PXFieldState<PXFieldOptions.CommitChanges>;
	Salutation: PXFieldState<PXFieldOptions.CommitChanges>;
	EMail: PXFieldState<PXFieldOptions.CommitChanges>;
	WebSite: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1Type: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.NoLabel>;
	Phone1: PXFieldState<PXFieldOptions.NoLabel>;
	Phone2Type: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.NoLabel>;
	Phone2: PXFieldState<PXFieldOptions.NoLabel>;
	Phone3Type: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.NoLabel>;
	Phone3: PXFieldState<PXFieldOptions.NoLabel>;
	FaxType: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.NoLabel>;
	Fax: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.NoLabel>;
}

export class Quote_Address extends PXView {
	AddressLine1: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine2: PXFieldState<PXFieldOptions.CommitChanges>;
	City: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}

export class Shipping_Address extends PXView {
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	State: PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	Latitude: PXFieldState;
	Longitude: PXFieldState;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}

export class CopyQuoteInfo extends PXView {
	Description: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalculatePrices: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalculateDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ConvertQuoteInfo extends PXView {
	CreateLaborRates: PXFieldState<PXFieldOptions.CommitChanges>;
	ActivateProject: PXFieldState<PXFieldOptions.CommitChanges>;
	ActivateTasks: PXFieldState<PXFieldOptions.CommitChanges>;
	CopyNotes: PXFieldState<PXFieldOptions.CommitChanges>;
	CopyFiles: PXFieldState<PXFieldOptions.CommitChanges>;
	MoveActivities: PXFieldState<PXFieldOptions.CommitChanges>;
	TaskCD: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class recalcdiscountsfilter extends PXView {
	RecalcTarget: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalcUnitPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalcDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class TasksForAddition extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	TaskCD: PXFieldState;
	Description: PXFieldState;
	ApproverID: PXFieldState;
	PMProject__NonProject: PXFieldState;
}

export class CurrencyInfo extends PXView implements ICurrencyInfo {
	CuryInfoID: PXFieldState;
	BaseCuryID: PXFieldState;
	BaseCalc: PXFieldState;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisplayCuryID: PXFieldState;
	CuryRateTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	BasePrecision: PXFieldState;
	CuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEffDate: PXFieldState<PXFieldOptions.CommitChanges>;
	RecipRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleCuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleRecipRate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false
})
export class Activities extends PXView {
	NewTask: PXActionState;
	NewEvent: PXActionState;
	NewMailActivity: PXActionState;
	NewActivity: PXActionState;
	TogglePinActivity: PXActionState;

	@columnConfig({
		allowFilter: false,
		allowSort: false
	})
	IsPinned: PXFieldState;
	@columnConfig({
		allowFilter: false,
		allowSort: false
	})
	IsCompleteIcon: PXFieldState;
	@columnConfig({
		allowFilter: false,
		allowSort: false
	})
	PriorityIcon: PXFieldState;
	@columnConfig({
		allowFilter: false,
		allowSort: false
	})
	CRReminder__ReminderIcon: PXFieldState;
	@columnConfig({
		allowFilter: false,
		allowSort: false
	})
	ClassIcon: PXFieldState;
	ClassInfo: PXFieldState;
	@linkCommand("ViewActivity")
	Subject: PXFieldState;
	UIStatus: PXFieldState;
	Released: PXFieldState;
	StartDate: PXFieldState;
	CreatedDateTime: PXFieldState;
	TimeSpent: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	CreatedByID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;
	WorkgroupID: PXFieldState;
	@linkCommand("OpenActivityOwner")
	OwnerID: PXFieldState;
	Source: PXFieldState<PXFieldOptions.Hidden>;
	BAccountID: PXFieldState<PXFieldOptions.Hidden>;
	ContactID: PXFieldState<PXFieldOptions.Hidden>;
	ProjectID: PXFieldState<PXFieldOptions.Hidden>;
	ProjectTaskID: PXFieldState<PXFieldOptions.Hidden>;
	body: PXFieldState;
}

export class ReasonApproveRejectParams extends PXView {
	Reason: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Multiline>;
}

export class ReassignApprovalFilter extends PXView {
	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class AddressLookupFilter extends PXView {
	SearchAddress: PXFieldState<PXFieldOptions.NoLabel>;
	ViewName: PXFieldState;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	AddressLine3: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState;
	State: PXFieldState;
	PostalCode: PXFieldState;
	Latitude: PXFieldState;
	Longitude: PXFieldState;
}
