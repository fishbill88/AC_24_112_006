import {
	SO301000
} from '../SO301000';

import {
	PXView,
	createCollection,
	createSingle,
	PXFieldState,
	PXFieldOptions,
	localizable,
	linkCommand,
	columnConfig,
	viewInfo,
	gridConfig
} from 'client-controls';


@localizable
export class RelatedItemsPanelHeaders {
	static AddRelatedItems = "Add Related Items";
}

export interface SO301000_RelatedItems extends SO301000 {}
export class SO301000_RelatedItems {
	RelatedItemsPanelHeaders = RelatedItemsPanelHeaders;

	@viewInfo({containerName: "Add Related Items"})
	RelatedItemsFilter = createSingle(RelatedItemsFilter);

	@viewInfo({containerName: "All Related Items"})
	AllRelatedItems = createCollection(RelatedItems);

	@viewInfo({containerName: "Substitute Items"})
	SubstituteItems = createCollection(RelatedItems);

	@viewInfo({containerName: "Up Sell Items"})
	UpSellItems = createCollection(RelatedItems);

	@viewInfo({containerName: "Add Related Items"})
	CrossSellItems = createCollection(RelatedItems);

	@viewInfo({containerName: "Other Related Items"})
	OtherRelatedItems = createCollection(RelatedItems);
}

export class RelatedItemsFilter extends PXView {
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitPrice: PXFieldState;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState;
	CuryExtPrice: PXFieldState;
	AvailableQty: PXFieldState;
	SiteID: PXFieldState;
	KeepOriginalPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	OnlyAvailableItems: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowForAllWarehouses: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowSubstituteItems: PXFieldState;
	ShowUpSellItems: PXFieldState;
	ShowCrossSellItems: PXFieldState;
	ShowOtherRelatedItems: PXFieldState;
	ShowAllRelatedItems: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	adjustPageSize: true,
	allowInsert: false,
	allowDelete: false
})
export class RelatedItems extends PXView {
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	QtySelected: PXFieldState;
	Rank: PXFieldState;
	Relation: PXFieldState;
	Tag: PXFieldState;
	@linkCommand("ViewRelatedItem") RelatedInventoryID: PXFieldState;
	SubItemID: PXFieldState;
	SubItemCD: PXFieldState<PXFieldOptions.Hidden>;
	Desc: PXFieldState;
	@columnConfig({hideViewLink: true}) UOM: PXFieldState;
	CuryUnitPrice: PXFieldState;
	CuryExtPrice: PXFieldState;
	PriceDiff: PXFieldState;
	AvailableQty: PXFieldState;
	@columnConfig({hideViewLink: true}) SiteID: PXFieldState;
	SiteCD: PXFieldState<PXFieldOptions.Hidden>;
	Interchangeable: PXFieldState;
	Required: PXFieldState;
}
