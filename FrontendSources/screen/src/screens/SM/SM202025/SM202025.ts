import { createCollection, createSingle, PXScreen, graphInfo,
	PXActionState, PXView, PXFieldState, PXFieldOptions,
	columnConfig, gridConfig, GridColumnShowHideMode, GridPreset } from 'client-controls';

@graphInfo({graphType: 'PX.SM.WikiStatusMaint', primaryView: 'fltStatusRecords', hideFilesIndicator: true, hideNotesIndicator: true})
export class SM202025 extends PXScreen {
	fltStatusRecords = createSingle(WikiPageStatusFilter);
	ArticlesByStatusRecords = createCollection(WikiPage);
}

export class WikiPageStatusFilter extends PXView {
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	MyOwner: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	MyWorkGroup: PXFieldState<PXFieldOptions.CommitChanges>;
	MyEscalated: PXFieldState<PXFieldOptions.CommitChanges>;
	UserID: PXFieldState<PXFieldOptions.CommitChanges>;
	StatusID: PXFieldState<PXFieldOptions.CommitChanges>;
	WikiID: PXFieldState<PXFieldOptions.CommitChanges>;
	FolderID: PXFieldState<PXFieldOptions.CommitChanges>;
	CreatedFrom: PXFieldState<PXFieldOptions.CommitChanges>;
	CreatedTill: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	allowUpdate: false
})
export class WikiPage extends PXView {
	view: PXActionState;
	Process: PXActionState;
	ProcessAll: PXActionState;
	@columnConfig({allowCheckAll: true, allowShowHide: GridColumnShowHideMode.False})
	Selected: PXFieldState;
	Name: PXFieldState;
	Title: PXFieldState;
	Summary: PXFieldState;
	Keywords: PXFieldState;
	Path: PXFieldState;
	LastModifiedByID_Modifier_Username: PXFieldState;
	StatusID: PXFieldState;
	Versioned: PXFieldState;
}