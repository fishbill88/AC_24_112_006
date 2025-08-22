import {
	PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, graphInfo, PXScreen, createSingle, createCollection, PXActionState
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.AP.APDiscountMaint', primaryView: 'Filter' })
export class AP204000 extends PXScreen {

	ViewAPDiscountSequence: PXActionState;

	Filter = createSingle(Vendor);
	CurrentVendor = createSingle(Vendor);
	CurrentDiscounts = createCollection(APDiscount);

}

export class Vendor extends PXView {
	AcctCD: PXFieldState<PXFieldOptions.CommitChanges>;
	LineDiscountTarget: PXFieldState;
}

@gridConfig({ initNewRow: true })
export class APDiscount extends PXView {
	@linkCommand('ViewAPDiscountSequence')
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	DiscountID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowUpdate: false })
	Description: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Type: PXFieldState;
	@columnConfig({ allowUpdate: false })
	ApplicableTo: PXFieldState;
	@columnConfig({ allowUpdate: false })
	IsManual: PXFieldState;
	@columnConfig({ allowUpdate: false })
	ExcludeFromDiscountableAmt: PXFieldState;
	@columnConfig({ allowUpdate: false })
	SkipDocumentDiscounts: PXFieldState;
	@columnConfig({ allowUpdate: false })
	IsAutoNumber: PXFieldState;
	@columnConfig({ allowUpdate: false })
	LastNumber: PXFieldState;
}
