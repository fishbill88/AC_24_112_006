import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo
} from 'client-controls';

import {
	Filter,
	PhotoLogs,
	MainPhoto,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PJ.PhotoLogs.PJ.Graphs.PhotoLogMaint',
	primaryView: 'Filter'
})
export class PJ405000 extends PXScreen {
	Refresh: PXActionState;
	editPhotoLog: PXActionState;
	ViewEntity: PXActionState;

	Filter = createSingle(Filter);
	PhotoLogs = createCollection(PhotoLogs);
	MainPhoto = createSingle(MainPhoto);
}

