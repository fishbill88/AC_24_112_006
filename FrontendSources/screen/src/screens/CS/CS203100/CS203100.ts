import {
	createCollection,
	PXScreen,
	graphInfo,
	PXView,
	columnConfig,
	gridConfig,
	PXFieldState,
	PXFieldOptions,
	GridPreset
} from "client-controls";

@graphInfo({graphType: "PX.Objects.IN.INUnitMaint", primaryView: "Unit"})
export class CS203100 extends PXScreen {

	Unit = createCollection(INUnit);
}

@gridConfig({
	preset: GridPreset.Primary,
	mergeToolbarWith: "ScreenToolbar"
})
export class INUnit extends PXView  {
	@columnConfig({allowNull: false, visible: false, hideViewLink: true})
	UnitType : PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({allowNull: false, visible: false})
	ItemClassID : PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({allowNull: false, visible: false})
	InventoryID : PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({hideViewLink: true})
	FromUnit : PXFieldState;
	@columnConfig({hideViewLink: true})
	ToUnit : PXFieldState;
	@columnConfig({allowNull: false})
	UnitMultDiv : PXFieldState;
	@columnConfig({allowNull: false})
	UnitRate : PXFieldState;
}