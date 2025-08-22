import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, PXActionState, TextAlign } from "client-controls";

@graphInfo({ graphType: "PX.Objects.FA.AssetSummary", primaryView: "Filter", })
export class FA402000 extends PXScreen {

	@viewInfo({ containerName: "Selection" })
	Filter = createSingle(AssetFilter);

	@viewInfo({ containerName: "Fixed Assets Summary" })
	assets = createCollection(FixedAsset);

	@viewInfo({ containerName: "Dispose Parameters" })
	DispParams = createSingle(DisposeParams);
}

export class AssetFilter extends PXView {

	ClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	AssetTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	PropertyType: PXFieldState<PXFieldOptions.CommitChanges>;
	Condition: PXFieldState<PXFieldOptions.CommitChanges>;
	AcqDateFrom: PXFieldState<PXFieldOptions.CommitChanges>;
	AcqDateTo: PXFieldState<PXFieldOptions.CommitChanges>;
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	BuildingID: PXFieldState<PXFieldOptions.CommitChanges>;
	Floor: PXFieldState<PXFieldOptions.CommitChanges>;
	Room: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Department: PXFieldState<PXFieldOptions.CommitChanges>;
	PONumber: PXFieldState<PXFieldOptions.CommitChanges>;
	ReceiptType: PXFieldState<PXFieldOptions.CommitChanges>;
	ReceiptNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	BillNumber: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ mergeToolbarWith: "ScreenToolbar" })
export class FixedAsset extends PXView {

	AssetCD: PXFieldState;
	ParentAssetID: PXFieldState;
	ClassID: PXFieldState;
	Description: PXFieldState;
	AssetTypeID: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	UsefulLife: PXFieldState;

	FADetails__DepreciateFromDate: PXFieldState;

	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	FADetails__AcquisitionCost: PXFieldState;

	@columnConfig({ allowNull: false })
	FADetails__PropertyType: PXFieldState;

	@columnConfig({ allowNull: false })
	FADetails__Condition: PXFieldState;

	FADetails__ReceiptNbr: PXFieldState;
	FADetails__PONumber: PXFieldState;
	FADetails__BillNumber: PXFieldState;
	FALocationHistory__LocationID: PXFieldState;
	FALocationHistory__BuildingID: PXFieldState;
	FALocationHistory__Floor: PXFieldState;
	FALocationHistory__Room: PXFieldState;
	FALocationHistory__EmployeeID: PXFieldState;
	FALocationHistory__Department: PXFieldState;

}

export class DisposeParams extends PXView {

	DisposalDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DisposalPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisposalAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	DisposalMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisposalAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisposalSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	Reason: PXFieldState;

}
