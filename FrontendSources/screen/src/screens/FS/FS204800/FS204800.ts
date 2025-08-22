import {
	graphInfo,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,
	createSingle
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.ManufacturerModelMaint', primaryView: 'ManufacturerModelRecords' })
export class FS204800 extends PXScreen {
	ManufacturerModelRecords = createSingle(FSManufacturerModel);
}

export class FSManufacturerModel extends PXView {
	ManufacturerID: PXFieldState<PXFieldOptions.CommitChanges>;
	ManufacturerModelCD: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	EquipmentTypeID: PXFieldState;
}
