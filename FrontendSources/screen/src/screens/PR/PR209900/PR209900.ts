import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo 
} from 'client-controls';
import {
	CertifiedProjectFilter,
	PMLaborCostRate,
	PRDeductionAndBenefitProjectPackage,
	Contract,
	PRProjectFringeBenefitRate,
	PRProjectFringeBenefitRateReducingDeduct
} from './views';

@graphInfo({graphType: 'PX.Objects.PR.PRCertifiedProjectMaint', primaryView: 'CertifiedProject', hideFilesIndicator: true, hideNotesIndicator: true})
export class PR209900 extends PXScreen {
	@viewInfo({containerName: ''})
	CertifiedProject = createSingle(CertifiedProjectFilter);
	@viewInfo({containerName: 'Earning Rates'})
	EarningRates = createCollection(PMLaborCostRate);

	@viewInfo({containerName: 'Deductions and Benefits'})
	DeductionsAndBenefitsPackage = createCollection(PRDeductionAndBenefitProjectPackage);

	@viewInfo({containerName: 'Fringe Benefits'})
	CurrentProject = createSingle(Contract);
	@viewInfo({containerName: 'Rates'})
	FringeBenefitRates = createCollection(PRProjectFringeBenefitRate);

	@viewInfo({containerName: 'Benefits Reducing the Rate'})
	FringeBenefitRateReducingDeductions = createCollection(PRProjectFringeBenefitRateReducingDeduct);
}
