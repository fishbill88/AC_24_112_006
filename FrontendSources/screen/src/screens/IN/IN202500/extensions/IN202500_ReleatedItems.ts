import {
	IN202500
} from '../IN202500';

import {
	PXView,
	createCollection,
	PXFieldState,
	PXFieldOptions,
	viewInfo,
	linkCommand,
	gridConfig,
	columnConfig,
	GridPreset
} from 'client-controls';

export interface IN202500_ReleatedItems extends IN202500 {}
export class IN202500_ReleatedItems {

	@viewInfo({ containerName: "Related Items" })
	RelatedItems = createCollection(RelatedItems);
}

@gridConfig({
	preset: GridPreset.Details
})
export class RelatedItems extends PXView {
	Relation: PXFieldState<PXFieldOptions.CommitChanges>;
	Rank: PXFieldState<PXFieldOptions.CommitChanges>;
	Tag: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewRelatedItem")
	RelatedInventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	Desc: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState;
	EffectiveDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpirationDate: PXFieldState<PXFieldOptions.CommitChanges>;
	Interchangeable: PXFieldState;
	Required: PXFieldState<PXFieldOptions.CommitChanges>;
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
}