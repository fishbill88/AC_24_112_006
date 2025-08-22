import {PXScreen, createSingle,	createCollection, graphInfo, viewInfo, PXActionState } from 'client-controls';
import { ActionExecution, ActionExecutionMapping,ActionExecutionParameter,BPEvent,BPEventData } from './views';

@graphInfo({graphType: 'PX.BusinessProcess.UI.ActionExecutionMaint', primaryView: 'ActionExecutions'})
export class SM204007 extends PXScreen {
	viewBusinessEvent: PXActionState;

	@viewInfo({containerName: 'Create Business Event'})
	NewEventData = createSingle(BPEventData);

	@viewInfo({containerName: ''})
	ActionExecutions = createSingle(ActionExecution);

	@viewInfo({containerName: 'Keys'})
	ActionExecutionMappings = createCollection(ActionExecutionMapping);
	@viewInfo({containerName: 'Field Values'})
	ActionExecutionParameters = createCollection(ActionExecutionParameter);
	@viewInfo({containerName: 'Executed By Events'})
	BusinessEvents = createCollection(BPEvent);
}
