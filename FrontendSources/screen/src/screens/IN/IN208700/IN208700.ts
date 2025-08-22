import { Messages as SysMessages } from 'client-controls/services/messages';
import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, GridPreset } from 'client-controls';

@graphInfo({graphType: 'PX.Objects.IN.INPICycleMaint', primaryView: 'PICycles', })
export class IN208700 extends PXScreen {
	@viewInfo({containerName: 'PI Cycle'})
	PICycles = createCollection(INPICycle);
}

// Views
@gridConfig({
	preset: GridPreset.Primary
})
export class INPICycle extends PXView  {
	CycleID: PXFieldState;
	Descr: PXFieldState;
	CountsPerYear: PXFieldState;
}
