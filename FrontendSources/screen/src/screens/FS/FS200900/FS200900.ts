import {
	graphInfo,
	gridConfig,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.LicenseTypeMaint', primaryView: 'LicenseTypeRecords' })
export class FS200900 extends PXScreen {
	LicenseTypeRecords = createCollection(FSLicenseType);
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar"
})
export class FSLicenseType extends PXView {
	@columnConfig({ hideViewLink: true })
	LicenseTypeCD: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
}
