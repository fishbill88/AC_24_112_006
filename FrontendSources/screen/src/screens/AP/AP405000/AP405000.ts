import { PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, graphInfo, PXScreen, createSingle, createCollection } from "client-controls";

@graphInfo({ graphType: 'PX.Objects.AP.AP1099DetailEnq', primaryView: 'YearVendorHeader' })
export class AP405000 extends PXScreen {
	YearVendorHeader = createSingle(AP1099YearMaster);
	YearVendorSummary = createCollection(AP1099Box);
}

export class AP1099YearMaster extends PXView {
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	FinYear: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ syncPosition: true, adjustPageSize: true })
export class AP1099Box extends PXView {
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	BoxNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Descr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AP1099History__HistAmt: PXFieldState;
}
