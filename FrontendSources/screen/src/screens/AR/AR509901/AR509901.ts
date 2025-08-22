import {
	createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, GridColumnType, TextAlign
} from "client-controls";

@graphInfo({ graphType: "PX.Objects.AR.ARIntegrityCheck", primaryView: "Filter", })
export class AR509901 extends PXScreen {

	@viewInfo({ containerName: "Selection" })
	Filter = createSingle(ARIntegrityCheckFilter);

	@viewInfo({ containerName: "Customers" })
	ARCustomerList = createCollection(Customer);

}

export class ARIntegrityCheckFilter extends PXView {

	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerClassID: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ syncPosition: true, mergeToolbarWith: "ScreenToolbar" })
export class Customer extends PXView {

	@columnConfig({ allowCheckAll: true, allowSort: false, allowNull: false, textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	Selected: PXFieldState;

	AcctCD: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CustomerClassID: PXFieldState;

	AcctName: PXFieldState;

}
