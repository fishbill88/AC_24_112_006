import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo,
	viewInfo
} from 'client-controls';

import {
	GenerateWeeksDialog,
	Setup,
	WeekFilter,
	CustomWeek,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.EP.EPSetupMaint',
	primaryView: 'Setup'
})
export class EP101000 extends PXScreen {
 	GenerateWeeks: PXActionState;
	GenerateWeeksOK: PXActionState;

	Setup = createSingle(Setup);
	GenerateWeeksDialog = createSingle(GenerateWeeksDialog);
	WeekFilter = createSingle(WeekFilter);
	CustomWeek = createCollection(CustomWeek);
}

