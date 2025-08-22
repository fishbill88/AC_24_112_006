import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, TextAlign } from "client-controls";

@graphInfo({graphType: "PX.Objects.AR.ARPriceClassMaint", primaryView: "Records" })
export class AR208000 extends PXScreen {

	Records = createCollection(ARPriceClass);
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar"
})
export class ARPriceClass extends PXView  {

	PriceClassID : PXFieldState;
	Description : PXFieldState;
}
