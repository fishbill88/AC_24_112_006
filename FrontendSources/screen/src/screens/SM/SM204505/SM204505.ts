import {
	createCollection,
	createSingle,
	graphInfo,
	viewInfo,
	gridConfig,
	linkCommand,
	columnConfig,
	PXActionState,
	PXFieldOptions,
	PXFieldState,
	PXScreen,
	PXView
} from 'client-controls';

@graphInfo({graphType: 'PX.SM.ProjectList', primaryView: 'Projects'})
export class SM204505 extends PXScreen {
	ReloadPage: PXActionState;
	ActionPanelValidateExtOK: PXActionState;
	ActionPanelChooseProjects: PXActionState;
	ActionPanelChooseProjectsAllMessages: PXActionState;
	ActionPanelChooseProjectsCancel: PXActionState;

	@viewInfo({containerName: ''})
	Projects = createCollection(CustProject);

	@viewInfo({containerName: 'Publish to Multiple Tenants'})
	ViewCompanyList = createCollection(RowSelectCompany);

	@viewInfo({containerName: 'Publish to Multiple Tenants'})
	ViewPublishOptions = createSingle(RowPublishOptions);

	@viewInfo({containerName: 'Validation Results'})
	ViewValidateExtensions = createSingle(RowValidate);

	@viewInfo({containerName: 'Choose projects'})
	ProjectsChooser = createCollection(CustProject2);
}

// Views
@gridConfig({adjustPageSize: true, syncPosition: true, autoAdjustColumns: true, pageSize: 50, batchUpdate: false})
export class CustProject extends PXView {
	@linkCommand('view')
	Name: PXFieldState;

	@columnConfig({allowCheckAll: true})
	IsWorking: PXFieldState;

	Description: PXFieldState;
	CreatedByID: PXFieldState<PXFieldOptions.Disabled>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Disabled>;
	LastModifiedByID: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({allowUpdate: false})
	LastModifiedDateTime: PXFieldState;

	IsPublished: PXFieldState;
	Level: PXFieldState;
	ScreenNames: PXFieldState;
	CertificationStatus: PXFieldState;
	Initials: PXFieldState;

	@columnConfig({allowUpdate: false})
	CreatedByID_Creator_Username: PXFieldState;
}

@gridConfig({allowDelete: false, allowInsert: false, adjustPageSize: true, autoAdjustColumns: true})
export class RowSelectCompany extends PXView {
	Selected: PXFieldState;
	Name: PXFieldState;
	ID: PXFieldState;
	ParentID: PXFieldState;
}

export class RowPublishOptions extends PXView {
	PublishOnlyDB: PXFieldState;
	DisableOptimization: PXFieldState;
}

export class RowValidate extends PXView {
	Messages: PXFieldState;
}

@gridConfig({adjustPageSize: true, autoAdjustColumns: true})
export class CustProject2 extends PXView {
	@columnConfig({allowCheckAll: true})
	Selected: PXFieldState;

	@columnConfig({allowUpdate: false})
	IsPublished: PXFieldState;

	@columnConfig({allowUpdate: false})
	Name: PXFieldState;

	@columnConfig({allowUpdate: false})
	Description: PXFieldState;
}