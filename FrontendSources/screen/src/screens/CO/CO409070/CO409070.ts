import {
	createCollection,
	PXScreen,
	PXView,
	graphInfo,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	columnConfig,
	GridColumnShowHideMode,
	linkCommand,
	GridPreset,
	GridFilterBarVisibility,
} from "client-controls";

@graphInfo({ graphType: "PX.Objects.SM.EmailEnq", primaryView: "Emails" })
export class CO409070 extends PXScreen {
	Emails = createCollection(SMEmail);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	showFilterBar: GridFilterBarVisibility.OnDemand,
	quickFilterFields: ["Subject", "MailFrom", "EMailAccount__Description"],
	allowUpdate: false,
})
export class SMEmail extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.True })
	CRActivity__ClassIcon: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.True })
	CRActivity__Type: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.True })
	IsIncome: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("ViewEMail")
	Subject: PXFieldState;
	MailFrom: PXFieldState;
	MailTo: PXFieldState;
	MPStatus: PXFieldState;
	CreatedDateTime: PXFieldState;
	CreatedByID_Creator_Username: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.True })
	MailCc: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.True })
	MailBcc: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("ViewEntity")
	CRActivity__Source: PXFieldState;
	CRActivity__RefNoteID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true })
	CRActivity__OwnerID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CRActivity__WorkgroupID: PXFieldState;
	MessageID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.True })
	IsArchived: PXFieldState<PXFieldOptions.Hidden>;
}
