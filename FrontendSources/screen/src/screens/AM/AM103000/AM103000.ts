import {
	PXScreen,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AM.EstimateSetup', primaryView: 'AMEstimateSetupRecord' })
export class AM103000 extends PXScreen {
	AMEstimateSetupRecord = createSingle(AMEstimateSetupRecord);
}

export class AMEstimateSetupRecord extends PXView {
	EstimateNumberingID: PXFieldState;
	DefaultRevisionID: PXFieldState;
	AutoNumberRevisionID: PXFieldState;
	DefaultEstimateClassID: PXFieldState;
	DefaultWorkCenterID: PXFieldState;
	DefaultOrderType: PXFieldState;
	NewRevisionIsPrimary: PXFieldState;
	UpdateAllRevisions: PXFieldState;
	UpdatePriceInfo: PXFieldState;
}
