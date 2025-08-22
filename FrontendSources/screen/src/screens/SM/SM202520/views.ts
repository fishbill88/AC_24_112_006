import { PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, PXActionState, TextAlign } from "client-controls";

// Views

export class ClearDateFilter extends PXView  {
	Till : PXFieldState<PXFieldOptions.CommitChanges>;
}

export class GetLinkFilterType extends PXView  {
	WikiLink : PXFieldState;
}

export class FilesFilter extends PXView  {
	DocName : PXFieldState<PXFieldOptions.CommitChanges>;
	DateCreatedFrom : PXFieldState<PXFieldOptions.CommitChanges>;
	AddedBy : PXFieldState<PXFieldOptions.CommitChanges>;
	CheckedOutBy : PXFieldState<PXFieldOptions.CommitChanges>;
	DateCreatedTo : PXFieldState<PXFieldOptions.CommitChanges>;
	ScreenID : PXFieldState<PXFieldOptions.CommitChanges>;
	ShowUnassignedFiles : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	allowDelete: false,
	allowInsert: false,
	topBarItems: {
		getFile: {index: 0, config: {commandName: "getFile", text: "Get file" }},
		getFileLink: {index: 1, config: {commandName: "getFileLink", text: "Get link" }},
		deleteFile: {index: 2, config: {commandName: "deleteFile", text: "Delete file" }},
		addLink: {index: 3, config: {commandName: "addLink", text: "Add Link" }},
		addLinkClose: {index: 4, config: {commandName: "addLinkClose", text: "Add Link & Close" }}
	}
})
export class UploadFile extends PXView  {
	getFile : PXActionState;
	getFileLink : PXActionState;
	deleteFile : PXActionState;
	addLink : PXActionState;
	addLinkClose : PXActionState;
	@columnConfig({width: 800})	Name : PXFieldState<PXFieldOptions.Readonly>;
	@columnConfig({allowUpdate: false, width: 90})	CreatedDateTime : PXFieldState;
	@columnConfig({allowUpdate: false, width: 108})	CreatedByID_Creator_Username : PXFieldState;
	@columnConfig({width: 150})	CheckedOutComment : PXFieldState;
}