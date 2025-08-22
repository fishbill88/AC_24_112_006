import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	GridColumnDisplayMode,
	GridPreset
} from "client-controls";

export class MasterView extends PXView {
	ItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Primary,
	autoAdjustColumns: true,
})
export class OptionMappings extends PXView {
	BCSyncStatus__ExternDescription: PXFieldState;
	ExternID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExternalOptionName: PXFieldState<PXFieldOptions.CommitChanges>;
	ExternalOptionValue: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ displayMode: GridColumnDisplayMode.Text })
	MappedAttributeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ displayMode: GridColumnDisplayMode.Text })
	MappedValue: PXFieldState<PXFieldOptions.CommitChanges>;
}
