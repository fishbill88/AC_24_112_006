import { PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, createSingle, PXScreen, graphInfo, createCollection } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AP.APUpdateDiscounts', primaryView: 'Filter', hideFilesIndicator: true, hideNotesIndicator: true })
export class AP502500 extends PXScreen {

	Filter = createSingle(ItemFilter);
	Items = createCollection(SelectedItem);

}

export class ItemFilter extends PXView {
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	PendingDiscountDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ adjustPageSize: true, mergeToolbarWith: 'ScreenToolbar' })
export class SelectedItem extends PXView {
	@columnConfig({ allowUpdate: false, allowCheckAll: true })
	Selected: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DiscountID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DiscountSequenceID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Description: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DiscountedFor: PXFieldState;
	@columnConfig({ allowUpdate: false })
	BreakBy: PXFieldState;
	@columnConfig({ allowUpdate: false })
	StartDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	EndDate: PXFieldState;
}
