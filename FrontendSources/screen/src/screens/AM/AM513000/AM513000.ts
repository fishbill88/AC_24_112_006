import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	GridPreset,
} from 'client-controls';

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
})
export class Items extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	BatNbr: PXFieldState;
	TranDate: PXFieldState;
	@columnConfig({ hideViewLink: true }) OrderType: PXFieldState;
	ProdOrdID: PXFieldState;
	@columnConfig({ hideViewLink: true }) OperationID: PXFieldState;
	EmployeeID: PXFieldState;
	LaborTime: PXFieldState;
	LaborRate: PXFieldState;
	ExtCost: PXFieldState;
	ProjectID: PXFieldState;
	TaskID: PXFieldState;
	CostCodeID: PXFieldState;
	TimeCardStatus: PXFieldState;
	SiteID: PXFieldState;
	InventoryID: PXFieldState;
	LaborCodeID: PXFieldState;
}

export class TimeCardFilter extends PXView {
	ShowAll: PXFieldState<PXFieldOptions.CommitChanges>;
}

@graphInfo({ graphType: 'PX.Objects.AM.AMTimeCardCreate', primaryView: 'Items' })
export class AM513000 extends PXScreen {
	TimeCardFilter = createSingle(TimeCardFilter);
	Items = createCollection(Items);
}
