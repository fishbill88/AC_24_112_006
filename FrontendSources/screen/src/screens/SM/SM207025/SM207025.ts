import {
	PXScreen,
	createSingle,
	createCollection,
	graphInfo,
	viewInfo,
	PXActionState,
} from 'client-controls';
import { SYInsertFrom,SYMapping,SYMappingField,SYWhatToShow,SYMappingCondition } from './views';

@graphInfo({graphType: 'PX.Api.SYExportMaint', primaryView: 'Mappings'})
export class SM207025 extends PXScreen {
	fillSource: PXActionState;
	fillDestination: PXActionState;

	@viewInfo({containerName: 'Mapping'})
	WhatToShowFilter = createSingle(SYWhatToShow);
	@viewInfo({containerName: 'Choose scenario to insert steps from'})
	InsertFromFilter = createSingle(SYInsertFrom);

	@viewInfo({containerName: 'Scenario Summary'})
	Mappings = createSingle(SYMapping);
	@viewInfo({containerName: 'Mapping'})
	FieldMappings = createCollection(SYMappingField);
	@viewInfo({containerName: 'Source Restrictions'})
	MatchingConditions = createCollection(SYMappingCondition);
}
