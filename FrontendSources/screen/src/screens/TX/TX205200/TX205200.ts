import { createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, PXFieldOptions } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.TX.TaxBucketMaint', primaryView: 'Bucket', })
export class TX205200 extends PXScreen {

	Bucket = createSingle(TaxBucketMaster);
	BucketLine = createCollection(TaxBucketLine);

}

export class TaxBucketMaster extends PXView {

	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	BucketID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxReportRevisionID: PXFieldState<PXFieldOptions.CommitChanges>;
	BucketType: PXFieldState;

}

export class TaxBucketLine extends PXView {

	LineNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxReportLine__LineType: PXFieldState;
	TaxReportLine__LineMult: PXFieldState;

}
