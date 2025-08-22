import {
	PXView, PXFieldState, PXActionState, PXScreen, PXFieldOptions,
	gridConfig, columnConfig, graphInfo, createSingle, createCollection } from "client-controls";

@graphInfo({graphType: 'PX.SM.InstallationSetup', primaryView: 'Setup'})
export class SM203505 extends PXScreen {
	reloadParameters: PXActionState;

	Setup = createSingle(UPSetup);
	StorageSettings = createCollection(UPStorageParameters);
}

export class UPSetup extends PXView {
	UpdateEnabled: PXFieldState<PXFieldOptions.CommitChanges>;
	UpdateServer: PXFieldState;
	UpdateNotification: PXFieldState;
	StorageProvider: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({adjustPageSize: true, fastFilterByAllFields: false})
export class UPStorageParameters extends PXView {
	@columnConfig({allowUpdate: false, allowFastFilter: false})
	Name: PXFieldState;
	@columnConfig({allowUpdate: false, allowFastFilter: false})
	Value: PXFieldState;
	ReloadParameters: PXActionState;
}