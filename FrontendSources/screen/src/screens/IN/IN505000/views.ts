import { PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, GridPreset} from 'client-controls';

// Views

export class INRecalculateInventoryFilter extends PXView {
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	RebuildHistory: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	ReplanBackorders: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowOnlyAllocatedItems: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry
})
export class InventoryItemCommon extends PXView {
	@columnConfig({allowCheckAll: true})
	Selected: PXFieldState;
	@columnConfig({allowUpdate: false})
	InventoryCD: PXFieldState;
	@columnConfig({allowUpdate: false, nullText: '0.0'})
	INSiteStatusSummary__QtyOnHand: PXFieldState;
	@columnConfig({allowUpdate: false, nullText: '0.0'})
	INSiteStatusSummary__QtyAvail: PXFieldState;
	@columnConfig({allowUpdate: false, nullText: '0.0'})
	INSiteStatusSummary__QtyNotAvail: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true
})
export class InventoryLinkFilter extends PXView {
	@columnConfig({allowUpdate: false, hideViewLink: true})
	InventoryID: PXFieldState;
	@columnConfig({allowUpdate: false})
	Descr: PXFieldState;
}
