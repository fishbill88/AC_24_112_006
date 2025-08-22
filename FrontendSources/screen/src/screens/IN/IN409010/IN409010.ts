import {
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,

	createSingle,
	createCollection,

	graphInfo,
	viewInfo,
	columnConfig,
	gridConfig,
	GridPreset,
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.IN.StoragePlaceEnq', primaryView: 'Filter' })
export class IN409010 extends PXScreen {
	@viewInfo({ containerName: "Filter" })
	Filter = createSingle(Filter);

	@viewInfo({ containerName: "Storages" })
	storages = createCollection(Storages);
}

export class Filter extends PXView {
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	StorageType: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	CartID: PXFieldState<PXFieldOptions.CommitChanges>;
	StorageID: PXFieldState<PXFieldOptions.CommitChanges>;

	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpandByLotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	allowUpdate: false
})
export class Storages extends PXView {
	@columnConfig({ allowFastFilter: true })
	SiteID: PXFieldState<PXFieldOptions.Disabled>;

	StorageCD: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFastFilter: true })
	InventoryID: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFastFilter: true })
	InventoryDescr: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	SubItemID: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	LotSerialNbr: PXFieldState<PXFieldOptions.Disabled>;

	ExpireDate: PXFieldState<PXFieldOptions.Disabled>;

	Qty: PXFieldState<PXFieldOptions.Disabled>;
	QtyPickedToCart: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	BaseUnit: PXFieldState<PXFieldOptions.Disabled>;
}
