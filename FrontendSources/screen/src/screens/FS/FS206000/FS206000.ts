import {
	graphInfo,
	createSingle,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.BillingCycleMaint', primaryView: 'BillingCycleRecords' })
export class FS206000 extends PXScreen {
	BillingCycleRecords = createSingle(FSBillingCycle);
}

export class FSBillingCycle extends PXView {
	BillingCycleCD: PXFieldState;
	Descr: PXFieldState;
	BillingBy: PXFieldState<PXFieldOptions.CommitChanges>;
	BillingCycleType: PXFieldState<PXFieldOptions.CommitChanges>;
	TimeCycleType: PXFieldState<PXFieldOptions.CommitChanges>;
	TimeCycleDayOfMonth: PXFieldState<PXFieldOptions.CommitChanges>;
	TimeCycleWeekDay: PXFieldState<PXFieldOptions.CommitChanges>;
	GroupBillByLocations: PXFieldState;
	InvoiceOnlyCompletedServiceOrder: PXFieldState;
}
