import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, TextAlign } from "client-controls";

@graphInfo({ graphType: "PX.Objects.FA.FixedAssetCostEnq", primaryView: "Filter", pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues })
export class FA403000 extends PXScreen {

	@viewInfo({ containerName: "Selection" })
	Filter = createSingle(FixedAssetFilter);

	@viewInfo({ containerName: "Accounts/Subs" })
	Amts = createCollection(Amounts);

}

export class FixedAssetFilter extends PXView {

	AssetID: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	BookID: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ syncPosition: true, mergeToolbarWith: "ScreenToolbar" })
export class Amounts extends PXView {

	BookID: PXFieldState;
	AccountID: PXFieldState;
	AcctDescr: PXFieldState;
	SubID: PXFieldState;
	SubDescr: PXFieldState;
	BranchID: PXFieldState;

	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	ItdAmt: PXFieldState;

	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	YtdAmt: PXFieldState;

	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	PtdAmt: PXFieldState;

}
