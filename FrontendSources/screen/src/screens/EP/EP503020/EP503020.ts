import {
	PXScreen,
	graphInfo,
	PXView,
	createCollection,
	PXFieldState,
	gridConfig,
	PXActionState,
	GridPreset,
	GridFilterBarVisibility,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.EP.ReassignDelegatedActivitiesProcess",
	primaryView: "Records",
})
export class EP503020 extends PXScreen {
	Records = createCollection(EPApprovalWingmanFilter);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	showFilterBar: GridFilterBarVisibility.OnDemand,
	initNewRow: true,
	suppressNoteFiles: true,
})
export class EPApprovalWingmanFilter extends PXView {
	Selected: PXFieldState;
	DelegationOf: PXFieldState;
	OrigOwnerID: PXFieldState;
	OwnerID: PXFieldState;
	DelegatedToContactID: PXFieldState;
	IsActive: PXFieldState;
	StartsOn: PXFieldState;
	ExpiresOn: PXFieldState;
	DocType: PXFieldState;
	RefNoteID: PXFieldState;
	CreatedDateTime: PXFieldState;
	Descr: PXFieldState;
}
