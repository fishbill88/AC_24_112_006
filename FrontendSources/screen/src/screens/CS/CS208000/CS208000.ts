import {
	PXScreen,
	createSingle,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	viewInfo,
	headerDescription
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CS.ShipTermsMaint', primaryView: 'ShipTermsCurrent' })
export class CS208000 extends PXScreen {

	ViewDocument: PXActionState;

	@viewInfo({ containerName: "Shipping Terms Summary" })
	ShipTermsCurrent = createSingle(ShipTermsCurrent);
	@viewInfo({ containerName: "Terms Details" })
	ShipTermsDetail = createCollection(ShipTermsDetail);
}

export class ShipTermsCurrent extends PXView {
	ShipTermsID: PXFieldState;
	@headerDescription
	Description: PXFieldState;
	FreightAmountSource: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ShipTermsDetail extends PXView {
	BreakAmount: PXFieldState;
	FreightCostPercent: PXFieldState;
	InvoiceAmountPercent: PXFieldState;
	ShippingHandling: PXFieldState;
	LineHandling: PXFieldState;
}