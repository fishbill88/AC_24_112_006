import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.EquipmentTypeMaint', primaryView: 'EquipmentTypeRecords' })
export class FS200800 extends PXScreen {
	EquipmentTypeRecords = createSingle(FSEquipmentType);
	Mapping = createCollection(CSAttributeGroup);
}

export class FSEquipmentType extends PXView {
	EquipmentTypeCD: PXFieldState;
	Descr: PXFieldState;
	RequireBranchLocation: PXFieldState;
}

export class CSAttributeGroup extends PXView {
	IsActive: PXFieldState;
	AttributeID: PXFieldState;
	Description: PXFieldState;
	SortOrder: PXFieldState;
	Required: PXFieldState;
	CSAttribute__IsInternal: PXFieldState;
	ControlType: PXFieldState;
	DefaultValue: PXFieldState;
}
