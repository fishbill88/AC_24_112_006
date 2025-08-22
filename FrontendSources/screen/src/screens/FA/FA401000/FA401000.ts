import {
	createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, PXActionState, TextAlign
} from "client-controls";

@graphInfo({ graphType: "PX.Objects.FA.AccBalanceByAssetInq", primaryView: "Filter", pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues })
export class FA401000 extends PXScreen {

	ViewAsset: PXActionState;

	@viewInfo({ containerName: "Selection" })
	Filter = createSingle(AccBalanceByAssetFilter);

	@viewInfo({ containerName: "Assets" })
	Amts = createCollection(Amounts);
}

export class AccBalanceByAssetFilter extends PXView {

	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	BookID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	Balance: PXFieldState;

}

@gridConfig({ syncPosition: true, mergeToolbarWith: "ScreenToolbar" })
export class Amounts extends PXView {

	@linkCommand("ViewAsset")
	AssetID: PXFieldState;

	Description: PXFieldState;
	Status: PXFieldState;
	ClassID: PXFieldState;
	DepreciateFromDate: PXFieldState;
	BranchID: PXFieldState;
	Department: PXFieldState;

	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	ItdAmt: PXFieldState;

	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	YtdAmt: PXFieldState;

	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	PtdAmt: PXFieldState;

}
