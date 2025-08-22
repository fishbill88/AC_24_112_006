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

export class OrderType extends PXView {
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	Active: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	Function: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CurrentOrderType extends PXView {
	ProdNumberingID: PXFieldState;

	WIPAcctID: PXFieldState;
	WIPSubID: PXFieldState;
	WIPVarianceAcctID: PXFieldState;
	WIPVarianceSubID: PXFieldState;

	DefaultCostMethod: PXFieldState;
	ExcludeFromMRP: PXFieldState;
	SubstituteWorkCenters: PXFieldState;
	AutoBackwardReporting: PXFieldState;

	ProductionReportID: PXFieldState;

	CheckSchdMatlAvailability: PXFieldState;


	ScrapSource: PXFieldState;
	ScrapSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ScrapLocationID: PXFieldState;

	CopyNotesItem: PXFieldState;
	CopyNotesOper: PXFieldState;
	CopyNotesMatl: PXFieldState;
	CopyNotesStep: PXFieldState;
	CopyNotesTool: PXFieldState;
	CopyNotesOvhd: PXFieldState;

	UnderIssueMaterial: PXFieldState;
	BackflushUnderIssueMaterial: PXFieldState;

	OverIssueMaterial: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeUnreleasedOverIssueMaterial: PXFieldState;

	IssueMaterialOnTheFly: PXFieldState;
	MoveCompletedOrders: PXFieldState;
	ExceedQtyOperations: PXFieldState;
	OverCompleteOrders: PXFieldState;
	DefaultOperationMoveQty: PXFieldState;
	PreassignLotSerial: PXFieldState<PXFieldOptions.CommitChanges>;
	ParentLotSerialRequired: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class OrderTypeAttributes extends PXView {
	OrderType: PXFieldState;
	LineNbr: PXFieldState;
	@columnConfig({ hideViewLink: true }) AttributeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Label: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	Enabled: PXFieldState;
	TransactionRequired: PXFieldState;
	Value: PXFieldState;
}


@graphInfo({ graphType: 'PX.Objects.AM.AMOrderTypeMaint', primaryView: 'OrderType' })
export class AM201100 extends PXScreen {
	OrderType = createSingle(OrderType);
	CurrentOrderType = createSingle(CurrentOrderType);
	OrderTypeAttributes = createCollection(OrderTypeAttributes);
}
