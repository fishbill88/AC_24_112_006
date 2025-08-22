import {
	PXScreen,
	createSingle,
	createCollection,
	graphInfo
} from 'client-controls';

import {
	Allocations,
	Steps,
	StepRules,
	StepSettings,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PM.AllocationMaint',
	primaryView: 'Allocations'
})
export class PM207500 extends PXScreen {
	Allocations = createSingle(Allocations);
	Steps = createCollection(Steps);
	StepRules = createSingle(StepRules);
	StepSettings = createSingle(StepSettings);
}

