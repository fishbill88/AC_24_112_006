import {
	PXView,
	PXFieldState,
	gridConfig,
	selectorSettings,
	PXFieldOptions,
	columnConfig,
	GridColumnShowHideMode,
	GridColumnType,
	TextAlign,
	GridPreset
} from "client-controls";

export class LienWaiverSetup extends PXView {
	ShouldWarnOnBillEntry: PXFieldState<PXFieldOptions.CommitChanges>;
	ShouldWarnOnPayment: PXFieldState<PXFieldOptions.CommitChanges>;
	ShouldStopPayments: PXFieldState<PXFieldOptions.CommitChanges>;
	ShouldGenerateConditional: PXFieldState<PXFieldOptions.CommitChanges>;
	GenerateWithoutCommitmentConditional: PXFieldState<PXFieldOptions.CommitChanges>;
	GenerationEventConditional: PXFieldState;
	ThroughDateSourceConditional: PXFieldState;
	GroupByConditional: PXFieldState;
	ShouldGenerateUnconditional: PXFieldState<PXFieldOptions.CommitChanges>;
	GenerateWithoutCommitmentUnconditional: PXFieldState<PXFieldOptions.CommitChanges>;
	GenerationEventUnconditional: PXFieldState;
	ThroughDateSourceUnconditional: PXFieldState;
	GroupByUnconditional: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class NotificationSetup extends PXView {
	@columnConfig({
		allowNull: false,
		textAlign: TextAlign.Center,
		type: GridColumnType.CheckBox
	})
	Active: PXFieldState;
	NotificationCD: PXFieldState;
	NBranchID: PXFieldState;
	EMailAccountID: PXFieldState;
	DefaultPrinterID: PXFieldState;
	@selectorSettings("ScreenID", "")
	@columnConfig({ format: "CC.CC.CC.CC" }) ReportID: PXFieldState;
	NotificationID: PXFieldState;
	@columnConfig({ allowNull: false }) Format: PXFieldState;
	RecipientsBehavior: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class NotificationSetupRecipient extends PXView {
	@columnConfig({
		width: 60,
		textAlign: TextAlign.Center,
		type: GridColumnType.CheckBox
	})
	Active: PXFieldState;
	@columnConfig({ width: 100 })
	ContactType: PXFieldState;
	@columnConfig({
		visible: false,
		allowShowHide: GridColumnShowHideMode.False
	})
	OriginalContactID: PXFieldState<PXFieldOptions.Hidden>;
	@selectorSettings("DisplayName", "")
	ContactID: PXFieldState;
	Format: PXFieldState;
	AddTo: PXFieldState;
}

export class ComplianceAttributeFilter extends PXView {
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
})
export class ComplianceAttribute extends PXView {
	Value: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class CSAttributeGroup extends PXView {
	AttributeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	@columnConfig({
		textAlign: TextAlign.Center,
		type: GridColumnType.CheckBox
	})
	Required: PXFieldState;
}
