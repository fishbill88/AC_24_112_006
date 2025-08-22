import {
	createCollection,
	PXScreen,
	PXView,
	graphInfo,
	PXFieldState,
	gridConfig,
	columnConfig,
	GridColumnShowHideMode,
	linkCommand,
	GridPreset,
	GridFilterBarVisibility,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.GDPR.GDPRToolsAuditMaint",
	primaryView: "Log",
	bpEventsIndicator: false,
	udfTypeField: "",
})
export class GD101010 extends PXScreen {
	Log = createCollection(SMPersonalDataLog);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	showFilterBar: GridFilterBarVisibility.OnDemand,
	allowUpdate: false,
})
export class SMPersonalDataLog extends PXView {
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	PseudonymizationStatus: PXFieldState;
	@linkCommand("OpenContact")
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	UIKey: PXFieldState;
	TableName: PXFieldState;
	CreatedByID: PXFieldState;
	CreatedDateTime: PXFieldState;
}
