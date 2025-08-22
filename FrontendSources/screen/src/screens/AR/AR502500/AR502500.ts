import {
	createCollection, createSingle,
	PXScreen, PXActionState, PXView, PXFieldState,
	graphInfo, viewInfo, gridConfig, columnConfig,
	GridColumnType, PXFieldOptions
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.AR.ARUpdateDiscounts", primaryView: "Filter",
	hideFilesIndicator: true, hideNotesIndicator: true
})
export class AR502500 extends PXScreen {
   	@viewInfo({containerName: "Selection"})
		  Filter = createSingle(ItemFilter);

   	@viewInfo({containerName: "Discount Sequences"})
	Items = createCollection(SelectedItem);
}

export class ItemFilter extends PXView {
	PendingDiscountDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	quickFilterFields: ['DiscountID', 'DiscountSequenceID', 'Description']
})
export class SelectedItem extends PXView {
	@columnConfig({ type: GridColumnType.CheckBox, allowCheckAll: true })
	Selected: PXFieldState;

	@columnConfig({ allowUpdate: false })
	DiscountID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	DiscountSequenceID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Description: PXFieldState;

	@columnConfig({ allowUpdate: false, allowNull: false })
	DiscountedFor: PXFieldState;

	@columnConfig({ allowUpdate: false, allowNull: false })
	BreakBy: PXFieldState;

	@columnConfig({ allowUpdate: false })
	StartDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	EndDate: PXFieldState;
}
