import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo,
	viewInfo
} from 'client-controls';

import {
	Items,
	ProjectTaskSources,
	LaborItemSources,
	CostCodeRanges,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PM.WorkCodeMaint',
	primaryView: 'Items',
	hideNotesIndicator: true,
})
export class PM209800 extends PXScreen {
	Items = createCollection(Items);
	ProjectTaskSources = createCollection(ProjectTaskSources);
	LaborItemSources = createCollection(LaborItemSources);
	CostCodeRanges = createCollection(CostCodeRanges);
}

