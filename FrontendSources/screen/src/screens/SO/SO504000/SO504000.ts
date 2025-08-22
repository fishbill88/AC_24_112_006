import {
	PXScreen,
	createSingle,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	columnConfig,
	linkCommand,
	PXFieldOptions,
	PXActionState,
	viewInfo,
	gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.SO.SOCreateIntercompanySalesOrders', primaryView: 'Filter' })
export class SO504000 extends PXScreen {

	ViewPODocument: PXActionState;
	ViewSOOrder: PXActionState;

	@viewInfo({ containerName: "Filter" })
	Filter = createSingle(Filter);

	@viewInfo({ containerName: "Documents" })
	Documents = createCollection(Documents);
}

export class Filter extends PXView {
	PODocType: PXFieldState<PXFieldOptions.CommitChanges>;
	DocDate: PXFieldState<PXFieldOptions.CommitChanges>;
	IntercompanyOrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	SellingCompany: PXFieldState<PXFieldOptions.CommitChanges>;
	PurchasingCompany: PXFieldState<PXFieldOptions.CommitChanges>;
	CopyProjectDetails: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	adjustPageSize: true,
	syncPosition: true,
	mergeToolbarWith: 'ScreenToolbar'
})
export class Documents extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	@columnConfig({ hideViewLink: true }) VendorID: PXFieldState;
	@columnConfig({ hideViewLink: true }) BranchID: PXFieldState;
	DocType: PXFieldState;
	@linkCommand('ViewPODocument') DocNbr: PXFieldState;
	DocDate: PXFieldState;
	FinPeriodID: PXFieldState;
	ExpectedDate: PXFieldState;
	@columnConfig({ hideViewLink: true }) CuryID: PXFieldState;
	CuryDocTotal: PXFieldState;
	CuryDiscTot: PXFieldState;
	CuryTaxTotal: PXFieldState;
	DocQty: PXFieldState;
	@columnConfig({ hideViewLink: true }) EmployeeID: PXFieldState;
	@columnConfig({ hideViewLink: true }) OwnerID: PXFieldState;
	@columnConfig({ hideViewLink: true }) WorkgroupID: PXFieldState;
	DocDesc: PXFieldState;
	Excluded: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand('ViewSOOrder') OrderNbr: PXFieldState;
}