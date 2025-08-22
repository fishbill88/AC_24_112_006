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
	DeductCodeTaxesUS,
	DeductCodeTaxesCAN,
	AcaInformation,
	WorkCompensationRates,
	MaximumInsurableWages,
	EarningsIncreasingWage,
	BenefitsIncreasingWage,
	TaxesIncreasingWage,
	DeductionsDecreasingWage,
	TaxesDecreasingWage,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PR.PRDedBenCodeMaint',
	primaryView: 'Document'
})
export class PR101060 extends PXScreen {
	Refresh: PXActionState;

	Document = createSingle(Document);
	CurrentDocument = createSingle(CurrentDocument);
	DeductCodeTaxesUS = createCollection(DeductCodeTaxesUS);
	DeductCodeTaxesCAN = createCollection(DeductCodeTaxesCAN);
	AcaInformation = createCollection(AcaInformation);
	WorkCompensationRates = createCollection(WorkCompensationRates);
	MaximumInsurableWages = createCollection(MaximumInsurableWages);
	EarningsIncreasingWage = createCollection(EarningsIncreasingWage);
	BenefitsIncreasingWage = createCollection(BenefitsIncreasingWage);
	TaxesIncreasingWage = createCollection(TaxesIncreasingWage);
	DeductionsDecreasingWage = createCollection(DeductionsDecreasingWage);
	TaxesDecreasingWage = createCollection(TaxesDecreasingWage);
}

