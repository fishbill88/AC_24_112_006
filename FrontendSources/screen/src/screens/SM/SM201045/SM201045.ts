// TODO: ScreenID links don't work (https://jira.acumatica.com/browse/AC-290765)
// TODO: the Collapse button hides all fields on the form (https://jira.acumatica.com/browse/AC-290766)
// TODO: the Date column in the grid is too narrow (https://jira.acumatica.com/browse/AC-290767)
// TODO: the Operation column in the grid is too narrow (https://jira.acumatica.com/browse/AC-290577)
// TODO: disable FastFilter in the grid (https://jira.acumatica.com/browse/AC-290459)
// TODO: hide Import from Excel button (https://jira.acumatica.com/browse/AC-290457)

import {
	PXScreen,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	createSingle,
} from "client-controls";

@graphInfo({
	graphType: "PX.SM.LoginTraceMaintenance",
	primaryView: "TraceFilter",
})
export class SM201045 extends PXScreen {
	TraceFilter = createSingle(TraceFilter);
	LoginTraces = createCollection(LoginTrace);
}

export class TraceFilter extends PXView {
	Username: PXFieldState<PXFieldOptions.CommitChanges>;

	DateFrom: PXFieldState<PXFieldOptions.CommitChanges>;

	DateTo: PXFieldState<PXFieldOptions.CommitChanges>;

	Operation: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class LoginTrace extends PXView {
	Date: PXFieldState;

	Username: PXFieldState;

	Operation: PXFieldState;

	Host: PXFieldState;

	IPAddress: PXFieldState;

	ScreenID: PXFieldState;

	ScreenID_SiteMap_Title: PXFieldState;

	Comment: PXFieldState;
}
