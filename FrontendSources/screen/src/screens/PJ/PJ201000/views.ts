import {
	gridConfig,
	linkCommand,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class ProjectManagementClasses extends PXView {
	ProjectManagementClassId: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.Multiline>;
	UseForProjectIssue: PXFieldState<PXFieldOptions.CommitChanges>;
	UseForRequestForInformation: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ProjectManagementClassesCurrent extends PXView {
	RequestForInformationResponseTimeFrame: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectIssueResponseTimeFrame: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class Attributes extends PXView {
	IsActive: PXFieldState;
	@linkCommand("CRAttribute_ViewDetails")
	AttributeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	SortOrder: PXFieldState;
	Required: PXFieldState;
	CSAttribute__IsInternal: PXFieldState;
	ControlType: PXFieldState;
	DefaultValue: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class ProjectManagementClassPriority extends PXView {
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	PriorityName: PXFieldState;
	SortOrder: PXFieldState;
	IsDefault: PXFieldState<PXFieldOptions.CommitChanges>;
}
