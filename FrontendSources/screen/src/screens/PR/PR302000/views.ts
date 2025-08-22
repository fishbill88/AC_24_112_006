import {
	GridColumnShowHideMode,
	PXFieldOptions,
	PXActionState,
	PXFieldState,
	PXView,
	columnConfig,
	gridConfig,
	linkCommand,
	GridPreset,
    getGridPereset
} from "client-controls";

export class Document extends PXView {
	DocType: PXFieldState<PXFieldOptions.CommitChanges>;
	RefNbr: PXFieldState;
	Status: PXFieldState;
	PayGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	PayPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState;
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	PaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	TransactionDate: PXFieldState;
	DocDesc: PXFieldState;
	GrossAmount: PXFieldState;
	DedAmount: PXFieldState;
	TaxAmount: PXFieldState;
	NetAmount: PXFieldState;
	TerminationReason: PXFieldState;
	IsRehirable: PXFieldState;
	TerminationDate: PXFieldState;
	ShowROETab: PXFieldState;
}

export class CurrentDocument extends PXView {
	BatchNbr: PXFieldState;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	CountryID: PXFieldState;
	@linkCommand("ViewOriginalDocument")
	OrigRefNbr: PXFieldState;
	@linkCommand("ViewPaymentBatch")
	PaymentBatchNbr: PXFieldState;
	ChkVoidType: PXFieldState;
	ExtRefNbr: PXFieldState;
	EmpType: PXFieldState<PXFieldOptions.CommitChanges>;
	RegularAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	ManualRegularAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	ApplyOvertimeRules: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false,
	initNewRow: true,
	autoRepaint: ["PaymentFringeBenefits", "PaymentFringeBenefitsDecreasingRate", "PaymentFringeEarningsDecreasingRate"]
})
export class Earnings extends PXView {
	CopySelectedEarningDetailLine: PXActionState;
	ViewOvertimeRules: PXActionState;
	ImportTimeActivities: PXActionState;
	RevertPTOSplitCalculation: PXActionState;

	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False
	})
	AllowCopy: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({
		hideViewLink: true
	})
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	Date: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true
	})
	TypeCD: PXFieldState<PXFieldOptions.CommitChanges>;
	TypeCD_Description: PXFieldState;
	@columnConfig({
		hideViewLink: true
	})
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Hours: PXFieldState<PXFieldOptions.CommitChanges>;
	Units: PXFieldState<PXFieldOptions.CommitChanges>;
	UnitType: PXFieldState<PXFieldOptions.CommitChanges>;
	Rate: PXFieldState<PXFieldOptions.CommitChanges>;
	ManualRate: PXFieldState<PXFieldOptions.CommitChanges>;
	Amount: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true
	})
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true
	})
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CertifiedJob: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	UnionID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true
	})
	LabourItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true
	})
	WorkCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShiftID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewTimeActivity")
	SourceNoteID: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false
})
export class SummaryEarnings extends PXView {
	TypeCD: PXFieldState<PXFieldOptions.CommitChanges>;
	TypeCD_Description: PXFieldState;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	RoundedHours: PXFieldState<PXFieldOptions.CommitChanges>;
	Rate: PXFieldState<PXFieldOptions.CommitChanges>;
	Amount: PXFieldState;
	MTDAmount: PXFieldState;
	QTDAmount: PXFieldState;
	YTDAmount: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false,
	adjustPageSize: false,
	initNewRow: true
})
export class Deductions extends PXView {
	ViewDeductionDetails: PXActionState;
	ViewBenefitDetails: PXActionState;

	CodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	CodeID_Description: PXFieldState;
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	Source: PXFieldState;
	ContribType: PXFieldState;
	DedAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	CntAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	SaveOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	YtdAmount: PXFieldState;
	EmployerYtdAmount: PXFieldState;
	PRDeductCode__IsPayableBenefit: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false,
	adjustPageSize: false,
	autoRepaint: ["TaxSplits"]
})
export class Taxes extends PXView {
	ViewTaxDetails: PXActionState;
	ViewTaxableWageDetails: PXActionState;

	TaxID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxID_Description: PXFieldState;
	TaxCategory: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	WageBaseAmount: PXFieldState;
	WageBaseGrossAmt: PXFieldState;
	WageBaseHours: PXFieldState;
	YtdAmount: PXFieldState;
	AdjustedGrossAmount: PXFieldState;
	ExemptionAmount: PXFieldState;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	fastFilterByAllFields: false
})
export class TaxSplits extends PXView {
	WageType: PXFieldState;
	TaxID: PXFieldState;
	WageBaseAmount: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false,
	adjustPageSize: false
})
export class PaymentPTOBanks extends PXView {
	ViewPTODetails: PXActionState;

	@columnConfig({
		hideViewLink: true
	})
	BankID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	BankID_Description: PXFieldState;
	EffectiveStartDate: PXFieldState;
	IsCertifiedJob: PXFieldState<PXFieldOptions.CommitChanges>;
	AccrualMethod: PXFieldState;
	AccrualRate: PXFieldState<PXFieldOptions.CommitChanges>;
	HoursPerYear: PXFieldState<PXFieldOptions.CommitChanges>;
	AccrualLimit: PXFieldState;
	AccrualAmount: PXFieldState;
	AccrualMoney: PXFieldState;
	FrontLoadingAmount: PXFieldState;
	CarryoverAmount: PXFieldState;
	CarryoverMoney: PXFieldState;
	TotalAccrual: PXFieldState;
	TotalAccrualMoney: PXFieldState;
	DisbursementAmount: PXFieldState;
	DisbursementMoney: PXFieldState;
	TotalDisbursement: PXFieldState;
	TotalDisbursementMoney: PXFieldState;
	SettlementDiscardAmount: PXFieldState;
	AccumulatedAmount: PXFieldState;
	AccumulatedMoney: PXFieldState;
	UsedAmount: PXFieldState;
	UsedMoney: PXFieldState;
	AvailableAmount: PXFieldState;
	AvailableMoney: PXFieldState;
	@columnConfig({
		width: 400
	})
	CalculationFormula: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false,
	adjustPageSize: false
})
export class WCPremiums extends PXView {
	@columnConfig({
		hideViewLink: true
	})
	BranchID: PXFieldState;
	@columnConfig({
		hideViewLink: true
	})
	WorkCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	PMWorkCode__Description: PXFieldState;
	@columnConfig({
		hideViewLink: true
	})
	DeductCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true
	})
	PRDeductCode__State: PXFieldState;
	DeductionCalcType: PXFieldState;
	BenefitCalcType: PXFieldState;
	DeductionRate: PXFieldState<PXFieldOptions.CommitChanges>;
	Rate: PXFieldState<PXFieldOptions.CommitChanges>;
	RegularWageBaseHours: PXFieldState<PXFieldOptions.CommitChanges>;
	OvertimeWageBaseHours: PXFieldState<PXFieldOptions.CommitChanges>;
	WageBaseHours: PXFieldState<PXFieldOptions.CommitChanges>;
	RegularWageBaseAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	OvertimeWageBaseAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	WageBaseAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	DeductionAmount: PXFieldState;
	Amount: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false,
	adjustPageSize: false
})
export class UnionPackageDeductions extends PXView {
	@columnConfig({
		hideViewLink: true
	})
	UnionID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true
	})
	LaborItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true
	})
	DeductCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	DeductionCalcType: PXFieldState;
	BenefitCalcType: PXFieldState;
	RegularWageBaseHours: PXFieldState<PXFieldOptions.CommitChanges>;
	OvertimeWageBaseHours: PXFieldState<PXFieldOptions.CommitChanges>;
	WageBaseHours: PXFieldState<PXFieldOptions.CommitChanges>;
	RegularWageBaseAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	OvertimeWageBaseAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	WageBaseAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	DeductionAmount: PXFieldState;
	BenefitAmount: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false,
	adjustPageSize: false
})
export class PaymentFringeBenefits extends PXView {
	ViewProjectDeductionAndBenefitPackages: PXActionState;

	ProjectID: PXFieldState;
	LaborItemID: PXFieldState;
	ProjectTaskID: PXFieldState;
	ApplicableHours: PXFieldState;
	ProjectHours: PXFieldState;
	FringeRate: PXFieldState;
	ReducingRate: PXFieldState;
	CalculatedFringeRate: PXFieldState;
	PaidFringeAmount: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false,
	adjustPageSize: false
})
export class PaymentFringeBenefitsDecreasingRate extends PXView {
	DeductCodeID: PXFieldState;
	AnnualizationException: PXFieldState;
	AnnualHours: PXFieldState;
	AnnualWeeks: PXFieldState;
	ApplicableHours: PXFieldState;
	Amount: PXFieldState;
	BenefitRate: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false,
	adjustPageSize: false
})
export class PaymentFringeEarningsDecreasingRate extends PXView {
	EarningTypeCD: PXFieldState;
	ActualPayRate: PXFieldState;
	PrevailingWage: PXFieldState;
	Amount: PXFieldState;
	AnnualizationException: PXFieldState;
	AnnualHours: PXFieldState;
	AnnualWeeks: PXFieldState;
	ApplicableHours: PXFieldState;
	BenefitRate: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false,
	allowInsert: false
})
export class RecordsOfEmployment extends PXView {
	@linkCommand("ViewRecordsOfEmployment")
	RefNbr: PXFieldState;
	Status: PXFieldState;
	Amendment: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false,
	adjustPageSize: false
})
export class PaymentOvertimeRules extends PXView {
	@columnConfig({
		allowCheckAll: true
	})
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	OvertimeRuleID: PXFieldState;
	PROvertimeRule__Description: PXFieldState;
	PROvertimeRule__DisbursingTypeCD: PXFieldState;
	PROvertimeRule__OvertimeMultiplier: PXFieldState;
	PROvertimeRule__RuleType: PXFieldState;
	PROvertimeRule__OvertimeThreshold: PXFieldState;
	PROvertimeRule__WeekDay: PXFieldState;
	PROvertimeRule__NumberOfConsecutiveDays: PXFieldState;
	PROvertimeRule__State: PXFieldState;
	PROvertimeRule__UnionID: PXFieldState;
	PROvertimeRule__ProjectID: PXFieldState;
}

export class ImportTimeActivitiesFilter extends PXView {
	ShowImportedActivities: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false
})
export class TimeActivities extends PXView {
	ToggleSelectedTimeActivities: PXActionState;

	Selected: PXFieldState;
	OwnerID: PXFieldState;
	OwnerID_Description: PXFieldState;
	Branch__BranchCD: PXFieldState;
	Date: PXFieldState;
	TimeSpent: PXFieldState;
	EarningTypeID: PXFieldState;
	ProjectID: PXFieldState;
	ProjectTaskID: PXFieldState;
	CertifiedJob: PXFieldState;
	UnionID: PXFieldState;
	LabourItemID: PXFieldState;
	CostCodeID: PXFieldState;
	WorkCodeID: PXFieldState;
}

export class ExistingPayment extends PXView {
	Message: PXFieldState<PXFieldOptions.Disabled>;
}

export class ExistingPayrollBatch extends PXView {
	Message: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false,
	adjustPageSize: false
})
export class DeductionDetails extends PXView {
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	CodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	CodeID_Description: PXFieldState;
	Amount: PXFieldState;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false,
	adjustPageSize: false
})
export class BenefitDetails extends PXView {
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	CodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	CodeID_Description: PXFieldState;
	Amount: PXFieldState<PXFieldOptions.CommitChanges>;
	LiabilityAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	LiabilitySubID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	EarningTypeCD: PXFieldState<PXFieldOptions.CommitChanges>;
	LabourItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false,
	adjustPageSize: false
})
export class TaxDetails extends PXView {
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxID_Description: PXFieldState;
	TaxCategory: PXFieldState;
	Amount: PXFieldState<PXFieldOptions.CommitChanges>;
	LiabilityAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	LiabilitySubID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	EarningTypeCD: PXFieldState<PXFieldOptions.CommitChanges>;
	LabourItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false,
	adjustPageSize: false
})
export class PaymentTaxApplicableAmounts extends PXView {
	TaxID: PXFieldState;
	WageTypeID: PXFieldState;
	IsSupplemental: PXFieldState;
	AmountAllowed: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false,
	adjustPageSize: false
})
export class PTODetails extends PXView {
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	BankID: PXFieldState<PXFieldOptions.CommitChanges>;
	BankID_Description: PXFieldState;
	Amount: PXFieldState<PXFieldOptions.CommitChanges>;
	LiabilityAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	LiabilitySubID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	EarningTypeCD: PXFieldState<PXFieldOptions.CommitChanges>;
	LabourItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false,
	adjustPageSize: false
})
export class DirectDepositSplits extends PXView {
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.Server
	})
	BankAcctNbr: PXFieldState;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.Server
	})
	BankAcctType: PXFieldState;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.Server
	})
	BankName: PXFieldState;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.Server
	})
	BankRoutingNbr: PXFieldState;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.Server
	})
	FinInstNbrCan: PXFieldState;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.Server
	})
	BankTransitNbrCan: PXFieldState;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.Server
	})
	BankAcctNbrCan: PXFieldState;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.Server
	})
	BeneficiaryName: PXFieldState;
	Amount: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false,
	adjustPageSize: false
})
export class ProjectPackageDeductions extends PXView {
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	LaborItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	DeductCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	DeductionCalcType: PXFieldState;
	BenefitCalcType: PXFieldState;
	RegularWageBaseHours: PXFieldState<PXFieldOptions.CommitChanges>;
	OvertimeWageBaseHours: PXFieldState<PXFieldOptions.CommitChanges>;
	WageBaseHours: PXFieldState<PXFieldOptions.CommitChanges>;
	RegularWageBaseAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	OvertimeWageBaseAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	WageBaseAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	DeductionAmount: PXFieldState;
	BenefitAmount: PXFieldState;
}

export class UpdateTaxesPopupView extends PXView {
	Message: PXFieldState<PXFieldOptions.Disabled>;
}

