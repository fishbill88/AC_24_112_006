import { Messages as SysMessages } from 'client-controls/services/messages';
import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, GridPreset } from 'client-controls';

@graphInfo({graphType: 'PX.Objects.IN.INMovementClassMaint', primaryView: 'MovementClasses', })
export class IN208600 extends PXScreen {
	@viewInfo({containerName: 'Movement Class'})
	MovementClasses = createCollection(INMovementClass);

}

// Views

@gridConfig({
	preset: GridPreset.Primary,
	initNewRow: true
})
export class INMovementClass extends PXView  {
	MovementClassID: PXFieldState;
	Descr: PXFieldState;
	CountsPerYear: PXFieldState<PXFieldOptions.CommitChanges>;
	MaxTurnoverPct: PXFieldState<PXFieldOptions.CommitChanges>;
}
