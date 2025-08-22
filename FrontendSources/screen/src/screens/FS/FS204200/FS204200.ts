import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.VehicleTypeMaint', primaryView: 'VehicleTypeRecords' })
export class FS204200 extends PXScreen {
	VehicleTypeRecords = createSingle(FSVehicleType);
	Mapping = createCollection(CSAttributeGroup);
}

export class FSVehicleType extends PXView {
	VehicleTypeCD: PXFieldState;
	Descr: PXFieldState;
}

export class CSAttributeGroup extends PXView {
	IsActive: PXFieldState;
	AttributeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	SortOrder: PXFieldState;
	Required: PXFieldState;
	CSAttribute__IsInternal: PXFieldState;
	ControlType: PXFieldState;
	DefaultValue: PXFieldState;
}
