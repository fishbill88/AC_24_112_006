import { createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, PXFieldOptions } from "client-controls";

@graphInfo({ graphType: "PX.Objects.FA.FABookPeriodsMaint", primaryView: "BookYear", })
export class FA304000 extends PXScreen {

	BookYear = createSingle(FABookYear);
	BookPeriod = createCollection(FABookPeriod);
}

export class FABookYear extends PXView {

	BookID: PXFieldState<PXFieldOptions.CommitChanges>;
	OrganizationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Year: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState;
	FinPeriods: PXFieldState;

}

export class FABookPeriod extends PXView {

	FinPeriodID: PXFieldState;
	StartDateUI: PXFieldState;
	EndDateUI: PXFieldState;
	Descr: PXFieldState;

}
