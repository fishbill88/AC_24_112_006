import {
	PXScreen,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
} from 'client-controls';

export class Setup extends PXView {
	ExceptionDaysBefore: PXFieldState;
	ExceptionDaysAfter: PXFieldState;

	ForecastPlanHorizon: PXFieldState;
	ForecastNumberingID: PXFieldState;

	MPSFence: PXFieldState;
	DefaultMPSTypeID: PXFieldState;

	AMPlanningHorizon: PXFieldState;
	PlanOrderType: PXFieldState;
	GracePeriod: PXFieldState;
	StockingMethod: PXFieldState;
	PurchaseCalendarID: PXFieldState;
	IncludeOnHoldSalesOrder: PXFieldState;
	IncludeOnHoldPurchaseOrder: PXFieldState;
	IncludeOnHoldProductionOrder: PXFieldState;
	IncludeOnHoldKitAssemblies: PXFieldState;
	UseFixMfgLeadTime: PXFieldState;
	IncludeExpiredBlanketSalesOrders: PXFieldState;

	UseDaysSupplytoConsolidateOrders: PXFieldState<PXFieldOptions.CommitChanges>;
	UseLongTermConsolidationBucket: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsolidateAfterDays: PXFieldState;
	BucketDays: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.MRPSetupMaint', primaryView: 'setup' })
export class AM100000 extends PXScreen {
	setup = createSingle(Setup);
}
