import {
	PXScreen, createSingle, createCollection, graphInfo, PXView, PXFieldState, gridConfig, columnConfig, linkCommand, PXActionState, PXFieldOptions
} from 'client-controls';

@gridConfig({ adjustPageSize: true, syncPosition: true, mergeToolbarWith: 'ScreenToolbar' })
export class FABookBalance extends PXView {

	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;

	@columnConfig({ hideViewLink: true })
	FixedAsset__BranchID: PXFieldState;

	@linkCommand('ViewAsset')
	@columnConfig({ allowUpdate: false })
	AssetID: PXFieldState;

	FixedAsset__Description: PXFieldState;

	@linkCommand('ViewClass')
	@columnConfig({ allowUpdate: false })
	ClassID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	FixedAsset__ParentAssetID: PXFieldState;

	@linkCommand('ViewBook')
	@columnConfig({ allowUpdate: false })
	BookID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CurrDeprPeriod: PXFieldState;

	@columnConfig({ allowNull: false })
	YtdDeprBase: PXFieldState;

	@columnConfig({ allowUpdate: false })
	FixedAsset__BaseCuryID: PXFieldState;

	FADetails__ReceiptDate: PXFieldState;

	FixedAsset__UsefulLife: PXFieldState;

	@columnConfig({ hideViewLink: true })
	FixedAsset__FAAccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	FixedAsset__FASubID: PXFieldState;

	FADetails__TagNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	Account__AccountClassID: PXFieldState;
}

export class Filter extends PXView {
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	BookID: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	ClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	ParentAssetID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@graphInfo({ graphType: 'PX.Objects.FA.CalcDeprProcess', primaryView: 'Filter' })
export class FA502000 extends PXScreen {

	ViewBook: PXActionState;
	ViewAsset: PXActionState;
	ViewClass: PXActionState;

	Filter = createSingle(Filter);

	Balances = createCollection(FABookBalance);
}
