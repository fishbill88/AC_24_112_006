import {
	PXScreen,
	graphInfo,
	PXView,
	createCollection,
	PXFieldState,
	gridConfig,
	linkCommand,
	PXFieldOptions,
	columnConfig,
	GridColumnShowHideMode,
	GridPreset,
	GridFilterBarVisibility,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.EP.ActivitiesEnq",
	primaryView: "Activities",
})
export class EP404300 extends PXScreen {
	Activities = createCollection(Activities);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	showFilterBar: GridFilterBarVisibility.OnDemand,
	initNewRow: true,
	allowUpdate: false,
})
export class Activities extends PXView {
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False, width: 35 })
	IsCompleteIcon: PXFieldState;
	@linkCommand("ViewActivity") Subject: PXFieldState;
	UIStatus: PXFieldState;
	StartDate: PXFieldState;
	EndDate: PXFieldState;
	DayOfWeek: PXFieldState;
	TimeSpent: PXFieldState;
	OvertimeSpent: PXFieldState;
	TimeBillable: PXFieldState;
	OvertimeBillable: PXFieldState;
	ClassID: PXFieldState;
	Type: PXFieldState;
	@columnConfig({ hideViewLink: true }) ProjectID: PXFieldState;
	@columnConfig({ hideViewLink: true }) ProjectTaskID: PXFieldState;
	WorkgroupID: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("viewOwner") OwnerID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("viewEntity") Source: PXFieldState;
}
