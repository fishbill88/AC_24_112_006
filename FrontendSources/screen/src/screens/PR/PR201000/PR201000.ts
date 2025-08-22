import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo
} from 'client-controls';

import {
	PayrollYear,
	Periods,
	PeriodCreation,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PR.PRPayPeriodMaint',
	primaryView: 'PayrollYear'
})
export class PR201000 extends PXScreen {
	AutoFill: PXActionState;
	Cancel: PXActionState;

	PayrollYear = createSingle(PayrollYear);
	Periods = createCollection(Periods);
	PeriodCreation = createSingle(PeriodCreation);
}

