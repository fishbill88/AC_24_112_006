import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, GridColumnType, TextAlign } from "client-controls";

@graphInfo({ graphType: "PX.Objects.FA.DisposalProcess", primaryView: "Filter", })
export class FA505000 extends PXScreen {

	@viewInfo({ containerName: "Options" })
	Filter = createSingle(ProcessAssetFilter);

	@viewInfo({ containerName: "Assets to Dispose" })
	Assets = createCollection(FixedAsset);

}

export class ProcessAssetFilter extends PXView {

	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	ParentAssetID: PXFieldState<PXFieldOptions.CommitChanges>;
	BookID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisposalDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DisposalPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisposalMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisposalAmt: PXFieldState;
	DisposalAmtMode: PXFieldState<PXFieldOptions.CommitChanges>;
	DisposalAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisposalSubID: PXFieldState;
	ActionBeforeDisposal: PXFieldState<PXFieldOptions.CommitChanges>;
	Reason: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ syncPosition: true, mergeToolbarWith: "ScreenToolbar" })
export class FixedAsset extends PXView {

	@columnConfig({ allowCheckAll: true, textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	Selected: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	BranchID: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	ClassID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	AssetCD: PXFieldState;

	Description: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	ParentAssetID: PXFieldState;

	@columnConfig({ allowUpdate: false, allowNull: false, textAlign: TextAlign.Right })
	FADetails__CurrentCost: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	DisposalAmt: PXFieldState;

	FADetails__ReceiptDate: PXFieldState;

	@columnConfig({ allowUpdate: false, textAlign: TextAlign.Right })
	UsefulLife: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	FAAccountID: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	FASubID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	FADetails__TagNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	Account__AccountClassID: PXFieldState;

}
