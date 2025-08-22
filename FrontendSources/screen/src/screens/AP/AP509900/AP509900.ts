import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, PXFieldOptions, createCollection, columnConfig, linkCommand, PXActionState, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AP.APIntegrityCheck', primaryView: 'Filter' })
export class AP509900 extends PXScreen {

	Filter = createSingle(APIntegrityCheckFilter);
	APVendorList = createCollection(Vendor);

}

export class APIntegrityCheckFilter extends PXView {

	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorClassID: PXFieldState;
	ViewVendor: PXActionState;

}

@gridConfig({ adjustPageSize: true, syncPosition: true, mergeToolbarWith: 'ScreenToolbar' })
export class Vendor extends PXView {

	@columnConfig({ allowNull: true, allowCheckAll: true, allowSort: false })
	Selected: PXFieldState;

	@linkCommand("ViewVendor")
	AcctCD: PXFieldState;

	@columnConfig({ hideViewLink: true })
	VendorClassID: PXFieldState;
	AcctName: PXFieldState;

}
