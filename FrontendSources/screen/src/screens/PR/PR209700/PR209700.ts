import {
	PXScreen,
	createSingle,
	createCollection,
	graphInfo
} from 'client-controls';

import {
	UnionLocal,
	EarningRates,
	DeductionsAndBenefitsPackage,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PR.PRUnionMaint',
	primaryView: 'UnionLocal'
})
export class PR209700 extends PXScreen {
	UnionLocal = createSingle(UnionLocal);
	EarningRates = createCollection(EarningRates);
	DeductionsAndBenefitsPackage = createCollection(DeductionsAndBenefitsPackage);
}

