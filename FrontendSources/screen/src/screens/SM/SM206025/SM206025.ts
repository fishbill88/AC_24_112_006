import {
	PXScreen,
	createSingle,
	createCollection,
	graphInfo,
	viewInfo,
	PXActionState,
} from 'client-controls';
import { Messages as SysMessages } from 'client-controls/services/messages';
import { SYInsertFrom,SYMapping,SYMappingField,SYWhatToShow,SYImportCondition,SYMappingCondition } from './views';

@graphInfo({graphType: 'PX.Api.SYImportMaint', primaryView: 'Mappings'})
export class SM206025 extends PXScreen {
	fillSource: PXActionState;
	fillDestination: PXActionState;
	SysMessages = SysMessages;

	@viewInfo({containerName: 'Choose scenario to insert steps from'})
	InsertFromFilter = createSingle(SYInsertFrom);

	@viewInfo({containerName: 'Scenario Summary'})
	Mappings = createSingle(SYMapping);

	@viewInfo({containerName: 'Mapping'})
	FieldMappings = createCollection(SYMappingField);

	@viewInfo({containerName: 'Mapping'})
	WhatToShowFilter = createSingle(SYWhatToShow);
	@viewInfo({containerName: 'Source Restrictions'})
	Conditions = createCollection(SYImportCondition);

	@viewInfo({containerName: 'Target Restrictions'})
	MatchingConditions = createCollection(SYMappingCondition);
}