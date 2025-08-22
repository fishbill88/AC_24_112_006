import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo
} from 'client-controls';

import {
	Photos,
	Attributes,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PJ.PhotoLogs.PJ.Graphs.PhotoEntry',
	primaryView: 'Photos'
})
export class PJ305010 extends PXScreen {
	UploadPhoto: PXActionState;

	Photos = createSingle(Photos);
	Attributes = createCollection(Attributes);
}

