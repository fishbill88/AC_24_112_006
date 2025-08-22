import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo,
	viewInfo
} from 'client-controls';

import {
	Submittals,
	CurrentSubmittal,
	SubmittalWorkflowItems,
	Activities,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PJ.Submittals.PJ.Graphs.SubmittalEntry',
	primaryView: 'Submittals', showUDFIndicator: true
})
export class PJ306000 extends PXScreen {
	ViewActivity: PXActionState;
	OpenActivityOwner: PXActionState;

	Submittals = createSingle(Submittals);
	CurrentSubmittal = createSingle(CurrentSubmittal);
	SubmittalWorkflowItems = createCollection(SubmittalWorkflowItems);
	Activities = createCollection(Activities);
}

