import { autoinject } from 'aurelia-framework';
import {
	GridPagerMode,
	PXFieldOptions,
	PXFieldState,
	PXScreen,
	PXView,
	columnConfig,
	createSingle,
	createCollection,
	graphInfo,
	gridConfig,
	viewInfo
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.PR.PRSetupMaint', primaryView: 'Setup' })
@autoinject
export class PR101000 extends PXScreen {
	@viewInfo({ containerName: "Setup" })
	Setup = createSingle(Setup);

	@viewInfo({ containerName: "TransactionDateExceptions" })
	TransactionDateExceptions = createCollection(TransactionDateExceptions);
}

export class Setup extends PXView {
	BatchNumberingID: PXFieldState;
	TranNumberingCD: PXFieldState;
	BatchNumberingCD: PXFieldState;
	PTOAdjustmentNumberingCD: PXFieldState;
	ROENumberingCD: PXFieldState;
	BatchForSubmissionNumberingCD: PXFieldState;
	PayRateDecimalPlaces: PXFieldState;
	PayPeriodDateChangeAllowed: PXFieldState;
	RegularHoursType: PXFieldState<PXFieldOptions.CommitChanges>;
	HolidaysType: PXFieldState<PXFieldOptions.CommitChanges>;
	CommissionType: PXFieldState;
	EnablePieceworkEarningType: PXFieldState<PXFieldOptions.CommitChanges>;
	HoldEntry: PXFieldState;
	UseBenefitRateFromUnionInCertProject: PXFieldState;
	ProjectCostAssignment: PXFieldState<PXFieldOptions.CommitChanges>;
	TimePostingOption: PXFieldState<PXFieldOptions.CommitChanges>;
	OffBalanceAccountGroupID: PXFieldState;
	UpdateGL: PXFieldState;
	SummPost: PXFieldState;
	AutoPost: PXFieldState;
	AutoReleaseOnPay: PXFieldState;
	DisableGLWarnings: PXFieldState;
	HideEmployeeInfo: PXFieldState;
	EarningsAcctDefault: PXFieldState<PXFieldOptions.CommitChanges>;
	EarningsAlternateAcctDefault: PXFieldState<PXFieldOptions.CommitChanges>;
	EarningsSubMask: PXFieldState<PXFieldOptions.CommitChanges>;
	EarningsAlternateSubMask: PXFieldState<PXFieldOptions.CommitChanges>;
	DeductLiabilityAcctDefault: PXFieldState;
	DeductLiabilitySubMask: PXFieldState;
	BenefitExpenseAcctDefault: PXFieldState<PXFieldOptions.CommitChanges>;
	BenefitExpenseAlternateAcctDefault: PXFieldState<PXFieldOptions.CommitChanges>;
	BenefitExpenseSubMask: PXFieldState<PXFieldOptions.CommitChanges>;
	BenefitExpenseAlternateSubMask: PXFieldState<PXFieldOptions.CommitChanges>;
	BenefitLiabilityAcctDefault: PXFieldState;
	BenefitLiabilitySubMask: PXFieldState;
	TaxExpenseAcctDefault: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxExpenseAlternateAcctDefault: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxExpenseSubMask: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxExpenseAlternateSubMask: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxLiabilityAcctDefault: PXFieldState;
	TaxLiabilitySubMask: PXFieldState;
	PTOExpenseAcctDefault: PXFieldState<PXFieldOptions.CommitChanges>;
	PTOExpenseAlternateAcctDefault: PXFieldState<PXFieldOptions.CommitChanges>;
	PTOExpenseSubMask: PXFieldState<PXFieldOptions.CommitChanges>;
	PTOExpenseAlternateSubMask: PXFieldState<PXFieldOptions.CommitChanges>;
	PTOLiabilityAcctDefault: PXFieldState;
	PTOLiabilitySubMask: PXFieldState;
	PTOAssetAcctDefault: PXFieldState;
	PTOAssetSubMask: PXFieldState;
	NoWeekendTransactionDate: PXFieldState;
}

@gridConfig({
	adjustPageSize: false,
	initNewRow: true,
	syncPosition: true,
	pagerMode: GridPagerMode.InfiniteScroll
})
export class TransactionDateExceptions extends PXView {
	@columnConfig({ width: 200 })
	Date: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ width: 200 })
	DayOfWeek: PXFieldState;
	@columnConfig({ width: 400 })
	Description: PXFieldState;
}
