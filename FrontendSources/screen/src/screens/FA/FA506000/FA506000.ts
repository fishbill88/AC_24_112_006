import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXView, PXFieldState, PXFieldOptions, columnConfig, GridColumnType, TextAlign } from "client-controls";

@graphInfo({ graphType: "PX.Objects.FA.SplitProcess", primaryView: "Filter", })
export class FA506000 extends PXScreen {

	@viewInfo({ containerName: "Options" })
	Filter = createSingle(SplitFilter);

	@viewInfo({ containerName: "Split Assets" })
	Splits = createCollection(FixedAsset);
}

export class SplitFilter extends PXView {

	AssetID: PXFieldState<PXFieldOptions.CommitChanges>;
	Cost: PXFieldState<PXFieldOptions.Disabled>;
	Qty: PXFieldState<PXFieldOptions.Disabled>;
	SplitDate: PXFieldState<PXFieldOptions.CommitChanges>;
	SplitPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	DeprBeforeSplit: PXFieldState;

}

export class FixedAsset extends PXView {

	@columnConfig({ type: GridColumnType.CheckBox, visible: false })
	Selected: PXFieldState;

	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	Cost: PXFieldState;

	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	SplittedQty: PXFieldState;

	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	Ratio: PXFieldState;

	SplittedAssetCD: PXFieldState;

}
