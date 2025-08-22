import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo
} from "client-controls";

import {
	EntityFilter,
	CurrentEntity,
	ImportMappings,
	ExportMappings,
	ExportFilters,
	ImportFilters,
	DeleteConfirmationPanel,
	StartRealTimePanel
} from './views';

@graphInfo({ graphType: 'PX.Commerce.Core.BCEntityMaint', primaryView: 'EntityFilter' })
export class BC202000 extends PXScreen {
	EntityFilter = createSingle(EntityFilter);

	CurrentEntity = createSingle(CurrentEntity);

	ImportMappings = createCollection(ImportMappings);

	ImportFilters = createCollection(ImportFilters);

	ExportMappings = createCollection(ExportMappings);

	ExportFilters = createCollection(ExportFilters);

	DeleteConfirmationPanel = createSingle(DeleteConfirmationPanel);

	StartRealTimePanel = createSingle(StartRealTimePanel);
}
