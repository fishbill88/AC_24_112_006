import { Messages as SysMessages } from 'client-controls/services/messages';
import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType,  RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.IN.INPriceClassMaint', primaryView: 'Records' })
export class IN209000 extends PXScreen {
	@viewInfo({containerName: 'Price Class'})
	Records = createCollection(INPriceClass);
}

// Views

@gridConfig({ mergeToolbarWith: 'ScreenToolbar' })
export class INPriceClass extends PXView  {
	PriceClassID: PXFieldState;
	Description: PXFieldState;
}
