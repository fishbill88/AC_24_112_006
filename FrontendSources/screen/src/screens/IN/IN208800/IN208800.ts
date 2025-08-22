import { createCollection, PXScreen, graphInfo, viewInfo, PXView, PXFieldState, gridConfig, columnConfig, GridPreset } from 'client-controls';

@graphInfo({graphType: 'PX.Objects.IN.INReplenishmentClassMaint', primaryView: 'Classes' })
export class IN208800 extends PXScreen {
	@viewInfo({containerName: 'Replenishment Class'})
	Classes = createCollection(INReplenishmentClass);

}

// Views

@gridConfig({
	preset: GridPreset.Primary,
	initNewRow: true
})
export class INReplenishmentClass extends PXView  {
	ReplenishmentClassID: PXFieldState;
	Descr: PXFieldState;
	@columnConfig({allowNull: false})
	ReplenishmentSource: PXFieldState;
}
