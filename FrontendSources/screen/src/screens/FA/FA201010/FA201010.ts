import {
	PXScreen, createCollection, graphInfo, PXView,
	PXFieldState, PXFieldOptions, columnConfig, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.FA.AssetTypeMaint', primaryView: 'AssetTypes' })
export class FA201010 extends PXScreen {
	AssetTypes = createCollection(FAType);
}

@gridConfig({ adjustPageSize: true, syncPosition: true, mergeToolbarWith: 'ScreenToolbar' })
export class FAType extends PXView {

	@columnConfig({ hideViewLink: true })
	AssetTypeID: PXFieldState<PXFieldOptions.CommitChanges>;

	Description: PXFieldState;
	IsTangible: PXFieldState<PXFieldOptions.CommitChanges>;
	Depreciable: PXFieldState<PXFieldOptions.CommitChanges>;
}
