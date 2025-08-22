import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	headerDescription,
} from 'client-controls';

export class CurrentBucket extends PXView {
	BucketID: PXFieldState;
	ActiveFlg: PXFieldState<PXFieldOptions.CommitChanges>;
	@headerDescription Descr: PXFieldState;
}

@gridConfig({
	initNewRow: true,
	syncPosition: true,
})
export class BucketRecords extends PXView {
	Bucket: PXFieldState;
	Value: PXFieldState;
	Interval: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.MRPBucketMaint', primaryView: 'CurrentBucket' })
export class AM201200 extends PXScreen {
	CurrentBucket = createSingle(CurrentBucket);

	BucketRecords = createCollection(BucketRecords);
}
