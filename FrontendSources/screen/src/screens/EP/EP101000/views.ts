import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	GridColumnType,
	GridColumnShowHideMode,
	linkCommand,
	gridConfig
} from "client-controls";

export class GenerateWeeksDialog extends PXView {
	FromDate: PXFieldState<PXFieldOptions.CommitChanges>;
	TillDate: PXFieldState<PXFieldOptions.CommitChanges>;
	CutOffDayOne: PXFieldState<PXFieldOptions.CommitChanges>;
	DayOne: PXFieldState<PXFieldOptions.CommitChanges>;
	CutOffDayTwo: PXFieldState<PXFieldOptions.CommitChanges>;
	DayTwo: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class Setup extends PXView {
	ClaimNumberingID: PXFieldState;
	ReceiptNumberingID: PXFieldState;
	TimeCardNumberingID: PXFieldState;
	EquipmentTimecardNumberingID: PXFieldState;
	ClaimDetailsAssignmentMapID: PXFieldState;
	ClaimAssignmentMapID: PXFieldState;
	TimeCardAssignmentMapID: PXFieldState;
	EquipmentTimecardAssignmentMapID: PXFieldState;
	ClaimDetailsAssignmentNotificationID: PXFieldState;
	ClaimAssignmentNotificationID: PXFieldState;
	TimeCardAssignmentNotificationID: PXFieldState;
	EquipmentTimeCardAssignmentNotificationID: PXFieldState;
	GroupTransactgion: PXFieldState;
	AutomaticReleaseAR: PXFieldState;
	AutomaticReleaseAP: PXFieldState;
	AutomaticReleasePM: PXFieldState;
	CopyNotesAR: PXFieldState;
	CopyFilesAR: PXFieldState;
	CopyNotesAP: PXFieldState;
	CopyFilesAP: PXFieldState;
	CopyNotesPM: PXFieldState;
	CopyFilesPM: PXFieldState;
	SalesSubMask: PXFieldState;
	ExpenseSubMask: PXFieldState;
	NonTaxableTipItem: PXFieldState<PXFieldOptions.CommitChanges>;
	UseReceiptAccountForTips: PXFieldState;
	HoldEntry: PXFieldState;
	PostSummarizedCorpCardExpenseReceipts: PXFieldState;
	RequireRefNbrInExpenseReceipts: PXFieldState;
	AllowMixedTaxSettingInClaims: PXFieldState;
	RequireTimes: PXFieldState;
	DefaultActivityType: PXFieldState<PXFieldOptions.CommitChanges>;
	MinBillableTime: PXFieldState;
	RegularHoursType: PXFieldState<PXFieldOptions.CommitChanges>;
	HolidaysType: PXFieldState<PXFieldOptions.CommitChanges>;
	VacationsType: PXFieldState<PXFieldOptions.CommitChanges>;
	isPreloadHolidays: PXFieldState;
	PostingOption: PXFieldState<PXFieldOptions.CommitChanges>;
	OffBalanceAccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomWeek: PXFieldState<PXFieldOptions.CommitChanges>;
	FirstDayOfWeek: PXFieldState;
}

export class WeekFilter extends PXView {
	Year: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true,
	initNewRow: true
})
export class CustomWeek extends PXView {
	// eslint-disable-next-line id-denylist
	Number: PXFieldState;
	IsActive: PXFieldState;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	IsFullWeek: PXFieldState;
}

