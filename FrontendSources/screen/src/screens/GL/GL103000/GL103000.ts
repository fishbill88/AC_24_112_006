import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, PXFieldOptions, createCollection, columnConfig
} from 'client-controls';


@graphInfo({ graphType: 'PX.Objects.GL.GLConsolSetupMaint', primaryView: 'GLSetupRecord' })
export class GL103000 extends PXScreen {
	GLSetupRecord = createSingle(GLSetupRecord);
	ConsolSetupRecords = createCollection(ConsolSetupRecords);
}

export class GLSetupRecord extends PXView {
	ConsolSegmentId: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ConsolSetupRecords extends PXView {

	@columnConfig({ allowCheckAll: true, allowSort: false })
	Selected: PXFieldState;

	IsActive: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	LedgerId: PXFieldState;

	SegmentValue: PXFieldState;
	PasteFlag: PXFieldState;
	Description: PXFieldState;
	Login: PXFieldState;
	Password: PXFieldState;
	Url: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SourceLedgerCD: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	SourceBranchCD: PXFieldState;

	@columnConfig({ hideViewLink: true })
	StartPeriod: PXFieldState;

	@columnConfig({ hideViewLink: true })
	EndPeriod: PXFieldState;

	LastPostPeriod: PXFieldState;
	LastConsDate: PXFieldState;
	BypassAccountSubValidation: PXFieldState;
}
