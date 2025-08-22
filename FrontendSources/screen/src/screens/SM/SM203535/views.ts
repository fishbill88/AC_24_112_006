import { PXView, PXFieldState, gridConfig, PXFieldOptions } from 'client-controls';

// Views

export class SendTestMessageDialog extends PXView {
	TO: PXFieldState<PXFieldOptions.CommitChanges>;
	Body: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class SmsPlugin extends PXView {
	Name: PXFieldState;
	PluginTypeName: PXFieldState<PXFieldOptions.CommitChanges>;
	IsDefault: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({allowDelete: false, allowInsert: false, autoAdjustColumns: true, fastFilterByAllFields: false })
export class SmsPluginParameter extends PXView {
	Name: PXFieldState;
	Description: PXFieldState;
	Value: PXFieldState;
}