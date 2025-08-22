import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	GridPreset,
} from 'client-controls';

export class BucketLookup extends PXView {
	BucketID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	QtyOnHand: PXFieldState;
	ProductManagerID: PXFieldState;
	SafetyStock: PXFieldState;
	ReplenishmentSource: PXFieldState;
	PreferredVendorID: PXFieldState;
	LeadTime: PXFieldState;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
})
export class BucketDetailRecords extends PXView {
	FromDate: PXFieldState;
	Bucket: PXFieldState;
	ToDate: PXFieldState;
	BeginQty: PXFieldState;
	ActualSupply: PXFieldState;
	ActualDemand: PXFieldState;
	NetQty: PXFieldState;
	PlannedSupply: PXFieldState;
	PlannedDemand: PXFieldState;
	EndQty: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.MRPBucketInq', primaryView: 'BucketLookup' })
export class AM401200 extends PXScreen {
	BucketLookup = createSingle(BucketLookup);
	BucketDetailRecords = createCollection(BucketDetailRecords);
}
