import { PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, GridColumnType, TextAlign, GridPreset } from "client-controls";

export class PRPTOBank extends PXView  {
	BankID : PXFieldState<PXFieldOptions.CommitChanges>;
	Description : PXFieldState;
	EarningTypeCD : PXFieldState;
	IsActive : PXFieldState;
	ApplyBandingRules : PXFieldState<PXFieldOptions.CommitChanges>;
	IsCertifiedJobAccrual : PXFieldState;
	CreateFinancialTransaction : PXFieldState<PXFieldOptions.CommitChanges>;
}

export class PRPTOBank2 extends PXView  {
	AccrualMethod : PXFieldState<PXFieldOptions.CommitChanges>;
	TransferDateType : PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate : PXFieldState<PXFieldOptions.Disabled>;
	StartDateMonth : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	StartDateDay : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	CarryoverType : PXFieldState<PXFieldOptions.CommitChanges>;
	SettlementBalanceType : PXFieldState<PXFieldOptions.CommitChanges>;
	DisbursingType : PXFieldState<PXFieldOptions.CommitChanges>;
	BandingRuleRoundingMethod : PXFieldState;
	PTOExpenseAcctID : PXFieldState;
	PTOExpenseSubID : PXFieldState;
	PTOLiabilityAcctID : PXFieldState;
	PTOLiabilitySubID : PXFieldState;
	PTOAssetAcctID : PXFieldState;
	PTOAssetSubID : PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details
})
export class PREmployeeClassPTOBank extends PXView  {
	@columnConfig({ width: 60, textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	IsActive: PXFieldState;

	@columnConfig({ hideViewLink: true })
	EmployeeClassID : PXFieldState<PXFieldOptions.CommitChanges>;

	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	AccrualRate : PXFieldState;
	HoursPerYear : PXFieldState;
	AccrualLimit: PXFieldState;

	@columnConfig({ width: 60, textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	AllowNegativeBalance: PXFieldState;

	@columnConfig({ width: 60, textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	DisburseFromCarryover: PXFieldState;

	CarryoverAmount : PXFieldState<PXFieldOptions.CommitChanges>;
	FrontLoadingAmount : PXFieldState;
	ProbationPeriodBehaviour : PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details
})
export class PRBandingRulePTOBank extends PXView  {
	@columnConfig({ hideViewLink: true })
	EmployeeClassID : PXFieldState<PXFieldOptions.CommitChanges>;

	YearsOfService: PXFieldState<PXFieldOptions.CommitChanges>;
	AccrualRate : PXFieldState;
	HoursPerYear : PXFieldState;
	AccrualLimit : PXFieldState;
	FrontLoadingAmount : PXFieldState;
	CarryoverAmount : PXFieldState<PXFieldOptions.CommitChanges>;
}
