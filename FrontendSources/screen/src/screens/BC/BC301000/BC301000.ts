import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	localizable
} from "client-controls";

import {
	MasterView,
	DetailsView,
	StatusEditPanel,
	StatusDetailsPanel
} from './views';

@graphInfo({ graphType: 'PX.Commerce.Core.BCSyncHistoryMaint', primaryView: 'MasterView' })
export class BC301000 extends PXScreen {
	MasterView = createSingle(MasterView);

	DetailsView = createCollection(DetailsView);

	StatusEditPanel = createSingle(StatusEditPanel);

	StatusDetailsPanel = createCollection(StatusDetailsPanel);
}
