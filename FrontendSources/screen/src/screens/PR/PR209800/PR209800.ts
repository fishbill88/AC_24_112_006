import {
	PXScreen,
	createSingle,
	createCollection,
	graphInfo
} from 'client-controls';

import {
	Filter,
	WorkCompensationCodes,
	WorkCompensationRates,
	ProjectTaskSources,
	LaborItemSources,
	MaximumInsurableWages,
	PMWorkCodeCostCodeRange,
} from './views';

@graphInfo({graphType: 'PX.Objects.PR.PRWorkCodeMaint',	primaryView: 'Filter' })
export class PR209800 extends PXScreen {
	Filter = createSingle(Filter);
	WorkCompensationCodes = createCollection(WorkCompensationCodes);
	WorkCompensationRates = createCollection(WorkCompensationRates);
	ProjectTaskSources = createCollection(ProjectTaskSources);
	LaborItemSources = createCollection(LaborItemSources);
	CostCodeRanges = createCollection(PMWorkCodeCostCodeRange);
	MaximumInsurableWages = createCollection(MaximumInsurableWages);
}

