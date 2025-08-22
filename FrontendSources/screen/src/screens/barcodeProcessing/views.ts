import {
	PXView,
	PXFieldState,
	PXFieldOptions,

	gridConfig,
	GridPreset,
} from "client-controls";

export class ScanHeaderBase extends PXView {
	Barcode: PXFieldState; // must not have CommitChanges, because the related control has its own custom commit logic
	ProcessingSucceeded: PXFieldState<PXFieldOptions.Disabled>;
	Message: PXFieldState<PXFieldOptions.Disabled | PXFieldOptions.Multiline | PXFieldOptions.NoLabel>;
}

export class ScanInfo extends PXView {
	Mode: PXFieldState<PXFieldOptions.Disabled>;
	Message: PXFieldState<PXFieldOptions.Disabled>;
	MessageSoundFile: PXFieldState<PXFieldOptions.Disabled>;
	Instructions: PXFieldState<PXFieldOptions.Disabled>;
	Prompt: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	allowUpdate: false
})
export class ScanLogs extends PXView {
	ScanTime: PXFieldState<PXFieldOptions.Disabled>;
	Mode: PXFieldState<PXFieldOptions.Disabled>;
	Prompt: PXFieldState<PXFieldOptions.Disabled>;
	Scan: PXFieldState<PXFieldOptions.Disabled>;
	Message: PXFieldState<PXFieldOptions.Disabled>;
	MessageType: PXFieldState<PXFieldOptions.Disabled | PXFieldOptions.Hidden>;
}
