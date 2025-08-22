import { PXView, PXFieldState, gridConfig } from 'client-controls';

// Views

export class PreferencesGeneral extends PXView {
	MaxUploadSize: PXFieldState;
}

export class UploadAllowedFileTypes extends PXView {
	FileExt: PXFieldState;
	IconUrl: PXFieldState;
	Forbidden: PXFieldState;
	IsImage: PXFieldState;
	DefApplication: PXFieldState;
}