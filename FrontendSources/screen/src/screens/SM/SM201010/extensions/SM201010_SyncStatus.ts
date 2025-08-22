import {
	SM201010
} from '../SM201010';

import {
	PXView,
	createCollection,
	PXFieldState,
	PXActionState,
	featureInstalled,
	columnConfig,
	viewInfo,
	gridConfig
} from 'client-controls';


export interface SM201010_SyncStatus extends SM201010 { }

@featureInstalled('PX.Objects.CS.FeaturesSet+SalesforceIntegration')
export class SM201010_SyncStatus {
	@viewInfo({ containerName: "Sync Status" })
	SyncRecs = createCollection(SFSyncRecord);
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	initNewRow: true,
	syncPosition: true,
	fastFilterByAllFields: false,
})
export class SFSyncRecord extends PXView {
	SyncSalesforce: PXActionState;
	@columnConfig({ width: 200 }) SYProvider__Name: PXFieldState;
	@columnConfig({ width: 200 }) RemoteID: PXFieldState;
	@columnConfig({ width: 120 }) Status: PXFieldState;
	@columnConfig({ width: 80 }) Operation: PXFieldState;
	@columnConfig({ width: 150 }) SFEntitySetup__ImportScenario: PXFieldState;
	@columnConfig({ width: 150 }) SFEntitySetup__ExportScenario: PXFieldState;
	@columnConfig({ width: 150 }) LastErrorMessage: PXFieldState;
	@columnConfig({ width: 120 }) LastAttemptTS: PXFieldState;
	@columnConfig({ width: 120 }) AttemptCount: PXFieldState;
}
