import {
	PXScreen,
	createSingle,
	createCollection,
	graphInfo
} from 'client-controls';

import {
	FiscalYearSetup,
	Periods,
} from './views';

@graphInfo({ graphType: 'PX.Objects.PR.PRPayGroupYearSetupMaint', primaryView: 'FiscalYearSetup' })
export class PR206000 extends PXScreen {
	FiscalYearSetup = createSingle(FiscalYearSetup);
	Periods = createCollection(Periods);
}

