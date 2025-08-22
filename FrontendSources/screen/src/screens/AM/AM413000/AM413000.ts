import {
	PXScreen,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AM.MultiLevelBomInq', primaryView: 'Filter' })
export class AM413000 extends PXScreen {
	Filter = createSingle(AMMultiLevelBomFilter);
}

export class AMMultiLevelBomFilter extends PXView {
	BOMID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisionID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	BOMDate: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreReplenishmentSettings: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeBomsOnHold: PXFieldState<PXFieldOptions.CommitChanges>;
	RollCosts: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreMinMaxLotSizeValues: PXFieldState<PXFieldOptions.CommitChanges>;
	UseCurrentInventoryCost: PXFieldState<PXFieldOptions.CommitChanges>;
}
