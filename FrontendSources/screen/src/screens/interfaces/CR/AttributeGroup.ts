import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	GridPreset,
} from "client-controls";

@gridConfig({ preset: GridPreset.Details, fastFilterByAllFields: false })
export class CSAttributeGroup extends PXView {
	IsActive: PXFieldState;
	AttributeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	SortOrder: PXFieldState;
	Required: PXFieldState;
	CSAttribute__IsInternal: PXFieldState;
	CSAttribute__ContainsPersonalData: PXFieldState;
	ControlType: PXFieldState;
	DefaultValue: PXFieldState;
}
