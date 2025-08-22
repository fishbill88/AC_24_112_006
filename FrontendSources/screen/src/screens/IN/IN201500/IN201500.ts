import { Messages as SysMessages } from 'client-controls/services/messages';
import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig,headerDescription,ICurrencyInfo,disabled,selectorSettings,PXFieldOptions,linkCommand,columnConfig,GridColumnShowHideMode,GridColumnType } from 'client-controls';

@graphInfo({graphType: 'PX.Objects.IN.INAvailabilitySchemeMaint', primaryView: 'Schemes', })
export class IN201500 extends PXScreen {
	@viewInfo({containerName: 'Availability Scheme'})
	Schemes = createSingle(INAvailabilityScheme);
}

// Views

export class INAvailabilityScheme extends PXView  {
	AvailabilitySchemeID: PXFieldState;
	@headerDescription
	Description: PXFieldState;
	InclQtyINIssues: PXFieldState;
	InclQtySOPrepared: PXFieldState;
	InclQtySOBooked: PXFieldState;
	InclQtySOShipped: PXFieldState;
	InclQtySOShipping: PXFieldState;
	InclQtyINAssemblyDemand: PXFieldState;
	InclQtySOBackOrdered: PXFieldState;
	InclQtyFSSrvOrdPrepared: PXFieldState;
	InclQtyFSSrvOrdBooked: PXFieldState;
	InclQtyProductionDemandPrepared: PXFieldState;
	InclQtyProductionDemand: PXFieldState;
	InclQtyProductionAllocated: PXFieldState;
	InclQtyFSSrvOrdAllocated: PXFieldState;
	InclQtyINReceipts: PXFieldState;
	InclQtyInTransit: PXFieldState;
	InclQtyPOReceipts: PXFieldState;
	InclQtyPOPrepared: PXFieldState;
	InclQtyPOOrders: PXFieldState;
	InclQtyFixedSOPO: PXFieldState;
	InclQtyINAssemblySupply: PXFieldState;
	InclQtySOReverse: PXFieldState;
	InclQtyProductionSupplyPrepared: PXFieldState;
	InclQtyProductionSupply: PXFieldState;
}
