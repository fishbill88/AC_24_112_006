import {
	PXActionState,
	PXScreen,
	createCollection,
	createSingle,
	graphInfo
} from 'client-controls';

import {
	Document,
	CurrentDocument,
	Earnings,
	SummaryEarnings,
	Deductions,
	Taxes,
	TaxSplits,
	PaymentPTOBanks,
	WCPremiums,
	UnionPackageDeductions,
	PaymentFringeBenefits,
	PaymentFringeBenefitsDecreasingRate,
	PaymentFringeEarningsDecreasingRate,
	RecordsOfEmployment,
	PaymentOvertimeRules,
	ImportTimeActivitiesFilter,
	TimeActivities,
	ExistingPayment,
	ExistingPayrollBatch,
	DeductionDetails,
	BenefitDetails,
	TaxDetails,
	PaymentTaxApplicableAmounts,
	PTODetails,
	DirectDepositSplits,
	ProjectPackageDeductions,
	UpdateTaxesPopupView,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PR.PRPayChecksAndAdjustments',
	primaryView: 'Document'
})
export class PR302000 extends PXScreen {

	Refresh: PXActionState;
	ViewOriginalDocument: PXActionState;
	ViewPaymentBatch: PXActionState;
	ViewTimeActivity: PXActionState;
	ViewRecordsOfEmployment: PXActionState;
	ViewDirectDepositSplits: PXActionState;
	RevertOvertimeCalculation: PXActionState;
	AddSelectedTimeActivities: PXActionState;
	AddSelectedTimeActivitiesAndClose: PXActionState;
	ViewExistingPayment: PXActionState;
	ViewExistingPayrollBatch: PXActionState;
	RedirectTaxMaintenance: PXActionState;
	DeleteEarningDetail: PXActionState;

	Document = createSingle(Document);
	CurrentDocument = createSingle(CurrentDocument);
	Earnings = createCollection(Earnings);
	SummaryEarnings = createCollection(SummaryEarnings);
	Deductions = createCollection(Deductions);
	Taxes = createCollection(Taxes);
	TaxSplits = createCollection(TaxSplits);
	PaymentPTOBanks = createCollection(PaymentPTOBanks);
	WCPremiums = createCollection(WCPremiums);
	UnionPackageDeductions = createCollection(UnionPackageDeductions);
	PaymentFringeBenefits = createCollection(PaymentFringeBenefits);
	PaymentFringeBenefitsDecreasingRate = createCollection(PaymentFringeBenefitsDecreasingRate);
	PaymentFringeEarningsDecreasingRate = createCollection(PaymentFringeEarningsDecreasingRate);
	RecordsOfEmployment = createCollection(RecordsOfEmployment);
	PaymentOvertimeRules = createCollection(PaymentOvertimeRules);
	ImportTimeActivitiesFilter = createSingle(ImportTimeActivitiesFilter);
	TimeActivities = createCollection(TimeActivities);
	ExistingPayment = createSingle(ExistingPayment);
	ExistingPayrollBatch = createSingle(ExistingPayrollBatch);
	DeductionDetails = createCollection(DeductionDetails);
	BenefitDetails = createCollection(BenefitDetails);
	TaxDetails = createCollection(TaxDetails);
	PaymentTaxApplicableAmounts = createCollection(PaymentTaxApplicableAmounts);
	PTODetails = createCollection(PTODetails);
	DirectDepositSplits = createCollection(DirectDepositSplits);
	ProjectPackageDeductions = createCollection(ProjectPackageDeductions);
	UpdateTaxesPopupView = createSingle(UpdateTaxesPopupView);
}

